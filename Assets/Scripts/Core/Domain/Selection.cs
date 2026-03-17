

using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Runtime.Board;

namespace Yujanggi.Core.Domain
{
    public readonly struct SelectionInfo
    {
        public SelectionInfo(PieceInfo piece, Pos pos)
        {
            Piece = piece;
            Pos = pos;
        }

        public PieceInfo Piece { get; }
        public Pos Pos { get; }
    }
    public class SelectionState
    {
        private readonly List<Pos> _movableCells = new(35);
        public SelectionState(PlayerTeam bottomPlayer)
            => BottomPlayer = bottomPlayer;

        public PlayerTeam BottomPlayer { get; }
        public SelectionInfo? Current { get; private set; }
        public IReadOnlyList<Pos> MovableCells => _movableCells;
        public bool HasSelection => Current.HasValue;
        public bool IsBottom => Current.HasValue && Current.Value.Piece.Team == BottomPlayer;
        public PieceInfo SelectedPiece => Current!.Value.Piece;
        public PlayerTeam Turn => Current!.Value.Piece.Team;
        public Pos SelectedPos => Current!.Value.Pos;

        public void Select(PieceInfo piece, Pos pos)
        {
            Current = new SelectionInfo(piece, pos);
            _movableCells.Clear();
        }
        public void Clear()
        {
            Current = null;
            _movableCells.Clear();
        }

        // Movable 관련
        public void AddMovable(Pos way)
            => _movableCells.Add(way);
        public void SetMovable(List<Pos> ways)
            => _movableCells.AddRange(ways);
        public bool IsMovable(Pos pos)
            => _movableCells.Contains(pos);
    }

    public readonly struct MoveContext
    {
        public MoveContext(MoveRecord record, IPiece capturedPiece)
        {
            Record = record;
            CapturedPieceView = capturedPiece;
        }
        public MoveRecord   Record { get; }
        public bool         IsCapture => Record.IsCapture;
        public IPiece       CapturedPieceView { get; }
    }
}