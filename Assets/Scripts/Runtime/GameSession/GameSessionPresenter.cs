using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Runtime.Audio;
using Yujanggi.Runtime.Board;
using Yujanggi.Runtime.UI;

namespace Yujanggi.Runtime.GameSession
{
    public class GameSessionPresenter 
    {
        public GameSessionPresenter(
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
        #region Live
        public void  BindLiveEvents(MatchEvents match)
        {
            match.OnCheckOccurred += HandleCheckOccured;
            match.OnCheckReleased += HandleCheckReleased;
            match.OnPieceMoved    += HandlePieceMoved;
        }
        public void  UnBindLiveEvents(MatchEvents match)
        {
            match.OnCheckOccurred -= HandleCheckOccured;
            match.OnCheckReleased -= HandleCheckReleased;
            match.OnPieceMoved    -= HandlePieceMoved;
        }
        private void HandleCheckOccured(PlayerTeam team)
        {
            _audio.PlaySfxOneShot(JanggiSfx.Check);
            _matchUI.PlayJanggun(team);
        }
        private void HandleCheckReleased()
            => _audio.PlaySfxOneShot(JanggiSfx.UnCheck);
        private void HandlePieceMoved(MoveRecord record)
        {
            _board.UnHighlight();
            _board.MovePiece(record.MovedPiece.Id, record.To);
            if (record.IsCapture)
            {
                _board.PlaceCapturedPiece(record.CapturedPiece.Id, record.CapturedPiece.Team);
                _audio.PlaySfxOneShot(JanggiSfx.Capture);
                return;
            }
            _audio.PlaySfxOneShot(JanggiSfx.Move);
        }


        #endregion;
        public void BindUI(IMatchViewData match)
        {
            _matchUI.BindEvents(match);
        }
        public void UnBindUI(IMatchViewData match)
        {
            _matchUI.UnBindEvents(match);
        }
        public void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legalCells, IReadOnlyList<Pos> illegalCells)
        {
            _board.UnHighlight();
            if (!pieceId.HasValue) return;
            _audio.PlaySfxOneShot(JanggiSfx.Select);
            _board.Highlight(pieceId.Value, legalCells, illegalCells);
        }
        public void OnTurnChanged(bool isLocal)
        {
            _board.UnHighlight();
            if (!isLocal) return;
            _audio.PlaySfxOneShot(JanggiSfx.TurnAlert);
        }
        public void OnGameEnded(bool loserIsLocal, in GameResultInfo info)
        {
            if (loserIsLocal) _audio.PlaySfxOneShot(JanggiSfx.Lose); 
            else _audio.PlaySfxOneShot(JanggiSfx.Win);

            ShowResultUI(in info);
        }
        public void SyncBoardState(IMatchManager match)
        {
            _board.UnHighlight();
            var board = match.Board;
            _board.SyncBoardState(board);
        }
        public void ResetGame(IBoardModel boardModel)
        {
            _resultUI.Hide();
            _board.SyncBoardState(boardModel);
        }
        public void StartGame(IBoardModel boardModel)
        {
            _board.StartGame(boardModel);
        }
        public void UnDo(MoveContext ctx)
        {
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
