namespace Yujanggi.Core.Replay
{
    using Match;

    // 너의 역할은 스냅샷을 만든다 그리고 조회를 한다
    public class ReplayManager
    {
        private int currIdx = 0;
        public ReplayManager(Record record)
        {
            _record = record;
        }
        private readonly Record _record;
    }
}
