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
            switch (result)
            {
                case BoardActionResult.SelectSuccess:
                    _audio.PlaySelect();
                    UpdateTurnInfo(_turnInfo.Player, TurnType.Attack);
                    break;

                case BoardActionResult.MoveSuccess:
                    
                    UpdateTurnInfo(NextPlayer(), TurnType.Select);
                    if (JangunCheck())
                        _audio.PlayJanggun();
                    else
                        _audio.PlayMove();
                    break;

                case BoardActionResult.CaptureSuccess:
                    _audio.PlayCapture();
                    UpdateTurnInfo(NextPlayer(), TurnType.Select);
                    if (JangunCheck())
                        _audio.PlayJanggun();
                    else
                        _audio.PlayCapture();
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
        private bool JangunCheck()
        {
            return _board.IsJanggun(_turnInfo.Player);
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


        // 이동관련 처리
        private List<IPiece> _garbageCho = new();
        private Pos _garbageChoPos = new Pos(0, -7);
        private List<IPiece> _garbageHan = new();
        private Pos _garbagehanPos = new Pos(0, -6);

        private void OnMoved(MoveContext context)
        {
            LogMove(context);
            SaveHistory(context);
            HandleCapture(context);
        }

        private void HandleCapture(in MoveContext context)
        {
            if (!context.IsCapture)
                return;

            var capturedView = context.CapturedPieceView;
            var team = context.Record.CapturedPiece.Team;

            var (garbageList, garbagePos) = GetGarbageData(team);

            garbageList.Add(capturedView);
            capturedView.MoveTo(garbagePos);

            AdvanceGarbagePos(team);
        }
        private void LogMove(in MoveContext context)
        {
            var record = context.Record;

            Debug.Log(
                $"From:({record.From.X},{record.From.Z}), " +
                $"To:({record.To.X},{record.To.Z}), " +
                $"Moved:{record.MovedPiece.Type}, " +
                $"Captured:{record.CapturedPiece.Type}"
            );
        }
        private void SaveHistory(in MoveContext context)
            => _history.Push(context);
        private (List<IPiece> garbageList, Pos garbagePos) GetGarbageData(PlayerTeam team)
        {
            if (team == PlayerTeam.Cho)
                return (_garbageCho, _garbageChoPos);

            return (_garbageHan, _garbagehanPos);
        }
        private void AdvanceGarbagePos(PlayerTeam team)
        {
            if (team == PlayerTeam.Cho)
                _garbageChoPos += Pos.Right;
            else
                _garbagehanPos += Pos.Right;
        }


        public void Undo()
        {
            _audio.PlayButton();
            if (_history.TryPop(out var context))
            {
                LogMove(context);
                _board.Undo(context);
                UpdateTurnInfo(NextPlayer(), TurnType.Select);
            }
            Debug.Log("Stack is empty");
        }
    }
}