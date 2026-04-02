using System;
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Rule;

namespace Yujanggi.Core.Match
{
    public class MatchEvents
    {
        // Local AI Network 분리
        public event Action<int?, IReadOnlyList<Pos>> OnSelectionChanged;
        public event Action<MoveRecord>               OnPieceMoved;
        public event Action<PlayerTeam>               OnCheck;
        public event Action                           OnMunggun;
        public event Action<PieceType>                OnPieceCaptured;
        public event Action<GameResultInfo>           GameEnded;

        public void SeletionChanged(int? id, IReadOnlyList<Pos> pos)
            => OnSelectionChanged?.Invoke(id, pos);
        public void PieceMoved(MoveRecord record)
            => OnPieceMoved?.Invoke(record);
        public void CheckOccured(PlayerTeam team)
            => OnCheck?.Invoke(team);
        public void MunggunOccured()
            => OnMunggun?.Invoke();
        public void PieceCaptured(PieceType type)
            => OnPieceCaptured?.Invoke(type);
        public void GameEnd(GameResultInfo info)
            => GameEnded?.Invoke(info);
    }
    public interface IMatchManager
    {
        public Turn         Turn { get; }
        public JanggiRule   Rule { get; }
        public Record       Record { get; }
        public Score        Score { get; }
        public BoardModel   Board { get; }
        public MatchEvents  MatchEvent { get; }
    }
    public class MatchManager : IMatchManager
    {
        public  MatchEvents  MatchEvent { get; } = new();
        public  Turn         Turn { get; }
        public  Record       Record { get; }
        public  Score        Score { get; }
        public  BoardModel   Board { get; }
        public  JanggiRule   Rule { get; }

        public bool HasSelection => _selection.HasSelection;
        public Pos  SelectedPos  => _selection.SelectedPos;

        private readonly SelectionState _selection;
        
        public bool TrySelect(Pos pos)
        {
            ClearSelection();

            if (!Board.IsInside(pos) || !Board.HasPiece(pos))
                return false;
            

            var piece = Board.GetPiece(pos);
            if (piece.Team != Turn.CurrentTeam)
                return false;
            
            _selection.Select(piece, pos);
            Rule.FindWays(Board, _selection);
            MatchEvent.SeletionChanged(_selection.SelectedPiece.Id, _selection.MovableCells);

            return true;
        }
        public bool TryMoveSelected(Pos to)
        {
            if (!Board.IsInside(to))
            {
                ClearSelection();
                return false;
            }

            if (_selection.SelectedPos == to)
            {
                ClearSelection();
                return false;
            }

            if (!_selection.IsMovable(to))
            {
                ClearSelection();
                return false;
            }

            var from = _selection.SelectedPos;
            ExecuteMove(from, to);
            ClearSelection();
            return true;
        }
        public bool TryMove(Pos from, Pos to)
        {
            if (!Board.IsInside(from) || !Board.IsInside(to))
                return false;

            if (!Board.HasPiece(from))
                return false;
            if (!Rule.CanMove(Board, from, to))
                return false;

            ExecuteMove(from, to);
            return true;
        }
        private void ExecuteMove(Pos from, Pos to)
        {
            var record = Board.DoMove(from, to);

            if (record.IsCapture)
                MatchEvent.PieceCaptured(record.CapturedPiece.Type);

            var otherTeam = Turn.CurrentTeam == PlayerTeam.Cho
                ? PlayerTeam.Han
                : PlayerTeam.Cho;

            var isJanggun = IsJanggun(otherTeam);
            var isEnd = isJanggun && !HasAnyLegalMove(otherTeam);

            var ctx = new MoveContext(record, isJanggun, isEnd);

            Record.Push(ctx);
            MunggunCheck();
            MatchEvent.PieceMoved(record);
            Turn.NextTurn();
        }
        public MatchManager(float maxTime=30f)
        {
            _selection  = new SelectionState();

            Turn        = new Turn(maxTime);
            Record      = new Record();
            Score       = new Score();
            Board       = new BoardModel();
            Rule        = new JanggiRule();

            BoardInitializer.SetUpPieces(Board);
        }
        public void StartGame()
        {
            ClearSelection();

            Turn.StartGame(PlayerTeam.Cho);
            Record.StartGame();
            Score.StartGame();
            Board.ResetBoard();
            BoardInitializer.SetUpPieces(Board);
        }
        public void BindEvents()
        {
            Turn.OnTurnEnd += Handicap;
        }
        public void UnBindEvents()
        {
            Turn.OnTurnEnd -= Handicap;
        }
        public bool TryUnDo(out MoveContext ctx)
        {
            if (!Record.TryPop(out ctx))
                return false;

            Turn.NextTurn();

            if (ctx.IsHandicap)
                return false;
            

            var record = ctx.Record;
            Board.UndoMove(record);
            
            var team = record.CapturedPiece.Team;
            var type = record.CapturedPiece.Type;
            var value = Score.GetPieceScore(type);
            Score.SetScore(team, -value);

            return true;
        }
        public void Handicap()
        {
            Record.Push(MoveContext.Handicap);
            Turn.NextTurn();
        }
 
        public GameResultInfo GiveUp()
        {
            Turn.SetTurn(TurnType.End);
            GameResultInfo info;
            info.Winner = Turn.CurrentTeam;
            info.MoveCnt = Record.MoveCount;
            info.Type = GameResult.GiveUp;
            return info;
        }
        public void Update(float deltaTime)
        {
            Turn.Update(deltaTime);
        }
        private void ClearSelection()
        {
            _selection.Clear();
            MatchEvent.SeletionChanged(null, Array.Empty<Pos>());
        }
        private bool IsJanggun(PlayerTeam otherTeam)
        {
            var result = Rule.IsKingInCheck(Board, otherTeam);
            if (result) MatchEvent.CheckOccured(otherTeam);
            return result;
        }
        private void MunggunCheck()
        {
            if (Record.TryPeek(out var ctx) && ctx.IsJanggun)
                MatchEvent.MunggunOccured();
        }
        private bool HasAnyLegalMove(PlayerTeam otherTeam)
        {
            var result = Rule.HasAnyLegalMove(Board, otherTeam);
            if (!result)
            {
                GameResultInfo info;
                info.MoveCnt = Record.MoveCount;
                info.Type = GameResult.CheckMate;
                info.Winner = Turn.CurrentTeam;
                MatchEvent.GameEnd(info);
            }
                

            return !result;
        }
    }
}