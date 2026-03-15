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


        private readonly PlayerTeam BottomPlayer = PlayerTeam.Cho;
        private GameTurnInfo _turnInfo;


        private void Awake()
        {
            _turnInfo = new();
        }
        private void Start()
        {
            _board.StartGame(BottomPlayer);
        }

        public void HandleClick(int x, int z)
        {
            var result = _board.HandleCellClick(new Pos(x, z), _turnInfo.Player);
            Debug.Log($"{result}");
            switch (result)
            {
                case BoardActionResult.SelectSuccess:
                    _audio.PlaySelect();
                    UpdateTurnInfo(_turnInfo.Player, TurnType.Attack);
                    break;

                case BoardActionResult.MoveSuccess:
                    _audio.PlayMove();
                    UpdateTurnInfo(NextPlayer(), TurnType.Select);
                    break;

                case BoardActionResult.CaptureSuccess:
                    _audio.PlayCapture();
                    UpdateTurnInfo(NextPlayer(), TurnType.Select);
                    break;

                case BoardActionResult.Reselect:
                    _audio.PlaySelect();
                    break;

                case BoardActionResult.SelectFailed:
                case BoardActionResult.None:
                default:
                    break;
            }
            
        }
        public void UpdateTurnInfo(PlayerTeam next, TurnType turn)
        {
            _turnInfo.Player = next;
            _turnInfo.Turn   = turn;
        }
        public PlayerTeam NextPlayer()
        {
            return _turnInfo.Player == PlayerTeam.Cho ? PlayerTeam.Han : PlayerTeam.Cho;
        }
    }
}