

using System;
using System.Collections.Generic;
using Yujanggi.Core.Domain;
namespace Yujanggi.Core.Record
{
    public class RecordManager
    {
        public event Action<(int, int)> OnRecordChanged; 
        private readonly Stack<MoveContext> _records = new();
        public int MoveCount => _records.Count;

        private readonly Pos _choOrigin = new Pos(0, -2);
        private readonly Pos _hanOrigin = new Pos(0, -3);
        private Pos _garbageChoPos;
        private Pos _garbagehanPos;

        public void StartGame()
        {
            _records.Clear();
            _garbageChoPos = _choOrigin;
            _garbagehanPos = _hanOrigin;
            OnRecordChanged?.Invoke((MoveCount, MoveCount));
        }
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
        public void ResetRecord()
        {

        }
    }
}