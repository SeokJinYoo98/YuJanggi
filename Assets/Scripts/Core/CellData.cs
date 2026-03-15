using System;
namespace Yujanggi.Core.Board
{
    using Domain;
    using Yujanggi.Utills.Board;

    public class CellData
    {
        public CellData(bool palace = false, PlayerType team=PlayerType.None, PieceType type=PieceType.None)
        {
            Palace = palace;
            Piece  = new PieceInfo(type, team);
        }
        public bool         Palace;
        public PieceInfo    Piece;
    }
    public struct PieceInfo
    {
        public PieceInfo(PieceType type, PlayerType team)
        {
            Type = type;
            Team = team;
        }
        public static PieceInfo None
            => new(PieceType.None, PlayerType.None);

        public PieceType  Type;
        public PlayerType Team;
    }
}