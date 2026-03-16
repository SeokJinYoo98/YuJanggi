

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
        private readonly List<Pos> _movableCells = new(25);
        public SelectionState(PlayerTeam bottomPlayer)
            => BottomPlayer = bottomPlayer;

        public PlayerTeam BottomPlayer { get; }
        public SelectionInfo? Current { get; private set; }
        public IReadOnlyList<Pos> MovableCells => _movableCells;
        public bool HasSelection => Current.HasValue;
        public bool IsBottom => Current.HasValue && Current.Value.Piece.Team == BottomPlayer;
        public PieceInfo SelectedPiece => Current!.Value.Piece;
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
        public void SetMovable(List<Pos> ways)
            => _movableCells.AddRange(ways);
        public bool IsMovable(Pos pos)
            => _movableCells.Contains(pos);
    }

    public readonly struct MoveContext
    {
        public MoveContext(Pos from, Pos to, PieceInfo attacker, PieceInfo captured, IPiece capturedPiece)
        {
            From = from;
            To = to;
            Attacker = attacker;
            CapturedPiece = captured;
            VictimView = capturedPiece;
        }
        public bool         IsCapture => CapturedPiece != PieceInfo.None;
        public Pos          From { get; }
        public Pos          To { get; }
        public PieceInfo    Attacker { get; }
        public PieceInfo    CapturedPiece { get; }
        public IPiece       VictimView { get; }
    }
}