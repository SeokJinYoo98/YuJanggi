using UnityEngine;

namespace Yujanggi.Runtime.Game
{
    using Audio;
    using Board;
    using Core.Controller;
    using Core.Domain;
    using Core.Match;
    using Input;
    using UI;
    using System.Collections;

    public class GameManager : MonoBehaviour, IGameManager
    {
        [SerializeField] private BoardPresenter _board;

        [SerializeField] private ResultUI        _resultUI;
        [SerializeField] private MatchUI         _matchUI;
        [SerializeField] private AudioManager    _audio;

        [SerializeField] private PcInputHandler  _localInput;

        private MatchManager _match;
        private IPlayerController _cho;
        private IPlayerController _han;

        private Coroutine _aiRoutine;
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
            var maxTime = 30f;
            _match  = new(maxTime);
            _cho    = new LocalController(_localInput, PlayerTeam.Cho);
            _han    = new AIController(_match, PlayerTeam.Han);

            BindEvents();
            _board.StartGame(_match.Board);
            _match.StartGame();
        }

        private void BindEvents()
        {
            _match.BindEvents();
            _matchUI.BindEvents(_match);

            var matchEvents = _match.MatchEvent;
            matchEvents.GameEnded += EndGame;

            _audio.BindEvents(matchEvents);
            _board.BindEvents(matchEvents);

            _cho.BindEvents(this);
            _han.BindEvents(this);

            var turn = _match.Turn;
            turn.OnTurnChanged += HandleTurnChanged;
        }
        private void UnBindEvents()
        {
            
            if (_matchUI != null && _match != null)
            {
                _match.UnBindEvents();
                _matchUI.UnBindEvents(_match);
                var matchEvents = _match.MatchEvent;
                matchEvents.GameEnded -= EndGame;
                _audio.UnBindEvents(matchEvents);
                _board.UnBindEvents(matchEvents);

                _cho.UnBindEvents(this);
                _han.UnBindEvents(this);

                var turn = _match.Turn;
                turn.OnTurnChanged -= HandleTurnChanged;
            }
        }
        public void HandleMove(Pos from, Pos to)
        {
            _match.TryMove(from, to);
        }
        public void HandleClick(Pos pos)
        {
            if (_match.HasSelection)
                _match.TryMoveSelected(pos);
            else
                _match.TrySelect(pos);
        }
        
        private void HandleTurnChanged(PlayerTeam turn)
        {
            
            if (turn == PlayerTeam.Cho)
            {
                _cho.SetInputEnabled(true);
                _han.SetInputEnabled(false);
            }
            else
            {
                _cho.SetInputEnabled(false);
                _han.SetInputEnabled(true);
            }
            var participant = GetPlayer(turn);
            if (participant is AIController ai)
                StartAiTurn(ai);
        }
        private IPlayerController GetPlayer(PlayerTeam team)
            => team == PlayerTeam.Cho ? _cho : _han;
        public void  GiveUp()
        {
            var info = _match.GiveUp();

            _resultUI.Show();
            _resultUI.GiveUp(info);

        }
        public void  ResetGame()
        {
            _resultUI.Hide();
            _match.StartGame();
            _cho?.SetInputEnabled(true);
            _han?.SetInputEnabled(false);
            _board.ResetGame(_match.Board);
        }
        public void  Handicap()
        {
            _match.Handicap();
        }
        public void  Undo()
        {
            StopAiTurn();
            if (!_match.TryUnDo(out var ctx))
                return;

            if (ctx.IsHandicap)
                return;

            
            _board.UnHighlight();
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

        private void EndGame(GameResultInfo info)
        {
            StopAiTurn();
            _resultUI.Show();
            _resultUI.EndGame(info);
            if (GetPlayer(info.Winner) is LocalController)
                _audio.PlayWin();
            else
                _audio.PlayLose();
        }
        private IEnumerator ProcessAiTurn(AIController ai)
        {
            if (!ai.TryThink())
                yield break;

            yield return new WaitForSeconds(1f);

            if (!ai.TryGetSelectedMove())
                yield break;
        }

        private void StartAiTurn(AIController ai)
        {
            StopAiTurn();

            _aiRoutine = StartCoroutine(ProcessAiTurn(ai));
        }
        private void StopAiTurn()
        {
            if (_aiRoutine != null)
            {
                StopCoroutine(_aiRoutine);
                _aiRoutine = null;
            }
        }
    }
}