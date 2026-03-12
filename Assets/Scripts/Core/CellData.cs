using System;
namespace Yujanggi.Core.Board
{
    using Domain;

    public class CellData
    {
        public IPiece   Piece;
        public bool     Palace;
    }
    public class PalaceData : CellData
    {

    }
}