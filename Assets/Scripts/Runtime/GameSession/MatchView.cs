using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Runtime.Audio;
using Yujanggi.Runtime.Board;

using Yujanggi.Runtime.UI;

using UnityEngine;

namespace Yujanggi.Runtime.GameSession
{
    public class MatchView 
    {
        public MatchView(
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
        public void CheckOccured(PlayerTeam team)
        {
            _audio.PlaySfxOneShot(JanggiSfx.Check);
            _matchUI.PlayJanggun(team);
        }
        public void CheckReleased()
            => _audio.PlaySfxOneShot(JanggiSfx.UnCheck);
        public void SyncBoardState(ILiveMatch match)
        {
            var board = match.Board;
            _board.SyncBoardState(board);
        }
        public void Clear()
            => _board.UnHighlight();
        public void HighlightPiece(int pieceId)
        {
            _audio.PlaySfxOneShot(JanggiSfx.Select);
            _board.HighlightOnlyPiece(pieceId);
        }
        public void HighlightWays(IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals)
            => _board.HighlightWays(legals, illegals);
        public void UnHighlight()
        {
            _board.UnHighlightPiece();
            _board.UnHighlightWays();
        }
        public void ApplyMoveView(MoveRecord record)
        {
            _board.MovePiece(record.MovedPiece.Id, record.To);
            _audio.PlaySfxOneShot(JanggiSfx.Move);
            if (record.IsCapture)
            {
                _board.PlaceCapturedPiece(record.CapturedPiece.Id, record.CapturedPiece.Team);
                _audio.PlaySfxOneShot(JanggiSfx.Capture);
            }
        }
        public void RevertMoveView(MoveRecord record)
        {
            var movedPiece = record.MovedPiece;
            var to = record.From;
            _board.MovePiece(movedPiece.Id, to);

            if (record.IsCapture)
            {
                to = record.To;
                var captured = record.CapturedPiece;
                _board.RestoreCapturedPiece(captured.Id, captured.Team, to);
            }
        }
        public void OnGameEnded(in GameResultInfo info, bool loserIsLocal)
        {
            if (loserIsLocal) _audio.PlaySfxOneShot(JanggiSfx.Lose);
            else _audio.PlaySfxOneShot(JanggiSfx.Win);
            _resultUI.EndGame(info);
        }
        public void ShowReultUI()
            => _resultUI.Show();
        public void HideResultUI()
            => _resultUI.Hide();




        #region Live
  



        #endregion;
        public void BindUI(IMatchUIDatas match)
        {
            _matchUI.BindEvents(match);
        }
        public void UnBindUI(IMatchUIDatas match)
        {
            _matchUI.UnBindEvents(match);
        }
        public void UnDo(MoveContext ctx)
        {
            _board.UnHighlight();
            if (ctx.IsHandicap) return;

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
        public void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legalCells, IReadOnlyList<Pos> illegalCells)
        { 
            if (!pieceId.HasValue)
            {
                _board.UnHighlight();
                return;
            }
            _audio.PlaySfxOneShot(JanggiSfx.Select);
            _board.Highlight(pieceId.Value, legalCells, illegalCells);
        }
        public void OnTurnChanged(bool isLocal)
        {
            if (!isLocal) return;
            Debug.Log("턴 바뀜");
            _audio.PlaySfxOneShot(JanggiSfx.TurnAlert);
        }
        public void OnGameEnded(bool loserIsLocal, in GameResultInfo info)
        {
            if (loserIsLocal) _audio.PlaySfxOneShot(JanggiSfx.Lose); 
            else _audio.PlaySfxOneShot(JanggiSfx.Win);
            _resultUI.EndGame(info);
            ShowResultUI();
        }
        public void ShowResultUI()
            => _resultUI.Show();

        public void ResetGame(IBoardModel boardModel)
        {
            _resultUI.Hide();
            _board.SyncBoardState(boardModel);
        }
        public void StartGame(IBoardModel boardModel)
        {
            _board.StartGame(boardModel);
        }

        

        private readonly BoardPresenter _board;
        private readonly ResultUI       _resultUI;
        private readonly MatchUI        _matchUI;
        private readonly AudioManager   _audio;

    }
}
