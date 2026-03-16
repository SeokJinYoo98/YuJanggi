

namespace Yujanggi.Core.Domain
{
    using Board;

    public readonly struct MoveRecord
    {
        public MoveRecord(Pos from, Pos to, PieceInfo moved, PieceInfo captured)
        {
            From = from; To = to;
            MovedPiece      = moved;
            CapturedPiece   = captured;
        }

        public Pos          From { get; }
        public Pos          To { get; }
        public PieceInfo    MovedPiece { get; }
        public PieceInfo    CapturedPiece { get; }

        public bool IsCapture => CapturedPiece != PieceInfo.None;
    }
}