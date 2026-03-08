using System;
using UnityEngine;

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
    public interface IPiece
    {
        PlayerType Team { get; }
        PieceType  Type { get; }

        // public void Highlight();
        // public void OnMove(Vector3Int to);
    }
    public struct ClickCommand
    {
        PlayerType Type;
        
    }
    public interface IPlayer
    {
        PlayerType Type { get; }
        //event Action<ClickCommand> OnMouseClicked;
        //(int x, int z) MouseClicked()
        //{
        //    return (1, 1);
        //}
    }

    public interface IBoard
    {
        void OnClickCell(int x, int z, PlayerType type);
    }
   
}