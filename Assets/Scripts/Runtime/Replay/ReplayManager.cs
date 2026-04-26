namespace Yujanggi.Runtime.Replay
{
    using Core.Match;
    using System;

    // 너의 역할은 스냅샷을 만든다 그리고 조회를 한다
    public class ReplayManager
    {
        public event Action OnReplayEntered;
        public event Action OnReplayExited;


        private bool _isLive = true;
        public ReplayManager(Record record)
        {
            _record = record;
        }
        public void ShowNext()
        {

        }
        public void ShowPrev()
        {
            
        }
        private readonly Record     _record;
    }
}
