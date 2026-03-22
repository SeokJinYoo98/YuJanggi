
using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Piece
{
    public class PieceManager : MonoBehaviour
    {
        [SerializeField] private PieceSpawner _pieceSpawner;

        private Pos _garbageChoPos;
        private readonly Pos _choOrigin = new Pos(0, -2);
        private Pos _garbagehanPos;
        private readonly Pos _hanOrigin = new Pos(0, -3);
        
        

        private readonly Dictionary<int, Board.Piece> _views = new();
        public void SpawnPieces(IBoardModel boardModel, PlayerTeam bottom)
        {
            int width = boardModel.WIDTH;
            int height = boardModel.HEIGHT;

            for (int x = 0; x < width; ++ x)
            {
                for (int z = 0; z < height; ++z)
                {
                    var pos = new Pos(x, z);
                    if (!boardModel.HasPiece(pos))
                        continue;

                    var pieceInfo = boardModel.GetPiece(pos);
                    var piece = _pieceSpawner.SpawnPiece(pieceInfo, pos, bottom);
                    _views[pieceInfo.PieceId] = piece;
                }
            }
        }
    }
}