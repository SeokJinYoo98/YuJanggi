using UnityEngine;

namespace Yujanggi.Runtime.Manager
{
    using Audio;
    using Board;
    using Core.Domain;
    using Core.Score;
    using Core.Turn;
    using System;
    using Yujanggi.Core.Record;
    using Yujanggi.Runtime.UI;
    using Yujanggi.Utills.Board;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ResultUI        _resultUI;
        [SerializeField] private MatchUI         _matchUI;
        [SerializeField] private BoardController _board;
        [SerializeField] private AudioManager    _audio;
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

            _turn.OnTurnChanged      += _matchUI.UpdateTurn;
            _recoder.OnRecordChanged += _matchUI.UpdateRecord;
            _score.OnScoreChanged    += _matchUI.UpdateScore;
        }
        private void OnDestroy()
        {
            if (_board != null)
                _board.OnMoved -= OnMoved;

            if (_matchUI != null && _turn != null)
            {
                _turn.OnTurnChanged      -= _matchUI.UpdateTurn;
                _recoder.OnRecordChanged -= _matchUI.UpdateRecord;
                _score.OnScoreChanged    -= _matchUI.UpdateScore;
            }
        }
        public void HandleClick(int x, int z)
        {
            if (_turn.IsEnd)
                return;

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
            if (context.IsCapture)
            {
                HandleCapture(context);
                CheckMate(context);
            }

        }

        private void JangunCheck(in MoveContext context)
        {
            if (context.IsJanggun)
            {
                _audio.PlayJanggun();
                _matchUI.PlayJanggun(context.MoveTeam);
            }
                
            
            if(_recoder.TryPeek(out var record) && record.IsJanggun)
            {
                _audio.PlayMunggun();
            }
        }
        private void HandleCapture(in MoveContext context)
        {
            var team = context.Record.CapturedPiece.Team;
            var type = context.Record.CapturedPiece.Type;
            var value = BoardHelper.GetPieceScore(type);

            _score.SetScore(team, value);
        }
        private void SaveHistory(in MoveContext context)
        {
            _recoder.Push(context);
        }
        public void Undo()
        {
            if (!_recoder.TryPop(out var context))
                return;

            var team = context.Record.CapturedPiece.Team;
            var type = context.Record.CapturedPiece.Type;
            var value = BoardHelper.GetPieceScore(type);

            _score.SetScore(team, -value);

            _board.Undo(context);
            _turn.NextTurn();
        }
        public void CheckMate(in MoveContext context)
        {
            if (!context.EndGame) return;
            _turn.SetTurn(TurnType.End);

            GameResultInfo info;
            info.MoveCnt = _recoder.MoveCount;
            info.Type    = GameResult.CheckMate;
            info.Winner  = _turn.Current;

            _resultUI.Show();
            _resultUI.EndGame(info);
        }
    }
}