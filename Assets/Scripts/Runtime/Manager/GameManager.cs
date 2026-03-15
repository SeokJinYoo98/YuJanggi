
/*
  GameManager
     ├ TurnInfo
     ├ 클릭 처리
     ├ 말 선택
     ├ 말 이동
     ├ 죽은 말 관리
     └ 승패 판정

    BoardView (MonoBehaviour)
     ├ SpawnPieces
     ├ Highlight
     └ MovePiece Animation

    BoardState
        └ 말 위치 데이터

    JanggiRule
        └ 이동 규칙

 */

using UnityEngine;


namespace Yujanggi.Runtime.Manager
{
    using Board;
    using Core.Domain;
    using Core.Manager;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardController _board;
        private readonly PlayerType BottomPlayer = PlayerType.Cho;
        private GameTurnInfo _turnInfo;


        private void Awake()
        {
            _turnInfo = new();
        }
        private void Start()
        {
            _board.StartGame(BottomPlayer);
        }

        public void HandleClick(int x, int z, PlayerType team)
        {
            // if (team != _turnInfo.Player) return;
            
            bool isCompleted = false;
            var pos = new Pos(x, z);
                
            switch (_turnInfo.Turn)
            {
                case TurnType.Select:
                    isCompleted = _board.SelectPiece(pos, _turnInfo.Player);
                    if (isCompleted) 
                        UpdateTurnInfo(_turnInfo.Player, TurnType.Attack);
                    break;

                case TurnType.Attack:
                    isCompleted = _board.SelectWay(pos);
                    if (isCompleted)
                        UpdateTurnInfo(NextPlayer(), TurnType.Select);
                    else
                        UpdateTurnInfo(_turnInfo.Player, TurnType.Select);
                    break;
                default:
                    break;
            }
        }
        public void UpdateTurnInfo(PlayerType next, TurnType turn)
        {
            _turnInfo.Player = next;
            _turnInfo.Turn   = turn;
        }
        public PlayerType NextPlayer()
        {
            return _turnInfo.Player == PlayerType.Cho ? PlayerType.Han : PlayerType.Cho;
        }
    }
}