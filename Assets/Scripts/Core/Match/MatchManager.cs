using System;
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Rule;


namespace Yujanggi.Core.Match
{
    public class MatchEvents
    {
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
        public Record       Record { get; }
        public Score        Score { get; }
        public BoardModel   Board { get; }
        public MatchEvents  MatchEvent { get; }
        public PlayerTeam   Bottom { get; }
    }
    public class MatchManager : IMatchManager
    {
        public  MatchEvents  MatchEvent { get; } = new();
        public  Turn         Turn { get; }
        public  Record       Record { get; }
        public  Score        Score { get; }
        public  BoardModel   Board { get; }
        public  JanggiRule   Rule { get; }
        public PlayerTeam    Bottom => _bottom;
        private readonly SelectionState _selection;

        private readonly PlayerTeam _bottom;

        public MatchManager(PlayerTeam bottom = PlayerTeam.Cho, float maxTime=30f)
        {
            _selection  = new SelectionState(_bottom);
            _bottom     = bottom;

            Turn        = new Turn(maxTime);
            Record      = new Record();
            Score       = new Score();
            Board       = new BoardModel(_bottom);
            Rule        = new JanggiRule(_bottom);

            BoardInitializer.SetUpPieces(Board, _bottom);
        }
        public void ResetGame()
        {
            ClearSelection();

            Turn.StartGame(PlayerTeam.Cho);
            Record.StartGame();
            Score.StartGame();
            Board.ResetBoard();
            BoardInitializer.SetUpPieces(Board, _bottom);
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
            info.Winner = Turn.Current;
            info.MoveCnt = Record.MoveCount;
            info.Type = GameResult.GiveUp;
            return info;
        }
        public void Update(float deltaTime)
        {
            Turn.Update(deltaTime);
        }
        public BoardActionResult  HandleCellClick(Pos pos)
        {
            if (Turn.IsEnd)
                return BoardActionResult.None;

            if (!Board.IsInside(pos))
                return BoardActionResult.None;

            if (!_selection.HasSelection)
                return HandleSelectPiece(pos, Turn.Current);

            return HandleSelectedClick(pos, Turn.Current);
        }


        private BoardActionResult HandleSelectPiece(Pos pos, PlayerTeam turn)
        {
            if (!CanSelectPiece(pos, turn))
                return BoardActionResult.SelectFailed;

            SelectPeice(pos);
            FindWays();
            
            return BoardActionResult.SelectSuccess;
        }
        private BoardActionResult HandleSelectedClick(Pos pos, PlayerTeam turn)
        {
            if (CanSelectPiece(pos, turn))
                return ReselectPiece(pos, turn);

            if (!_selection.IsMovable(pos))
            {
                ClearSelection();
                return BoardActionResult.MoveFailed;
            }

            return MoveSelectedPiece(pos, turn);
        }
        private BoardActionResult ReselectPiece(Pos pos, PlayerTeam turn)
        {
            if (pos == _selection.SelectedPos)
            {
                ClearSelection();
                return BoardActionResult.SelectFailed;
            }

            SelectPeice(pos);
            FindWays();
            return BoardActionResult.Reselect;
        }
        private BoardActionResult MoveSelectedPiece(Pos toPos, PlayerTeam turn)
        {
            var fromPos = _selection.SelectedPos;
            var record = Board.DoMove(fromPos, toPos);
            if (record.IsCapture) MatchEvent.PieceCaptured(record.CapturedPiece.Type);
            var otherTeam = turn == PlayerTeam.Cho ? PlayerTeam.Han : PlayerTeam.Cho;
            var isJanggun = IsJanggun(otherTeam);
            var isEnd = isJanggun && HasAnyLegalMove(otherTeam);

            MunggunCheck();
            MatchEvent.PieceMoved(record);
            Turn.NextTurn();
            Record.Push(new(record, isJanggun, isEnd));
            ClearSelection();
  
            return record.IsCapture
                ? BoardActionResult.CaptureSuccess
                : BoardActionResult.MoveSuccess;
        }
        private bool CanSelectPiece(Pos pos, PlayerTeam turn)
        {
            if (!Board.HasPiece(pos))
                return false;

            var pieceInfo = Board.GetPiece(pos);

            return pieceInfo.Team == turn;
        }
        private void SelectPeice(Pos pos)
        {
            ClearSelection();
            var pieceInfo = Board.GetPiece(pos);
            _selection.Select(pieceInfo, pos);
        }
        private void ClearSelection()
        {
            _selection.Clear();
            // Array.Empty - 싱글톤 (재사용)
            MatchEvent.SeletionChanged(null, Array.Empty<Pos>());
        }
        private void FindWays()
        {
            Rule.FindWays(Board, _selection);
            MatchEvent.SeletionChanged(_selection.SelectedPiece.Id, _selection.MovableCells);
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
                info.Winner = Turn.Current;
                MatchEvent.GameEnd(info);
            }
                

            return result;
        }
    }
}