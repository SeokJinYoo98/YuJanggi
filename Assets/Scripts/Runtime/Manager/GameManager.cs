using System.Collections.Generic;
using UnityEngine;

namespace Yujanggi.Runtime.Manager
{
    using Audio;
    using Board;
    using Core.Domain;
    using Core.Score;
    using Core.Turn;
    using Yujanggi.Core.Record;
    using Yujanggi.Runtime.UI;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardController _board;
        [SerializeField] private AudioManager    _audio;
        [SerializeField] private TurnUI          _turnUI;
        private readonly PlayerTeam BottomPlayer = PlayerTeam.Cho;

        private ScoreManager    _score;
        private TurnManager     _turn;
        private RecordManager   _recoder;
        private void Awake()
        {
            Application.targetFrameRate = 144;
            _turn       = new();
            _score      = new();
            _recoder    = new();
        }
        private void Start()
        {
            _board.StartGame(BottomPlayer);
            _board.OnMoved += OnMoved;
            _turn.StartTurn(PlayerTeam.Cho);
        }
        private void OnDestroy()
        {
            if (_board != null)
                _board.OnMoved -= OnMoved;
        }
        public void HandleClick(int x, int z)
        {
            var result = _board.HandleCellClick(new Pos(x, z), _turn.Current);
            switch (result)
            {
                case BoardActionResult.SelectSuccess:
                    _turn.SetTurn(TurnType.Attack);
                    _audio.PlaySelect();
                    break;

                case BoardActionResult.MoveSuccess:
                    _turn.NextTurn();
                    _audio.PlayMove();
                    break;

                case BoardActionResult.CaptureSuccess:
                    _turn.NextTurn();
                    _audio.PlayCapture();
                    break;

                case BoardActionResult.Reselect:
                    _turn.SetTurn(TurnType.Select);
                    _audio.PlaySelect();
                    break;

                case BoardActionResult.SelectFailed:
                case BoardActionResult.None:
                default:
                    break;
            }

        }

        private void OnMoved(MoveContext context)
        { 
            JangunCheck(context);
            SaveHistory(context);
            HandleCapture(context);

        
            if (context.EndGame)
                Application.Quit();
        }
        private void JangunCheck(in MoveContext context)
        {
            if (context.IsJanggun)
                _audio.PlayJanggun();
            
            if(_recoder.TryPeek(out var record) && record.IsCapture)
            {
                _audio.PlayMunggun();
            }
        }
        private void HandleCapture(in MoveContext context)
        {
            if (!context.IsCapture)
                return;
            _score.AddCaptureScore(context.Record.CapturedPiece);
        }
        private void SaveHistory(in MoveContext context)
        {
            _recoder.Push(context);
            _turnUI.UpdateMoveText(_recoder.MoveCount);
        }
        public void Undo()
        {
            _audio.PlayButton();
            if (_recoder.TryPop(out var context))
            {
                _board.Undo(context);
                _turn.NextTurn();
                
            }
            _turnUI.UpdateMoveText(_recoder.MoveCount);
            Debug.Log("Stack is empty");
        }



    }
}