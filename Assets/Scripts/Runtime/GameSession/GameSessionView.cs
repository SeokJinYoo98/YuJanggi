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
        public SessionDisplayMode DisplayMode { get; private set; } = SessionDisplayMode.Live;
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

        public void BindEvents(IMatchManager match)
        {
            var events = match.MatchEvent;

            events.OnCheckOccurred    += HandleCheckOccured;
            events.OnCheckReleased    += HandleCheckReleased;
            events.OnPieceMoved       += HandlePieceMoved;

            _matchUI.BindEvents(match);
        }
        public void UnBindEvents(IMatchManager match)
        {
            var events = match.MatchEvent;

            events.OnCheckOccurred    -= HandleCheckOccured;
            events.OnCheckReleased    -= HandleCheckReleased;
            events.OnPieceMoved       -= HandlePieceMoved;

            _matchUI.UnBindEvents(match);
        }
        // Local
        public void SetDisplayMode(SessionDisplayMode mode)
            => DisplayMode = mode;
        public void HandleSelectionChanged(int? id, IReadOnlyList<Pos> legal, IReadOnlyList<Pos> illegal)
        {
            _board.UnHighlight();

            if (!id.HasValue)
                return;

            _board.Highlight(id.Value, legal, illegal);
            _audio.PlaySfxOneShot(JanggiSfx.Select);
        }
        // 통합
        public void HandlePieceMoved(MoveRecord record)
        {
            if (DisplayMode != SessionDisplayMode.Live) return;

            _board.MovePiece(record.MovedPiece.Id, record.To);

            if (record.IsCapture)
            {
                _board.PlaceCapturedPiece(record.CapturedPiece.Id, record.CapturedPiece.Team);
                _audio.PlaySfxOneShot(JanggiSfx.Capture);
                return;
            }

            _audio.PlaySfxOneShot(JanggiSfx.Move);
        }
        public void HandleCheckOccured(PlayerTeam team)
        {
            _audio.PlaySfxOneShot(JanggiSfx.Check);
            _matchUI.PlayJanggun(team);
        }
        public void HandleCheckReleased()
            => _audio.PlaySfxOneShot(JanggiSfx.UnCheck);
        // 기타
        public void OnTurnChanged(PlayerTeam team, bool isLocal)
        {
            if (isLocal) _audio.PlaySfxOneShot(JanggiSfx.TurnAlert);
            _matchUI.UpdateTurn(team);
        }
        public void OnGameEnded(bool winnerIsLocal, in GameResultInfo info)
        {
            if (winnerIsLocal) _audio.PlaySfxOneShot(JanggiSfx.Win);
            else _audio.PlaySfxOneShot(JanggiSfx.Lose);

            _resultUI.Show();
            _resultUI.EndGame(info);
        }
        public void ShowResultUI(in GameResultInfo info)
        {
            _resultUI.Show();
            _resultUI.GiveUp(info);
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
    }
}
