using UnityEngine;

namespace Yujanggi.Runtime.Manager
{
    using Audio;
    using Board;
    using Core.Domain;
    using Core.AI;
    using Core.Match;
    using Core.Participant;
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

        //private bool _replay = false;

        private void Awake()
        {
            Application.targetFrameRate = 144;
            _cho = new(PlayerTeam.Cho);
            _han = new(PlayerTeam.Han);
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
            var bottom  = PlayerTeam.Cho;
            var maxTime = 30f;
            _match      = new(bottom, maxTime);

            _cho.Bind(_localPlayer);
            _han.Bind(new AIController(_match.Rule, _match.Board, bottom));

            BindEvents();
            _board.StartGame(bottom, _match.Board);
        }

        private void BindEvents()
        {
            _match.BindEvents();
            _matchUI.BindEvents(_match);

            var matchEvents = _match.MatchEvent;
            matchEvents.GameEnded += EndGame;

            _audio.BindEvents(matchEvents);
            _board.BindEvents(matchEvents);

            if (_cho.Controller != null)
                _cho.Controller.OnBoardClicked += HandleClick;

            if (_han.Controller != null)
                _han.Controller.OnBoardClicked += HandleClick;

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

                if (_cho.Controller != null)
                    _cho.Controller.OnBoardClicked -= HandleClick;

                if (_han.Controller != null)
                    _han.Controller.OnBoardClicked -= HandleClick;
                var turn = _match.Turn;
                turn.OnTurnChanged -= HandleTurnChanged;
            }
        }
        public void HandleClick(Pos pos)
        {
            var result = _match.HandleCellClick(pos);
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
            _match.ResetGame();
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

        private IEnumerator ProcessAiTurn(AIController ai, PlayerTeam team)
        {
            if (!ai.TryThink(team))
                yield break;

            yield return new WaitForSeconds(0.3f);

            if (!ai.TrySelectPiece())
                yield break;
            yield return new WaitForSeconds(0.2f);
            if (!ai.TrySelectCell())
                yield break;
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
    }
}