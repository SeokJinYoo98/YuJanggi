namespace Yujanggi.Runtime.Replay
{
    using Core.Match;
    using System;
    using System.Collections;
    using UnityEngine;
    using Yujanggi.Core.Domain;
    using Yujanggi.Runtime.Audio;
    using Yujanggi.Runtime.Board;
  
    public class ReplayPresenter
    {
        public event Action OnReplayEntered;
        public event Action OnReplayExited;

        private readonly IReplayBoardRenderer _board;
        private readonly ICoroutineRunner     _runner; 
        private readonly Record               _record;
        private readonly AudioManager         _audio;
        private MoveContext?                  _currCtx = null;
       
        public ReplayPresenter(IReplayBoardRenderer board, Record record, ICoroutineRunner runner, AudioManager audio)
        {
            _board  = board;
            _record = record;
            _runner = runner;
            _audio  = audio;
        }

        public bool TryNext()
        {
            if (_record.IsLive)
                return false;

            if (!_record.TryNext(out var moveCtx))
                return false;

            _currCtx = moveCtx;
            RebuildBoard(false);

            if (_record.IsLive)
                ExitReplay();

            return true;
        }
        public bool TryPrev()
        {
            if (_record.IsLive) EnterReplay();
                
            if (!_record.TryPrev(out var moveCtx)) return false;
            _currCtx = moveCtx;
            RebuildBoard(true);
 
            return true;
        }
        private void EnterReplay()
        {
            OnReplayEntered?.Invoke();
        }
        private void ExitReplay()
        {
            _currCtx = null;
            OnReplayExited?.Invoke();
        }
        private IEnumerator ProcessReplayAnim()
        {

            yield return new WaitForSeconds(1f);
            RebuildBoard(true);
            yield break;
        }
        // 보드를 이전 상태로 세팅한다.
        private void RebuildBoard(bool isPrev)
        {

            if (!_currCtx.HasValue) return;
            
            var ctx = _currCtx.Value;
            if (ctx.IsHandicap) return;
            var record = ctx.Record;
            Pos to = isPrev ? record.From : record.To;

            var movedId = record.MovedPiece.Id;
            _board.MovePiece(movedId, to);
            if (!record.IsCapture) return;

            to = isPrev ? record.To : record.From;
            var capturedId   = record.CapturedPiece.Id;
            var capturedTeam = record.CapturedPiece.Team;
            _board.RestoreCapturedPiece(capturedId, capturedTeam, to);
        }
        // 움직인다.
        // 만약 버튼이 또 눌리면?


        // 되돌린다.
    }
}
