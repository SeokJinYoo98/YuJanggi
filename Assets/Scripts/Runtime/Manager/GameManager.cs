using UnityEngine;

namespace Yujanggi.Runtime.Manager
{
    using Audio;
    using Board;
    using Core.Domain;
    using System.Collections.Generic;
    using Yujanggi.Core.Match;
    using Yujanggi.Runtime.Player.Yujanggi.Runtime.Controller;
    using Yujanggi.Runtime.UI;
    using Yujanggi.Utills.Board;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ResultUI        _resultUI;
        [SerializeField] private MatchUI         _matchUI;
        [SerializeField] private AudioManager    _audio;

        [SerializeField] private JanggiController _player;

        [SerializeField] private BoardPresenter _board;

        private MatchManager _match;

        //private bool _replay = false;

        private void Awake()
        {
            Application.targetFrameRate = 144;

        }
        private void Start()
        {
            StartGame();

        }
        private void OnDestroy()
        {
            UnBindEvents();
        }
        private void Update()
        {
            _match.Update(Time.deltaTime);
        }

        private void StartGame()
        {
            var bottom = PlayerTeam.Cho;
            var maxTime = 30f;

            _match          = new(bottom, maxTime);

            BindEvents();
            _board.StartGame(bottom, _match.Board);
        }

        private void    BindEvents()
        {   
            var turn = _match.Turn;
            turn.OnTurnEnd             += _match.Handicap;
            turn.OnTurnChanged         += _matchUI.UpdateTurn;
            turn.OnTimeChanged         += _matchUI.UpdateTimer;

            var record = _match.Record;
            record.OnRecordChanged     += _matchUI.UpdateRecord;

            var score = _match.Score;
            score.OnScoreChanged       += _matchUI.UpdateScore;

            var matchEvents = _match.MatchEvent;
            matchEvents.OnSelectionChanged  += HandleSelectionChanged;
            matchEvents.OnPieceMoved        += HandlePieceMove;
            matchEvents.OnCheck             += HandleCheck;
            matchEvents.OnMunggun           += _audio.PlayMunggun;

            _player.OnBoardClicked     += HandleClick;

        }
        private void    UnBindEvents()
        {
        
            if (_matchUI != null && _match != null)
            {
                var turn = _match.Turn;
                turn.OnTurnEnd             -= _match.Handicap;
                turn.OnTurnChanged         -= _matchUI.UpdateTurn;
                turn.OnTimeChanged         -= _matchUI.UpdateTimer;

                var record = _match.Record;
                record.OnRecordChanged     -= _matchUI.UpdateRecord;

                var score = _match.Score;
                score.OnScoreChanged       -= _matchUI.UpdateScore;

                var matchEvents = _match.MatchEvent;
                matchEvents.OnSelectionChanged  -= HandleSelectionChanged;
                matchEvents.OnPieceMoved        -= HandlePieceMove;
                matchEvents.OnCheck             -= HandleCheck;
                matchEvents.OnMunggun           -= _audio.PlayMunggun;

            }
            if (_player != null)
                _player.OnBoardClicked -= HandleClick;
            
            
        }
        public void     HandleClick(Pos pos)
        {
            Debug.Log($"{pos.X}, {pos.Z}");

            var result = _match.HandleCellClick(pos);
            switch (result)
            {
                case BoardActionResult.SelectSuccess:
                case BoardActionResult.Reselect:
                    _audio.PlaySelect();
                    break;

                case BoardActionResult.MoveSuccess:
                    _audio.PlayMove();
                    break;

                case BoardActionResult.CaptureSuccess:
                    _audio.PlayCapture();
                    break;

                case BoardActionResult.SelectFailed:
                case BoardActionResult.None:
                default:
                    break;
            }
        }
        
        private void    JangunCheck(in MoveContext context)
        {
            //if (context.IsJanggun)
            //{
            //    _audio.PlayJanggun();
            //    _matchUI.PlayJanggun(context.MoveTeam);
            //    return;
            //}
                
            
            //if(_recoder.TryPeek(out MoveContext prev) && prev.IsJanggun)
            //{
            //    _audio.PlayMunggun();
            //}
        }

        public void     GiveUp()
        {
            var info = _match.GiveUp();

            _resultUI.Show();
            _resultUI.GiveUp(info);

        }
        public void     ResetGame()
        {
            //_resultUI.Hide();
            //_score.StartGame();
            //_recoder.StartGame();
            //_turn.StartGame(PlayerTeam.Cho);

            //_board.ResetGame(BottomPlayer);
        }
        public void Handicap()
        {
            _match.Handicap();
        }
        public void Undo()
        {
            if (!_match.TryUnDo(out var ctx))
                return;

            if (ctx.IsHandicap)
                return;

            var movedPiece = ctx.Record.MovedPiece;

            var movedId = movedPiece.Id;
            var to = ctx.Record.From;
            _board.MovePiece(movedId, to);

            if (ctx.IsCapture)
            {
                to = ctx.Record.To;
                var captured = ctx.Record.CapturedPiece;
                _board.RestoreCapturedPiece(captured.Id, captured.Team, to);
            }
        }
        private void HandlePieceMove(MoveRecord record)
        {
            var id = record.MovedPiece.Id;
            var to = record.To;
            _board.MovePiece(id, to);
            
            if(record.IsCapture)
            {
                var captured = record.CapturedPiece;
                _board.PlaceCapturedPiece(captured.Id, captured.Team);
            }
            _board.UnHighlight();
        }
        private void HandleSelectionChanged(int? id, IReadOnlyList<Pos> pos)
        {
            if (id == null)
            {
                _board.UnHighlight();
                return;
            }

            _board.Highlight(id.Value, pos);
        }
        private void HandleCheck(PlayerTeam team)
        {
            _audio.PlayJanggun();
            _matchUI.PlayJanggun(team);
        }
    }
}