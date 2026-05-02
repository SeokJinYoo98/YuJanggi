namespace Yujanggi.Runtime.GameSession
{
    using Core.Match;
    using System;
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using Yujanggi.Core.Domain;
    using Yujanggi.Runtime.Audio;
    using Yujanggi.Runtime.Board;


    public class ReplayView
    {
        
        public event Action OnReplayEntered;
        public event Action OnReplayExited;
        
        private Coroutine    _replayRoutine;

        private readonly ICoroutineRunner     _runner;
        private readonly IReplayBoardRenderer _board;
        private readonly Record               _record;
        private readonly AudioManager         _audio;
        private readonly TMP_Text             _displayMode;
        private int                           _currIdx = 0;

        private bool IsRecordAtLatest => _record.IsLive;
        private bool IsRecordsEmpty   => _record.Count == 0;
        public bool  IsLiveMode       => _currState == ReplayState.Live;
        public ReplayView(IReplayBoardRenderer board, Record record, ICoroutineRunner runner, AudioManager audio, TMP_Text displayMode)
        {
            _board       = board;
            _record      = record;
            _runner      = runner;
            _audio       = audio;
            _displayMode = displayMode;
        }
        public void ResetGame()
        {
            _currState = ReplayState.Live;
        }
     
        private enum ReplayState { Live, Forward, Backward };
        private ReplayState     _currState   = ReplayState.Live;
        private MoveContext?    _currCtx     = null;


        private const float _replayTimer = 0.5f;
        private void StopCoroutine()
        {
            if (_replayRoutine == null) return;
            _runner.Stop(_replayRoutine);
            _replayRoutine = null;
        }
        private void StartCoroutine(MoveContext ctx)
        {
            if (_replayRoutine != null) return;
            _replayRoutine = _runner.Run(ReplayRoutine(ctx));
        }
        private void UpdateState(ReplayState nextState, in MoveContext? nextCtx, int nextIdx)
        {
            _currState = nextState;
            _currCtx   = nextCtx;
            _currIdx   = nextIdx;
        }
        private void ClearPrevState(ReplayState nextState)
        {
            _board.UnHighlight();
            StopCoroutine();
            if (_currCtx.HasValue)
            {
                if (nextState == ReplayState.Forward)
                    DoMove(_currCtx.Value, false);
                else
                    UnDoMove(_currCtx.Value);
            }

        }
        private void PrepareVisual(in MoveRecord moveRecord)
        {
            var movedPiece = moveRecord.MovedPiece;
            _board.HighlightOnlyPiece(movedPiece.Id);
        }

        private void EnterState(ReplayState nextState, in MoveContext nextCtx, int nextIdx)
        {
            ClearPrevState(nextState);

            if (!nextCtx.IsHandicap)
            {
                PrepareVisual(nextCtx.Record);
                StartCoroutine(nextCtx);
            }

            UpdateState(nextState, nextCtx, nextIdx);
        }
        public void ReplayBackward()
        {
            if (IsRecordsEmpty)
            {
                return;
            }

            if (IsLiveMode)
            {
                EnterReplayView();
                return;
            }
            if (_currIdx == 0)
            {
                return;
            }

            var nextState = ReplayState.Backward;
            int nextIdx = _currIdx - 1;
            if (!_record.TryGetMoveCtx(nextIdx, out var nextCtx))
            {
                return;
            }

            EnterState(nextState, in nextCtx, nextIdx);
        }
        public void ReplayForward()
        {
            if (IsLiveMode)
            {
                return;
            }
            if (IsRecordAtLatest)
            {
                ExitReplayView();
                return;
            }
            var nextState = ReplayState.Forward;
            var nextIdx   = _currIdx + 1;
            if (!_record.TryGetMoveCtx(nextIdx, out var nextCtx))
            {
                return;
            }
                
  
            EnterState(nextState, in nextCtx, nextIdx);
        }

        private void EnterReplayView()
        {
            _record.EnterReplay();
            OnReplayEntered?.Invoke();
            _displayMode.SetText("기보 보기");
            
            var nextState = ReplayState.Backward;
            var nextIdx  = _record.Count - 1;
            if (!_record.TryGetMoveCtx(nextIdx, out var nextCtx)) return;

            EnterState(nextState, in nextCtx, nextIdx);
        }
        private void ExitReplayView()
        {
            _record.ExitReplay();
            ClearPrevState(ReplayState.Forward);
            UpdateState(ReplayState.Live, null, _record.Count - 1);
            OnReplayExited?.Invoke();

            _displayMode.SetText("라이브 보기");
        }
        private void UnDoMove(MoveContext moveCtx)
        {
            if (moveCtx.IsHandicap) return;
            var record = moveCtx.Record;
            var movedPiece   = record.MovedPiece;
            var movedToPos   = record.From;
            _board.MovePiece(movedPiece.Id, movedToPos);

            if (!record.IsCapture)
            {
                Debug.Log("캡처가 아니래");
                return;
            }
            var capturedId    = record.CapturedPiece.Id;
            var cpaturedTeam  = record.CapturedPiece.Team;
            var cpaturedToPos = record.To;
            _board.RestoreCapturedPiece(capturedId, cpaturedTeam, cpaturedToPos);
        }
        private void DoMove(MoveContext moveCtx, bool playAudio)
        {
            if (moveCtx.IsHandicap) return;

            if (playAudio)
                _audio.PlaySfxOneShot(JanggiSfx.Move);

            var record = moveCtx.Record;
            var movedPiece = record.MovedPiece;
            var movedToPos = record.To;
            _board.MovePiece(movedPiece.Id, movedToPos);

            if (!record.IsCapture)
            {
                Debug.Log("캡처가 아니래");
                return;
            }
            if (playAudio) 
                _audio.PlaySfxOneShot(JanggiSfx.Capture);
            var capturedId   = record.CapturedPiece.Id;
            var capturedTeam = record.CapturedPiece.Team;
            _board.PlaceCapturedPiece(capturedId, capturedTeam);
        }
        private IEnumerator ReplayRoutine(MoveContext ctx)
        {
            yield return new WaitForSeconds(_replayTimer);
            while (!ctx.IsHandicap)
            {
                DoMove(ctx, true);
                yield return new WaitForSeconds(_replayTimer);
                UnDoMove(ctx);
                yield return new WaitForSeconds(_replayTimer);
            }
        }
    }
}
