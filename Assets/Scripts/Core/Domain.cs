using System;
namespace Yujanggi.Core.Domain
{
    using System.Collections.Generic;
    using Yujanggi.Core.Board;
    using Yujanggi.Data.Board;
    using Yujanggi.Runtime.Board;

    public enum BoardActionResult
    {
        None,
        SelectSuccess,
        SelectFailed,
        MoveSuccess,
        MoveFailed,
        CaptureSuccess,
        Reselect
    }
    public enum             PlayerTeam
    { Cho, Han, None }
    public enum             PieceType
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
    public interface        IPlayerController
    {
        PlayerTeam Type { get; }
    }
    public enum             TurnType
    {
        Select,
        Attack,
        Update
    }
    public readonly struct Pos : IEquatable<Pos>
    {
        public Pos(int x, int z)
        {
            this.X = x;
            this.Z = z;
        }
        public readonly int X;
        public readonly int Z;

        public static Pos operator +(Pos a, Pos b)
            => new Pos(a.X + b.X, a.Z + b.Z);
        public static Pos operator -(Pos a, Pos b)
            => new Pos(a.X - b.X, a.Z - b.Z);
        public static bool operator ==(Pos a, Pos b)
            => a.X == b.X && a.Z == b.Z;
        public static bool operator !=(Pos a, Pos b)
            => !(a == b);
        public bool Equals(Pos other)
            => X == other.X && Z == other.Z;
        public override bool Equals(object obj)
            => obj is Pos other && Equals(other);
        public override int GetHashCode()
            => HashCode.Combine(X, Z);

        public static readonly Pos Up = new Pos(+0, +1);
        public static readonly Pos Down = new Pos(+0, -1);
        public static readonly Pos Left = new Pos(-1, +0);
        public static readonly Pos Right = new Pos(+1, +0);
        public static readonly Pos LeftUp = new Pos(-1, +1);
        public static readonly Pos RightUp = new Pos(+1, +1);
        public static readonly Pos LeftDown = new Pos(-1, -1);
        public static readonly Pos RightDown = new Pos(+1, -1);
        public static readonly Pos Invalid = new Pos(-100, -100);
    }
    public readonly struct SelectionInfo
    {
        public SelectionInfo(PieceInfo piece, Pos pos)
        {
            Piece = piece;
            Pos = pos;
        }

        public PieceInfo Piece  { get; }
        public Pos Pos          { get; }
    }
    public class SelectionState
    {
        private readonly List<Pos> _movableCells = new(25);
        public SelectionState(PlayerTeam bottomPlayer)
            => BottomPlayer = bottomPlayer;

        public PlayerTeam           BottomPlayer { get; }
        public SelectionInfo?       Current      { get; private set; }
        public IReadOnlyList<Pos>   MovableCells => _movableCells;
        public bool                 HasSelection => Current.HasValue;
        public bool                 IsBottom => Current.HasValue && Current.Value.Piece.Team == BottomPlayer;
        public PieceInfo            SelectedPiece => Current!.Value.Piece;
        public Pos                  SelectedPos   => Current!.Value.Pos;
        
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
    
    public readonly struct CaptureContext
    {
        public CaptureContext(Pos from, Pos to, PieceInfo attacker, PieceInfo victim, IPiece victimView)
        {
            From = from;
            To = to;
            Attacker = attacker;
            Victim = victim;
            VictimView = victimView;
        }

        public Pos          From        { get; }
        public Pos          To          { get; }
        public PieceInfo    Attacker    { get; }
        public PieceInfo    Victim      { get; }
        public IPiece       VictimView  { get; }
    }



}