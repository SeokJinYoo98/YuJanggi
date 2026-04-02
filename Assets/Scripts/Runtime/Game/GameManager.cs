using UnityEngine;

namespace Yujanggi.Runtime.Game
{
    using Audio;
    using Board;
    using Core.Controller;
    using Core.Domain;
    using Core.Match;
    using Participant;
    using Input;
    using UI;
    using System.Collections;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardPresenter _board;

        [SerializeField] private ResultUI        _resultUI;
        [SerializeField] private MatchUI         _matchUI;
        [SerializeField] private AudioManager    _audio;

        [SerializeField] private LocalPlayerController _localPlayer;

        private MatchManager _match;

        private Participant _cho;
        private Participant _han;

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
            _match      = new(maxTime);

            _cho = new Participant(PlayerTeam.Cho, PlayerType.AI);
            _han = new Participant(PlayerTeam.Han, PlayerType.Local);
            _cho.Init(new AIController(_match));
            _han.Init(_localPlayer);

            if (_han.Controller is LocalPlayerController player)
                player.RotateCamera(PlayerTeam.Han);

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
                _cho.Controller.SetInputEnabled(true);
                _han.Controller.SetInputEnabled(false);
            }
            else
            {
                _cho.Controller.SetInputEnabled(false);
                _han.Controller.SetInputEnabled(true);
            }
            var participant = GetParticipant(turn);

            if (participant.Controller is AIController ai)
            {
                StartCoroutine(ProcessAiTurn(ai, turn));
            }
        }
        private Participant GetParticipant(PlayerTeam team)
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
            _cho.Controller?.SetInputEnabled(true);
            _han.Controller?.SetInputEnabled(false);
            _board.ResetGame(_match.Board);
        }
        public void  Handicap()
        {
            _match.Handicap();
        }
        public void  Undo()
        {
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
            _resultUI.Show();
            _resultUI.EndGame(info);
            var winner = GetParticipant(info.Winner);
            if (winner.Controller is LocalPlayerController)
                _audio.PlayWin();
            else
                _audio.PlayLose();
        }
        private IEnumerator ProcessAiTurn(AIController ai, PlayerTeam team)
        {
            if (!ai.TryThink(team))
                yield break;

            yield return new WaitForSeconds(1f);

            if (!ai.TryGetSelectedMove())
                yield break;

        }
    }
}