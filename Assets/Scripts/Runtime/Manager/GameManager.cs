using UnityEngine;
using System.Collections.Generic;

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


        private Stack<MoveContext> _history = new();

        private void Awake()
        {
            Application.targetFrameRate = 144;
            _turnInfo = new();
        }
        private void Start()
        {
            _board.StartGame(BottomPlayer);
            _board.OnMove += OnMoved;
        }
        private void OnDestroy()
        {
            if (_board != null)
                _board.OnMove -= OnMoved;
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
        private void UpdateTurnInfo(PlayerTeam next, TurnType turn)
        {
            _turnInfo.Player = next;
            _turnInfo.Turn   = turn;
        }
        private PlayerTeam NextPlayer()
        {
            return _turnInfo.Player == PlayerTeam.Cho ? PlayerTeam.Han : PlayerTeam.Cho;
        }


        private List<IPiece> _garbageCho = new();
        private Pos _garbageChoPos = new Pos(0, -7);

        private List<IPiece> _garbageHan = new();
        private Pos _garbagehanPos = new Pos(0, -6);
        private void OnMoved(MoveContext context)
        {
            Debug.Log($"From:({context.From.X},{context.From.Z}), " +
                $"To:({context.To.X},{context.To.Z}), Attacker:{context.Attacker.Type}, " +
                $"Captured:{context.CapturedPiece.Type}");

            _history.Push(context);
 
            if (context.IsCapture)
            {
                var team = context.CapturedPiece.Team;
                List<IPiece> garbage;
                Pos pos;
                if (team == PlayerTeam.Cho)
                {
                    garbage = _garbageCho;
                    pos = _garbageChoPos;
                    _garbageChoPos += Pos.Right;
                }
                else
                {
                    garbage = _garbageHan;
                    pos = _garbagehanPos;
                    _garbagehanPos += Pos.Right;
                }
                var view = context.VictimView;
                garbage.Add(context.VictimView);
                view.MoveTo(pos);
            }
        }
    }
}