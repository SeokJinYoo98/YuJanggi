

using System;
using System.Collections.Generic;
using Yujanggi.Core.Domain;
namespace Yujanggi.Core.Match
{
    public class Record
    {
        public event Action<int, int> OnRecordChanged; 
        public int MoveCount => _records.Count;
        private int _currView = 0;
        private readonly List<MoveContext> _records = new(100);
        public void Push(MoveContext context)
        {
            _records.Add(context);

            OnRecordChanged?.Invoke(MoveCount, MoveCount);
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

            OnRecordChanged?.Invoke(MoveCount, MoveCount);
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
            OnRecordChanged?.Invoke(MoveCount, MoveCount);
        }
    }
}