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
        Cho,
        Han
    }
    public enum PieceType
    {
        King,       // 궁
        Chariot,    // 차
        Cannon,     // 포
        Horse,      // 마
        Elephant,   // 상
        Guard,      // 사
        Soldier,    // 졸/병
        None
    }
}