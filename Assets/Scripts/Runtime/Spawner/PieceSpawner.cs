using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Data.Board;

namespace Yujanggi.Runtime.Board
{

    public class PieceSpawner : MonoBehaviour
    {
        [SerializeField] private PieceDataBase _pieceDB;
        [SerializeField] private Transform _cho;
        [SerializeField] private Transform _han;
        
        public IPiece Spawn(PieceInfo pieceInfo, Pos pos)
        {
            var team = pieceInfo.Team;
            var type = pieceInfo.Type;

            var parent = team == PlayerTeam.Cho ? _cho : _han;

            var data    = _pieceDB.GetData(team, type); 
            var prefab  = _pieceDB.GetPrefab(team);
            var piece = Instantiate(prefab, parent);
            piece.Init(data, pos);
   
            return piece;
        }
    }
}