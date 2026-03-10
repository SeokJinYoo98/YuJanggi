using NUnit.Framework;
using System;
namespace Yujanggi.Core.Domain
{
    using System.Collections.Generic;
    using UnityEngine.UIElements;
    using Yujanggi.Core.Board;

    public enum PlayerType
    {
        Cho,
        Han,
        None
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
    public interface IPiece
    {
        bool        IsOwner(PlayerType type);
        PieceType   Type { get; }
        PlayerType  Team { get; }
        public void Highlight();
        public void FindWays(IBoardState board, int x, int z);
        public void MoveTo(int x, int z);

    }
    public interface IPlayer
    {
        PlayerType Type { get; }
    }

    public interface IBoard
    {
        void HandleClick(int x, int z, PlayerType type);
    }
    public enum TurnType
    {
        Select,
        Attack,
        Update
    }
    public struct TurnInfo
    {
        public TurnType             State;
        public PlayerType           Turn;
        public IPiece               CurrentPiece;
        public int                  x, z;
    }
}