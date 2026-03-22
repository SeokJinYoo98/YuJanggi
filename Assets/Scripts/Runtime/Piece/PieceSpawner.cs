using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Data.Board;

namespace Yujanggi.Runtime.Piece
{
    public class PieceSpawner : MonoBehaviour
    {
        [SerializeField] private PieceDataBase _pieceDB;
        [SerializeField] private Transform _cho;
        [SerializeField] private Transform _han;
        
        public IPiece Spawn(PieceModel pieceInfo, Pos pos, PlayerTeam bottom)
        {
            var team = pieceInfo.Team;
            var type = pieceInfo.Type;

            var parent = team == PlayerTeam.Cho ? _cho : _han;

            var data    = _pieceDB.GetData(team, type); 
            var prefab  = _pieceDB.GetPrefab(team);
            var piece = Instantiate(prefab, parent);
            piece.Init(data, pos, bottom);
            
            
            return piece;
        }

        public Piece SpawnPiece(PieceModel pieceInfo, Pos pos, PlayerTeam bottom)
        {
            var team = pieceInfo.Team;
            var type = pieceInfo.Type;

            var parent = team == PlayerTeam.Cho ? _cho : _han;

            var data   = _pieceDB.GetData(team, type);
            var prefab = _pieceDB.GetPrefab(team);
            var piece  = Instantiate(prefab, parent);
            piece.Init(data, pos, bottom); ;

            return piece;
        }
    }
}