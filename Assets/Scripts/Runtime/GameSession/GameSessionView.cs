using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Runtime.Audio;
using Yujanggi.Runtime.Board;
using Yujanggi.Runtime.UI;

namespace Yujanggi.Runtime.GameSession
{
    public enum SessionDisplayMode { Live, Replay }
    public class GameSessionView
    {
        public GameSessionView(
            BoardPresenter  board,
            ResultUI        resultUI,
            MatchUI         matchUI,
            AudioManager    audio)
        {
            _board      = board;
            _resultUI   = resultUI;
            _matchUI    = matchUI;
            _audio      = audio;
        }
        public void BindUI(IMatchViewData match)
        {
            _matchUI.UnBindEvents(match);
        }
        public void UnBindUI(IMatchViewData match)
        {
            _matchUI.BindEvents(match);
        }
        public void SelectionChanged(int? pieceId, IReadOnlyList<Pos> legalCells, IReadOnlyList<Pos> illegalCells)
        {
            _board.UnHighlight();
            if (!pieceId.HasValue) return;
            _audio.PlaySfxOneShot(JanggiSfx.Select);
            _board.Highlight(pieceId.Value, legalCells, illegalCells);
        }
        public void PieceMoved(in MoveRecord record)
        {
            _board.MovePiece(record.MovedPiece.Id, record.To);
            if (record.IsCapture)
            {
                _board.PlaceCapturedPiece(record.CapturedPiece.Id, record.CapturedPiece.Team);
                _audio.PlaySfxOneShot(JanggiSfx.Capture);
                return;
            }
            _audio.PlaySfxOneShot(JanggiSfx.Move);
        }
        public void CheckOccured(PlayerTeam team)
        {
            _audio.PlaySfxOneShot(JanggiSfx.Check);
            _matchUI.PlayJanggun(team);
        }
        public void CheckReleased() 
            => _audio.PlaySfxOneShot(JanggiSfx.UnCheck);
        public void OnTurnChanged(PlayerTeam team, bool isLocal)
        {
            if (isLocal) _audio.PlaySfxOneShot(JanggiSfx.TurnAlert);
            _matchUI.UpdateTurn(team);
        }
        public void OnGameEnded(bool winnerIsLocal, in GameResultInfo info)
        {
            if (winnerIsLocal) _audio.PlaySfxOneShot(JanggiSfx.Win);
            else _audio.PlaySfxOneShot(JanggiSfx.Lose);

            ShowResultUI(in info);
        }

        public void ResetGame(IBoardModel boardModel)
        {
            _resultUI.Hide();
            _board.ResetGame(boardModel);
        }
        public void StartGame(IBoardModel boardModel)
        {
            _board.StartGame(boardModel);
        }

        public void UnDo(in MoveContext ctx)
        {
            // BoardPresenter도 UnDo만
            _board.UnHighlight();
            var movedPiece = ctx.Record.MovedPiece;

            var movedId = movedPiece.Id;
            var to = ctx.Record.From;
            _board.MovePiece(movedId, to);

            if (ctx.IsCapture)
            {
                to = ctx.Record.To;
                var captured = ctx.Record.CapturedPiece;
                _board.RestoreCapturedPiece(captured.Id, captured.Team, to);
            }
        }
       
        private readonly BoardPresenter _board;
        private readonly ResultUI       _resultUI;
        private readonly MatchUI        _matchUI;
        private readonly AudioManager   _audio;
        private void ShowResultUI(in GameResultInfo info)
        {
            _resultUI.Show();
            _resultUI.GiveUp(info);
        }
    }
}
