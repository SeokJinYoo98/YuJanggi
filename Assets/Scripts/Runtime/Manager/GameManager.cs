
/*
  GameManager
     ├ TurnInfo
     ├ 클릭 처리
     ├ 말 선택
     ├ 말 이동
     ├ 죽은 말 관리
     └ 승패 판정

    Board (MonoBehaviour)
         ├ BoardState
         ├ PieceSpawner
         └ BoardHighlighter

    BoardState
        └ 말 위치 데이터

    JanggiRule
        └ 이동 규칙

 */

using UnityEngine;


namespace Yujanggi.Runtime.Manager
{
    using Board;
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Board _board;

    }
}