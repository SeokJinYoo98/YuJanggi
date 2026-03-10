using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using NUnit.Framework;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;
    using Yujanggi.Data.Board;


    public class Board : MonoBehaviour, IBoard
    {
        [SerializeField] private PieceDataBase  _pieceDB;
        [SerializeField] private Transform      _cho;
        [SerializeField] private Transform      _han;

        private BoardState  _state;
        private TurnInfo    _turnInfo;
        private JanggiRule  _rule;
        private void Awake()
        {
            _rule     = new();
            _state    = new();
            _turnInfo = new();
            _turnInfo.State         = TurnType.Select;
            _turnInfo.Turn          = PlayerType.Cho;
            _turnInfo.CurrentPiece  = null;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SpawnPieces();
        }

        // Spawn
        public  void SpawnPieces()
        {
            SpawnCho();
            SpawnHan();
        }
        private void SpawnCho()
        {
            Spawn(PlayerType.Cho, PieceType.Chariot,  0, 0);
            Spawn(PlayerType.Cho, PieceType.Chariot,  8, 0);
            Spawn(PlayerType.Cho, PieceType.Elephant, 1, 0);
            Spawn(PlayerType.Cho, PieceType.Elephant, 7, 0);
            Spawn(PlayerType.Cho, PieceType.Horse,    6, 0);
            Spawn(PlayerType.Cho, PieceType.Horse,    2, 0);
            Spawn(PlayerType.Cho, PieceType.Guard,    3, 0);
            Spawn(PlayerType.Cho, PieceType.Guard,    5, 0);
            Spawn(PlayerType.Cho, PieceType.King,     4, 1);
            Spawn(PlayerType.Cho, PieceType.Cannon,   1, 2);
            Spawn(PlayerType.Cho, PieceType.Cannon,   7, 2);
            Spawn(PlayerType.Cho, PieceType.Soldier,  0, 3);
            Spawn(PlayerType.Cho, PieceType.Soldier,  2, 3);
            Spawn(PlayerType.Cho, PieceType.Soldier,  4, 3);
            Spawn(PlayerType.Cho, PieceType.Soldier,  6, 3);
            Spawn(PlayerType.Cho, PieceType.Soldier,  8, 3);
        }
        private void SpawnHan()
        {
            Spawn(PlayerType.Han, PieceType.Chariot,  0, 9);
            Spawn(PlayerType.Han, PieceType.Chariot,  8, 9);
            Spawn(PlayerType.Han, PieceType.Elephant, 1, 9);
            Spawn(PlayerType.Han, PieceType.Elephant, 7, 9);
            Spawn(PlayerType.Han, PieceType.Horse,    6, 9);
            Spawn(PlayerType.Han, PieceType.Horse,    2, 9);
            Spawn(PlayerType.Han, PieceType.Guard,    3, 9);
            Spawn(PlayerType.Han, PieceType.Guard,    5, 9);
            Spawn(PlayerType.Han, PieceType.King,     4, 8);
            Spawn(PlayerType.Han, PieceType.Cannon,   1, 7);
            Spawn(PlayerType.Han, PieceType.Cannon,   7, 7);
            Spawn(PlayerType.Han, PieceType.Soldier,  0, 6);
            Spawn(PlayerType.Han, PieceType.Soldier,  2, 6);
            Spawn(PlayerType.Han, PieceType.Soldier,  4, 6);
            Spawn(PlayerType.Han, PieceType.Soldier,  6, 6);
            Spawn(PlayerType.Han, PieceType.Soldier,  8, 6);
        }
        private void Spawn(PlayerType playerType, PieceType pieceType, int x, int z)
        {
            var parent = playerType == PlayerType.Cho ? _cho : _han;
            var prefab = _pieceDB.GetPrefab(playerType);
            var piece  = Instantiate(prefab, parent);
            var data   = _pieceDB.GetData(playerType, pieceType);
            piece.Init(data, x, z);
            this._state[x, z].CurrentPiece = piece;
        }

        public void HandleClick(int x, int z, PlayerType type)
        {
            if (_turnInfo.State == TurnType.Update) 
                return;

            if (_turnInfo.State == TurnType.Select)
                Select(x, z, type);
   
            else if (_turnInfo.State == TurnType.Attack)
                Attack(x, z, type);
        }

        private void Select(int x, int z, PlayerType type)
        { 
            // State로 옮기고
            var piece = _state.GetPiece(x, z);
            if (piece == null) return;
            if (!piece.IsOwner(_turnInfo.Turn)) return;
            _state.HightCell(x, z);

            _turnInfo.CurrentPiece = piece;
            _turnInfo.x = x; _turnInfo.z = z;
            _turnInfo.State = TurnType.Attack;
            
            _turnInfo.CurrentPiece.FindWays(_state, x, z);
        }

        private void Attack(int x, int z, PlayerType type)
        {
            if (_state.IsValidMovement(x, z))
            {
                _state.MoveTo(_turnInfo.x, _turnInfo.z, x, z);
                _turnInfo.State = TurnType.Update;
                _turnInfo.Turn  = PlayerType.Han;
                _turnInfo.CurrentPiece  = null;
            }
            // 리스트에 있나 본다?
            // 없으면 -> 언셀렉트
            // 있으면 -> 무브
            
            UnSelect(x, z, type);
        }

        private void UnSelect(int x, int z, PlayerType type)
        {
            _state.HightCell(_turnInfo.x, _turnInfo.z);
            _turnInfo.CurrentPiece  = null;
            _turnInfo.State         = TurnType.Select;
        }
        private void BoardUpdate()
        {

        }

        private void MovePiece()
        {
            
        }

        // 피스 관련 설정은 모두 스테이트로 옮기자.
    }
}
