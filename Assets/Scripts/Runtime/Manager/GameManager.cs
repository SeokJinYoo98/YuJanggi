using UnityEngine;

namespace Yujanggi.Runtime.Manager
{
    using Board;
    using Core.Domain;
    using Core.Manager;
    using Audio;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardController _board;
        [SerializeField] private AudioManager    _audio;


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
                    {
                        _audio.PlaySelect();
                        UpdateTurnInfo(_turnInfo.Player, TurnType.Attack);
                    }
                        
                    break;

                case TurnType.Attack:
                    isCompleted = _board.SelectWay(pos);
                    if (isCompleted)
                    {
                        _audio.PlayMove();
                        UpdateTurnInfo(NextPlayer(), TurnType.Select);
                    }
                        
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