

using System;
using System.Collections.Generic;
using Yujanggi.Core.Domain;
namespace Yujanggi.Core.Match
{
    public class Record
    {
        public event Action<(int, int)> OnRecordChanged; 
        public int MoveCount => _records.Count;

        private readonly List<MoveContext> _records = new(100);
        private int _currIdx = 0;
        public void Push(MoveContext context)
        {
            _records.Add(context);
            _currIdx = _records.Count;
            OnRecordChanged?.Invoke((MoveCount, MoveCount));
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
            _currIdx = _records.Count - 1;
            OnRecordChanged?.Invoke((MoveCount, MoveCount));
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
        public void StartGame()
        {
            _records.Clear();
            _currIdx = 0;
            OnRecordChanged?.Invoke((_records.Count, _records.Count));
        }


        public bool TryReplayNext(out MoveContext context)
        {
            if (_records.Count - 1 == _currIdx)
            {
                context = default;
                return false;
            }
            ++_currIdx;
            context = _records[_currIdx - 1];
            OnRecordChanged?.Invoke((_currIdx, _records.Count));
            return true;
        }
        public bool TryReplayPrev(out MoveContext context)
        {
            if (_currIdx == 0)
            {
                context = default;
                return false;
            }
            --_currIdx;
            context = _records[_currIdx - 1];
            OnRecordChanged?.Invoke((_currIdx, _records.Count));
            return true;
        }
    }
}