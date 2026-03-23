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

        private bool _replay = false;
        private void StartGame()
        {
            _score.StartGame();
            _recoder.StartGame();
            _turn.StartGame(PlayerTeam.Cho);
            _board.StartGame(BottomPlayer);
        }
        private void Awake()
        {
            Application.targetFrameRate = 144;
            float maxTurnTime = 30;
            _turn       = new(maxTurnTime);
            _score      = new();
            _recoder    = new();
        }
        private void Start()
        {
            StartGame();

            _board.OnMoved           += OnMoved;
            _turn.OnTurnChanged      += _matchUI.UpdateTurn;
            _turn.OnTimeChanged      += _matchUI.UpdateTimer;
            _turn.OnTurnEnd          += Handicap;
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
                _turn.OnTimeChanged      -= _matchUI.UpdateTimer;
                _turn.OnTurnEnd          -= Handicap;
                _score.OnScoreChanged    -= _matchUI.UpdateScore;
            }
        }

        private void Update()
        {
            _turn.Update(Time.deltaTime);
        }
        private void OnMoved(MoveContext context)
        {
            JangunCheck(context);
            SaveHistory(context);
            HandleCapture(context);
            CheckMate(context);
        }
        private void JangunCheck(in MoveContext context)
        {
            if (context.IsJanggun)
            {
                _audio.PlayJanggun();
                _matchUI.PlayJanggun(context.MoveTeam);
                return;
            }
                
            
            if(_recoder.TryPeek(out MoveContext prev) && prev.IsJanggun)
            {
                _audio.PlayMunggun();
            }
        }
        private void HandleCapture(in MoveContext context)
        {
            if (!context.IsCapture) return;
            var team = context.Record.CapturedPiece.Team;
            var type = context.Record.CapturedPiece.Type;
            var value = BoardHelper.GetPieceScore(type);

            _score.SetScore(team, value);
        }
        private void SaveHistory(in MoveContext context)
        {
            _recoder.Push(context);
        }
        private void CheckMate(in MoveContext context)
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

        public void HandleClick(int x, int z)
        {
            if (_turn.IsEnd || _replay)
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
        public void GiveUp()
        {
            _turn.SetTurn(TurnType.End);

            GameResultInfo info;
            info.MoveCnt = _recoder.MoveCount;
            info.Type    = GameResult.GiveUp;
            info.Winner  = _turn.Current;

            _resultUI.Show();
            _resultUI.GiveUp(info);
        }
        public void ResetGame()
        {
            _resultUI.Hide();
            _score.StartGame();
            _recoder.StartGame();
            _turn.StartGame(PlayerTeam.Cho);

            _board.ResetGame(BottomPlayer);
        }
        public void Undo()
        {
            if (!_recoder.TryPop(out MoveContext context))
                return;
            if (!context.IsHandicap)
            {
                var team = context.Record.CapturedPiece.Team;
                var type = context.Record.CapturedPiece.Type;
                var value = BoardHelper.GetPieceScore(type);

                _score.SetScore(team, -value);

                _board.Undo(context);
            }
            _turn.NextTurn();
        }
        public void Handicap()
        {
            _board.HandiCap();
            _recoder.Push(MoveContext.Handicap);
            _turn.NextTurn();
        }

        public void ReplayPrev()
        {
            
        }
        public void ReplayNext()
        {
            if(!_recoder.TryReplayNext(out var context))
            {
                _replay = false;
                return;
            }
        }

        public void ReplayMode()
        {
            _replay = true;
        }
    }
}