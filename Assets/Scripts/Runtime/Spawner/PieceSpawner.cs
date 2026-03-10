using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Data.Board;
using Yujanggi.Core.Board;
namespace Yujanggi.Runtime.Board
{

    public class PieceSpawner : MonoBehaviour
    {
        [SerializeField] private PieceDataBase _pieceDB;
        [SerializeField] private Transform _cho;
        [SerializeField] private Transform _han;
        
        private BoardState _state;

        public void  SpawnPieces(BoardState state)
        {
            if (_state == null) _state = state;
            SpawnCho();
            SpawnHan();
        }
        private void SpawnCho()
        {
            Spawn(PlayerType.Cho, PieceType.Chariot, 0, 0);
            Spawn(PlayerType.Cho, PieceType.Chariot, 8, 0);
            Spawn(PlayerType.Cho, PieceType.Elephant, 1, 0);
            Spawn(PlayerType.Cho, PieceType.Elephant, 7, 0);
            Spawn(PlayerType.Cho, PieceType.Horse, 6, 0);
            Spawn(PlayerType.Cho, PieceType.Horse, 2, 0);
            Spawn(PlayerType.Cho, PieceType.Guard, 3, 0);
            Spawn(PlayerType.Cho, PieceType.Guard, 5, 0);
            Spawn(PlayerType.Cho, PieceType.King, 4, 1);
            Spawn(PlayerType.Cho, PieceType.Cannon, 1, 2);
            Spawn(PlayerType.Cho, PieceType.Cannon, 7, 2);
            Spawn(PlayerType.Cho, PieceType.Soldier, 0, 3);
            Spawn(PlayerType.Cho, PieceType.Soldier, 2, 3);
            Spawn(PlayerType.Cho, PieceType.Soldier, 4, 3);
            Spawn(PlayerType.Cho, PieceType.Soldier, 6, 3);
            Spawn(PlayerType.Cho, PieceType.Soldier, 8, 3);
        }
        private void SpawnHan()
        {
            Spawn(PlayerType.Han, PieceType.Chariot, 0, 9);
            Spawn(PlayerType.Han, PieceType.Chariot, 8, 9);
            Spawn(PlayerType.Han, PieceType.Elephant, 1, 9);
            Spawn(PlayerType.Han, PieceType.Elephant, 7, 9);
            Spawn(PlayerType.Han, PieceType.Horse, 6, 9);
            Spawn(PlayerType.Han, PieceType.Horse, 2, 9);
            Spawn(PlayerType.Han, PieceType.Guard, 3, 9);
            Spawn(PlayerType.Han, PieceType.Guard, 5, 9);
            Spawn(PlayerType.Han, PieceType.King, 4, 8);
            Spawn(PlayerType.Han, PieceType.Cannon, 1, 7);
            Spawn(PlayerType.Han, PieceType.Cannon, 7, 7);
            Spawn(PlayerType.Han, PieceType.Soldier, 0, 6);
            Spawn(PlayerType.Han, PieceType.Soldier, 2, 6);
            Spawn(PlayerType.Han, PieceType.Soldier, 4, 6);
            Spawn(PlayerType.Han, PieceType.Soldier, 6, 6);
            Spawn(PlayerType.Han, PieceType.Soldier, 8, 6);
        }
        private void Spawn(PlayerType playerType, PieceType pieceType, int x, int z)
        {
            var parent = playerType == PlayerType.Cho ? _cho : _han;
            var prefab = _pieceDB.GetPrefab(playerType);
            var piece = Instantiate(prefab, parent);
            var data = _pieceDB.GetData(playerType, pieceType);
            piece.Init(data, x, z);
            _state.GetCell(x, z).Piece = piece;
        }
    }
}