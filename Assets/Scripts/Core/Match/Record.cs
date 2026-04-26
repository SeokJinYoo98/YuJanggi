using System;
using System.Collections.Generic;
using Yujanggi.Core.Domain;
namespace Yujanggi.Core.Match
{
    public class Record
    {
        public event Action<int, int> OnRecordChanged;
        public bool IsLive => _currIdx == MoveCount;
        public int MoveCount        => _records.Count;
        public int CurrentIndex     => _currIdx;
        private int _currIdx = 0;
        private readonly List<MoveContext> _records = new(100);
        public void StartGame()
        {
            _records.Clear();
            _currIdx = 0;

            OnRecordChanged?.Invoke(_currIdx, MoveCount);
        }

        public void Push(MoveContext context)
        {
            bool wasLive = IsLive;

            _records.Add(context);

            if (wasLive)
            {
                // 라이브 상태였으면 따라간다
                _currIdx = MoveCount;
            }

            OnRecordChanged?.Invoke(_currIdx, MoveCount);
        }

        public bool TryPrev(out MoveContext context)
        {
            if (_currIdx <= 0)
            {
                context = default;
                return false;
            }

            // 현재 상태에서 직전 수를 되돌릴 때 필요한 record
            context = _records[_currIdx - 1];

            _currIdx--;

            OnRecordChanged?.Invoke(_currIdx, MoveCount);
            return true;
        }
        public bool TryNext(out MoveContext context)
        {
            if (_currIdx >= MoveCount)
            {
                context = default;
                return false;
            }

            // 다음 상태로 진행할 때 적용할 record
            context = _records[_currIdx];

            _currIdx++;

            OnRecordChanged?.Invoke(_currIdx, MoveCount);
            return true;
        }

        public bool TryPop(out MoveContext context)
        {
            if (_records.Count == 0)
            {
                context = default;
                return false;
            }

            int lastIdx = _records.Count - 1;
            context = _records[lastIdx];
            _records.RemoveAt(lastIdx);

            if (_currIdx > MoveCount)
                _currIdx = MoveCount;

            OnRecordChanged?.Invoke(_currIdx, MoveCount);
            return true;
        }
        public bool TryPeek(out MoveContext context)
        {
            if (_records.Count == 0)
            {
                context = default;
                return false;
            }

            context = _records[^1];
            return true;
        }

        public void SeekToLatest()
        {
            _currIdx = MoveCount;
            OnRecordChanged?.Invoke(_currIdx, MoveCount);
        }

    }
}