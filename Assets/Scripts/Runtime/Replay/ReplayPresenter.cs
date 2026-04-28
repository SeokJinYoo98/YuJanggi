namespace Yujanggi.Runtime.Replay
{
    using Core.Match;
    using System;
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using Yujanggi.Core.Domain;
    using Yujanggi.Runtime.Audio;
    using Yujanggi.Runtime.Board;

    /*
        1. Enter -> 최신 기보 재생 (인덱스 움직일 필요 없음)
        2. Prev  -> 이전 기보 재생
        3. Next  -> 다음 기보 재생 (최신 기보 도달 및 횟수 체크)
        4. Exit  -> 리플레이 종료

            _currIdx  = 0
            CurrTurn  = 1
            TotalTurn = 1
    */
    public class ReplayPresenter
    {
        
        public event Action OnReplayEntered;
        public event Action OnReplayExited;

        private MoveContext? _currCtx   = null;
        private bool         _isForward = false;

        private Coroutine    _replayRoutine;

        private readonly ICoroutineRunner     _runner;
        private readonly IReplayBoardRenderer _board;
        private readonly Record               _record;
        private readonly AudioManager         _audio;
        private readonly TMP_Text             _displayMode;
        private int                           _currIdx = 0;
        private bool                          _isActivate = false;
        public ReplayPresenter(IReplayBoardRenderer board, Record record, ICoroutineRunner runner, AudioManager audio, TMP_Text displayMode)
        {
            _board       = board;
            _record      = record;
            _runner      = runner;
            _audio       = audio;
            _displayMode = displayMode;
        }
        #region Enter Exit
        public void Reset()
        {
            StopReplayAnim();
            ExitReplay();
        }
        private bool ExitReplay()
        {
            StopReplayAnim();
            _isActivate         = false;
            _currCtx            = null;
            _displayMode.text   = "실시간 보기";
            OnReplayExited?.Invoke();
            return false;
        }
        public bool  TryNextStep()
        {
            var nextIdx = _currIdx + 1;
            if (_record.Count <= nextIdx)
                return ExitReplay();

            if (!_record.TryGetMoveCtx(nextIdx, out var moveCtx)) 
                return false;
           
            PrepareReplay(in moveCtx, true);
            PlayReplayStep();

            _currIdx = nextIdx;
            return true;
        }
        public bool  TryPrevStep()
        {
            if (_record.Count == 0) 
                return false;

            if (!_isActivate) // 요기서 최근 인덱스 바로 재생
                return EnterReplay(false);

            var nextIdx = _currIdx - 1;
            if (!_record.TryGetMoveCtx(nextIdx, out var moveCtx))
                return false;

            PrepareReplay(in moveCtx, false);
            PlayReplayStep();

            _currIdx = nextIdx;
            return true;
        }
        private bool EnterReplay(bool isForward)
        {
            _displayMode.text = "기보 보기";
            _currIdx = _record.Count - 1;

            if (!_record.TryGetMoveCtx(_currIdx, out var ctx))
                return false;
            OnReplayEntered?.Invoke();

            _currCtx    = ctx;
            _isForward  = isForward;
            _isActivate = true;

            _board.HighlightOnlyPiece(ctx.Record.MovedPiece.Id);
            PlayReplayStep();
            return true;
        }


        #endregion
        #region Couroutine
        private IEnumerator ProcessReplayAnim()
        {
            while(true)
            {
                if (!_currCtx.HasValue) yield break;
                
                ApplyReplayStep(_isForward);
                yield return new WaitForSeconds(1f);
                ApplyReplayStep(!_isForward);
                yield return new WaitForSeconds(1f);
            }
        }
        private void        StopReplayAnim()
        {
            if (_replayRoutine == null) return;
            _runner.Stop(_replayRoutine);
            _replayRoutine = null;
        }

        #endregion
        #region Board
        private void PrepareReplay(in MoveContext moveCtx, bool isForward)
        {
            StopReplayAnim();
            ApplyReplayStep(_isForward);
            _currCtx   = moveCtx;
            _isForward = isForward;
     
            var moveRecord  = _currCtx.Value.Record;
            var id          = moveRecord.MovedPiece.Id;
            _board.HighlightOnlyPiece(id);
        }
        private void ApplyReplayStep(bool isForward)
        {
            var ctx = _currCtx.Value;
            if (ctx.IsHandicap) return;

            if (isForward) ApplyForwardStep(ctx.Record);
            else ApplyBackwardStep(ctx.Record);
        }
        private void PlayReplayStep()
        {
            StopReplayAnim();
            _replayRoutine = _runner.Run(ProcessReplayAnim());
        }


        private void ApplyForwardStep(in MoveRecord record)
        {
            // 이동된 말 이동
            _audio.PlaySfxOneShot(JanggiSfx.Move);
            var movedId = record.MovedPiece.Id;
            var movedTo = record.To;
            _board.MovePiece(movedId, movedTo);

            // 잡힌 말 처리
            if (!record.IsCapture) return;
            _audio.PlaySfxOneShot(JanggiSfx.Capture);
            var capturedId   = record.CapturedPiece.Id;
            var capturedTeam = record.CapturedPiece.Team;
            _board.PlaceCapturedPiece(capturedId, capturedTeam);
        }
        private void ApplyBackwardStep(in MoveRecord record)
        {
            var movedId = record.MovedPiece.Id;
            var movedTo = record.From;
            _board.MovePiece(movedId, movedTo);

            if (!record.IsCapture) return;
            var capturedId      = record.CapturedPiece.Id;
            var capturedTeam    = record.CapturedPiece.Team;
            var capturedTo      = record.To;
            _board.RestoreCapturedPiece(capturedId, capturedTeam, capturedTo);
        }
        #endregion
    }
}
