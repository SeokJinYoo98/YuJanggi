
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
        [SerializeField] private Board _board;

        private GameTurnInfo _turnInfo;


        private void Awake()
        {
            _turnInfo = new();
        }

        public void HandleClick(int x, int z, PlayerType team)
        {
            bool isCompleted = false;
            switch (_turnInfo.Turn)
            {
                case TurnType.Select:
                    isCompleted = _board.SelectPiece(x, z, _turnInfo.Player);
                    if (isCompleted) 
                        UpdateTurnInfo(_turnInfo.Player, TurnType.Attack);
                    break;

                case TurnType.Attack:
                    isCompleted = _board.SelectWay(x, z, _turnInfo.Player);
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