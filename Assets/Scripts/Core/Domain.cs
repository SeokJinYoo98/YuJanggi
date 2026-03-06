namespace Yujanggi.Core.Domain
{
    public struct Position
    {
        public int X;
        public int Z;
        public Position(int x, int z)
        {
            X = x; Z = z;
        }
    }
    public enum PlayerType
    {
        None,
        Cho,
        Han
    }
    public enum PieceType
    {
        Soldier
    }
}