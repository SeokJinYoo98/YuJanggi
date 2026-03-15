namespace Yujanggi.Core.Board
{
    using Domain;
    using System;

    public class CellData
    {
        public CellData(bool palace = false, PlayerTeam team=PlayerTeam.None, PieceType type=PieceType.None)
        {
            Palace = palace;
            Piece  = new PieceInfo(type, team);
        }
        public bool         Palace;
        public PieceInfo    Piece;
    }
    public readonly struct PieceInfo
    {
        public static PieceInfo None
            => new(PieceType.None, PlayerTeam.None);
        public static bool operator ==(PieceInfo a, PieceInfo b)
            => a.Type == b.Type && a.Team == b.Team;
        public static bool operator !=(PieceInfo a, PieceInfo b)
            => !(a == b);
        public override bool Equals(object obj)
            => obj is PieceInfo other && this == other;
        public override int GetHashCode()
            => HashCode.Combine(Type, Team);

        public readonly PlayerTeam Team;
        public readonly PieceType  Type;
        public bool IsNone => this == None;
        public PieceInfo(PieceType type, PlayerTeam team)
        {
            Type = type;
            Team = team;
        }
    }
}