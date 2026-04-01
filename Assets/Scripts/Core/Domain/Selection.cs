using System.Collections.Generic;
using Yujanggi.Core.Board;

namespace Yujanggi.Core.Domain
{
    public readonly struct SelectionInfo
    {
        public SelectionInfo(PieceModel piece, Pos pos)
        {
            Piece = piece;
            Pos = pos;
        }

        public PieceModel Piece { get; }
        public Pos Pos { get; }
    }

    public class Selection
    {
        private readonly List<Pos> _illLegalCells = new(20);
        private readonly List<Pos> _legalCells    = new(20);

        public bool HasSelection => 0 < _legalCells.Count;
        public Pos FromPos;
        public Pos ToPos;
    }
    public class SelectionState
    {
        private readonly List<Pos> _movableCells = new(35);
        public SelectionState(PlayerTeam bottomPlayer)
            => BottomPlayer = bottomPlayer;

        public PlayerTeam           BottomPlayer { get; }
        public SelectionInfo?       Current { get; private set; }
        public IReadOnlyList<Pos>   MovableCells => _movableCells;
        public bool                 HasSelection => Current.HasValue;
        public bool                 IsBottom => Current.HasValue && Current.Value.Piece.Team == BottomPlayer;
        public PieceModel           SelectedPiece => Current!.Value.Piece;
        public PlayerTeam           Turn => Current!.Value.Piece.Team;
        public Pos                  SelectedPos => Current!.Value.Pos;

        public void Select(PieceModel piece, Pos pos)
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
        public static MoveContext Handicap => new(MoveRecord.None, false, false);
        public MoveContext(MoveRecord record, bool isJanggun, bool isEnd)
        {
            Record = record;
            MoveTeam = record.MovedPiece.Team;

            IsJanggun = isJanggun;
            EndGame = isEnd;
        }
        public MoveRecord   Record { get; }
        public bool         IsCapture => Record.IsCapture;
        public PlayerTeam   MoveTeam { get; }
        public bool         IsJanggun { get; }
        public bool         EndGame { get; }

        public bool IsHandicap
            => Record.Equals(MoveRecord.None);
    }
}