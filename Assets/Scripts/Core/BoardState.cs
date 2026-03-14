using System.Collections.Generic;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Board
{
    public interface IBoardState
    {
        public PlayerType BottomPlayer { get; }
        public bool BoundaryCheck(int x, int z);
        public bool IsTherePiece(int x, int z, out PlayerType playerTeam, out PieceType pieceType);
        public void ClearMovable();
        public void AddMovable(List<(int x, int z)> ways);
        public bool IsMovable(int x, int z);
        public      IReadOnlyList<(int x, int z)> MovableCells { get; }
        public      IPiece GetPiece(int x, int z);
        public bool IsPalace(int x, int z);

    }
    public class BoardState : IBoardState
    {
        private readonly int         WIDTH  = 9;
        private readonly int         HEIGHT = 10;
        private CellData[,] _board;
 
        public BoardState(int width = 9, int height = 10, PlayerType bottom = PlayerType.Cho)
        {
            _bottom = bottom;
            
            WIDTH = width; HEIGHT = height;
            _board = new CellData[HEIGHT, WIDTH];

            for (int z = 0; z < HEIGHT; ++z)
            {
                for (int x = 0; x < WIDTH; ++x)
                {
                    _board[z, x] = new CellData();
                }
            }

            for (int x = 3; x <= 5; ++x)
            {
                for (int z = 0; z <= 2; ++z)
                {
                    _board[z, x].Palace = true;
                    _board[z + 7, x].Palace = true;
                }
            }
        }
        private PlayerType _bottom;
        public PlayerType   BottomPlayer => _bottom;
        public IPiece       GetPiece(int x, int z)
        {
            if (!BoundaryCheck(x, z)) return null;
            return _board[z, x].Piece;
        }
        public void MovePiece(int fromX, int fromZ, int toX, int toZ, out IPiece killed)
        {
            _board[fromZ, fromX].Piece?.MoveTo(toX, toZ);
            killed = _board[toZ, toX].Piece;
            _board[toZ, toX].Piece = _board[fromZ, fromX].Piece;
            RemovePiece(fromX, fromX);
        }
        public CellData     GetCell(int x, int z)
        {
            if (!BoundaryCheck(x, z)) return null;
            return _board[z, x];
        }
        public void         SetPiece(IPiece piece, int x, int z)
            => _board[z, x].Piece = piece;
        public void         RemovePiece(int x, int z)
            => _board[z, x].Piece = null;
        public bool         BoundaryCheck(int x, int z)
        {
            return 0 <= x && x < WIDTH && 0 <= z && z < HEIGHT;
        }

        public bool         IsTherePiece(int x, int z, out PlayerType playerTeam, out PieceType pieceType)
        {
            var p = GetPiece(x, z);
            if (p == null)
            {
                pieceType = PieceType.None;
                playerTeam = PlayerType.None;
                return false;
            }
            pieceType = p.Type;
            playerTeam = p.Team;
            return true;
        }


        private List<(int x, int z)> _movableCells = new(25);
        public void ClearMovable()
            => _movableCells.Clear();
        public void AddMovable(List<(int x, int z)> ways)
            => _movableCells.AddRange(ways);
        public bool IsMovable(int x, int z)
            => _movableCells.Contains((x, z));
        public IReadOnlyList<(int x, int z)> MovableCells => _movableCells;
        public bool IsPalace(int x, int z)
        {
            if (!BoundaryCheck(x, z)) 
                return false;
            
            return _board[z, x].Palace;
        }
           
    }
}
