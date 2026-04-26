namespace Yujanggi.Runtime.Replay
{
    using Core.Match;
    using System;
    using Yujanggi.Core.Domain;

    // 너의 역할은 스냅샷을 만든다 그리고 조회를 한다
    public class ReplayManager
    {
        public event Action OnReplayEntered;
        public event Action OnReplayExited;
        public bool IsRunning => _isRunning;

        private bool _isRunning = false;
        public ReplayManager(Record record)
        {
            _record = record;
        }
        
        public bool TryGetNextMoveCtx(out MoveContext moveCtx)
        {
            moveCtx = default;
            if(!_record.IsLive)
            {
                ExitReplay();
            }
            return true;
        }
        public bool TryGetPrevMoveCtx(out MoveContext moveCtx)
        {
            moveCtx = default;
            if (_record.IsLive)
            {
                EnterReplay();
            }
            return true;
        }
        private void EnterReplay()
        {
            OnReplayEntered?.Invoke();
        }
        private void ExitReplay()
        {
            OnReplayExited?.Invoke();
        }
        private readonly Record     _record;
    }
}
