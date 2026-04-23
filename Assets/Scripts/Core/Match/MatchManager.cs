using System;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Rule;

namespace Yujanggi.Core.Match
{
    public class MatchEvents
    {
        public event Action<MoveRecord>               OnPieceMoved;
        public event Action<PlayerTeam>               OnCheckOccurred;
        public event Action                           OnCheckReleased;
        public event Action<GameResultInfo>           OnGameEnded;
        public event Action<PlayerTeam>               OnTurnChanged;
        public void PieceMoved(MoveRecord record)
            => OnPieceMoved?.Invoke(record);
        public void CheckOccurred(PlayerTeam team)
            => OnCheckOccurred?.Invoke(team);
        public void CheckReleased()
            => OnCheckReleased?.Invoke();
        public void GameEnded(GameResultInfo info)
            => OnGameEnded?.Invoke(info);
        public void TurnChanged(PlayerTeam next)
            => OnTurnChanged?.Invoke(next);
    }
    public interface IMatchViewData
    {
        public Turn         Turn { get; }
        public JanggiRule   Rule { get; }
        public Record       Record { get; }
        public Score        Score { get; }
    }
    public interface IMatchManager
    {

        public BoardModel   Board { get; }
        public MatchEvents  MatchEvent { get; }
    }
    public class MatchManager : IMatchManager, IMatchViewData
    {
        public  MatchEvents  MatchEvent { get; } = new();
        public  Turn         Turn { get; }
        public  Record       Record { get; }
        public  Score        Score { get; }
        public  BoardModel   Board { get; }
        public  JanggiRule   Rule { get; }
        public MatchManager(Turn turn, Record record, Score score, BoardModel board, JanggiRule rule)
        {
            Turn   = turn;
            Record = record;
            Score  = score;
            Board  = board;
            Rule   = rule;
        }
        public void     TryMove(Pos from, Pos to)
        {
            if (!Board.IsInside(from) || !Board.IsInside(to))
                return;

            if (!Board.HasPiece(from))
                return;

            if (Rule.IsLegalMove(Board, from, to))
                ExecuteMove(from, to);
            return;
        }
        public void     StartGame(Formation cho, Formation han)
        {
            Record.StartGame();
            Score.StartGame();
            Board.ResetBoard();
            BoardInitializer.SetUpPieces(Board, cho, han);
            Turn.StartGame(PlayerTeam.Cho);
        }
        public void     ResetGame(Formation cho, Formation han)
        {
            StartGame(cho, han);
        }
        public void     UnBindEvents()
        {
            this.Turn.OnTurnChanged  -= TurnChanged;
            this.Turn.OnTurnEnd      -= Handicap;

        }
        public void     BindEvents()
        {
            this.Turn.OnTurnChanged  += TurnChanged;
            this.Turn.OnTurnEnd      += Handicap;
        }

        public bool     TryUnDo(out MoveContext ctx)
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
  
            Score.ApplyScore(team, type);

            return true;
        }
        public void     Handicap()
        {
            Record.Push(MoveContext.Handicap);
            Turn.NextTurn();
        }
        public GameResultInfo GiveUp()
        {
            Turn.SetTurn(TurnType.End);
            GameResultInfo info;
            info.Winner     = Turn.CurrentTeam;
            info.MoveCnt    = Record.MoveCount;
            info.Type       = GameResult.GiveUp;
            return info;
        }
        public void  Update(float deltaTime)
        {
            Turn.Update(deltaTime);
        }



        private bool IsCheck(PlayerTeam otherTeam)
        {
            var result = Rule.IsKingInCheck(Board, otherTeam);
            if (result) MatchEvent.CheckOccurred(otherTeam); 
            if (Record.TryPeek(out var ctx) && ctx.IsJanggun)
                MatchEvent.CheckReleased();
            return result;
        }
        private bool HasAnyLegalMove(PlayerTeam otherTeam)
        {
            int cnt = Rule.CountLegalMove(Board, otherTeam);
            if (cnt == 0)
            {
                GameResultInfo info;
                info.MoveCnt = Record.MoveCount;
                info.Type = GameResult.CheckMate;
                info.Winner = Turn.CurrentTeam;
                MatchEvent.GameEnded(info);
            }

            return cnt != 0;
        }
        private void ExecuteMove(Pos from, Pos to)
        {
            var record = Board.DoMove(from, to);

            var otherTeam = Turn.CurrentTeam == PlayerTeam.Cho
                ? PlayerTeam.Han
                : PlayerTeam.Cho;

            if (record.IsCapture)
                Score.ApplyScore(otherTeam, record.CapturedPiece.Type);

            var isJanggun = IsCheck(otherTeam);
            var isEnd = HasAnyLegalMove(otherTeam);
            if (isEnd) Turn.SetTurn(TurnType.End);
            var ctx = new MoveContext(record, isJanggun, isEnd);

            Record.Push(ctx);
            MatchEvent.PieceMoved(record);
            Turn.NextTurn();
        }
        private void TurnChanged(PlayerTeam next)
        {
            this.MatchEvent.TurnChanged(next);
        }

    }
}