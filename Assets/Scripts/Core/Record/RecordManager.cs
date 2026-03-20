

using System;
using System.Collections.Generic;
using Yujanggi.Core.Domain;
namespace Yujanggi.Core.Record
{
    public class RecordManager
    {
        public event Action<(int, int)> OnRecordChanged; 
        private readonly Stack<MoveContext> _records = new();
        private int MoveCount => _records.Count;

        private Pos _garbageChoPos = new (0, -5);
        private Pos _garbagehanPos = new (0, -4);
        public void Push(MoveContext record)
        {
            _records.Push(record);
            UpdateGarbagePos(record, false);

            OnRecordChanged?.Invoke((MoveCount, MoveCount));
        }

        public bool TryPop(out MoveContext record)
        {
            if (_records.Count == 0)
            {
                record = default;
                return false;
            }
            record = _records.Pop();
            UpdateGarbagePos(record, true);

            OnRecordChanged?.Invoke((MoveCount, MoveCount));
            return true;
        }

        public bool TryPeek(out MoveContext record)
        {
            if (_records.Count == 0)
            {
                record = default;
                return false;
            }
            record = _records.Peek();
            return true;
        }

        private void UpdateGarbagePos(in MoveContext record, bool isPop)
        {
            if (!record.IsCapture) return;

            var team = record.Record.CapturedPiece.Team;
            if (isPop)
            {
                if (team == PlayerTeam.Cho)
                    _garbageChoPos -= Pos.Right;
                else
                    _garbagehanPos -= Pos.Right;
            }
            else
            {
                if (team == PlayerTeam.Cho)
                {
                    record.CapturedPieceView.MoveTo(_garbageChoPos);
                    _garbageChoPos += Pos.Right;
                }  
                else
                {
                    record.CapturedPieceView.MoveTo(_garbagehanPos);
                    _garbagehanPos += Pos.Right;
                }
            }
        }
    }
}