using System;
using UnityEngine;

namespace Yujanggi.Core.Board
{
    using Domain;
    using UnityEngine;

    public class CellData
    {
        public Position Position;
        public IPiece   CurrentPiece;
        public void PrintInfo()
        {
            Debug.Log($"{Position.X}, {Position.Z}: {CurrentPiece.Team}, {CurrentPiece.Type}");
        }
    }

}