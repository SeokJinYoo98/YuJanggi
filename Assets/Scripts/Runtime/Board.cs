using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;
    using Yujanggi.Data.Board;


    public class Board : MonoBehaviour, IBoard
    {
        [SerializeField] private PieceSpawner   _pieceSpawner;
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
            _pieceSpawner.SpawnPieces(_state);
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
