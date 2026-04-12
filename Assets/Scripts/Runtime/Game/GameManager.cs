using System.Collections.Generic;
using UnityEngine;
namespace Yujanggi.Runtime.Game
{
    using GameSession;
    using Audio;
    using Board;
    using Controller;
    using Core.Domain;
    using Input;
    using System.Collections;
    using UI;
    using UnityEngine.SceneManagement;

    public class GameManager : MonoBehaviour, IGameInputHandler
    {
        [SerializeField] private BoardPresenter  _boardPresenter;
        [SerializeField] private CoroutineRunner _runner;
        [SerializeField] private ResultUI        _resultUI;
        [SerializeField] private MatchUI         _matchUI;
        [SerializeField] private PcInputHandler  _localInput;

        private GameSession _session;
        private AudioManager _audio;

        private void Awake()
        {
            _session = new(GameSessionStore.Current, _localInput, _runner);
        }
        private void Start()
        {
            _audio = AudioManager.Instance;

            BindEvents();
            StartGame();

        }
        private void OnDestroy()
        {
            UnBindEvents();
        }
        private void Update()
        {
            _session.Match.Update(Time.deltaTime);
        }

        private void StartGame()
        {
            _session.StartGame();
            _boardPresenter.StartGame(_session.Match.Board);

        }
        #region UIRequestHandlers        
        public void HandleGiveUp()
        {
            _audio.PlayButton();
            var info = _session.GiveUp();
            _resultUI.Show();
            _resultUI.GiveUp(info);
        }
        public void HandleResetGame()
        {
            _audio.PlayButton();
            _resultUI.Hide();
            _session.ResetGame();
            _boardPresenter.ResetGame(_session.Match.Board);
        }
        public void HandleHandicap()
        {
            _audio.PlayButton();
            _session.Match.Handicap();
        }
        public void HandleUndo()
        {
            _audio.PlayButton();
            if (!_session.Match.TryUnDo(out var ctx))
                return;

            if (ctx.IsHandicap)
                return;

            // BoardPresenter도 UnDo만
            _boardPresenter.UnHighlight();
            var movedPiece = ctx.Record.MovedPiece;

            var movedId = movedPiece.Id;
            var to = ctx.Record.From;
            _boardPresenter.MovePiece(movedId, to);

            if (ctx.IsCapture)
            {
                to = ctx.Record.To;
                var captured = ctx.Record.CapturedPiece;
                _boardPresenter.RestoreCapturedPiece(captured.Id, captured.Team, to);
            }
        }
        public void HandleMainLobby()
        {
            _audio.PlayButton();
            UnBindEvents();
            SceneManager.LoadScene("LobbyScene");
        }
        #endregion
        #region MatchEventHandlers
        public void HandleGameEnded(GameResultInfo info)
        {
            _session.DisableAllControllers();
            _resultUI.Show();
            _resultUI.EndGame(info);
            if (_session.GetPlayer(info.Winner) is LocalController)
                _audio.PlaySfxOneShot(JanggiSfx.Win);
            else
                _audio.PlaySfxOneShot(JanggiSfx.Lose);
        }
        public void HandleCheckReleased()
            => _audio.PlaySfxOneShot(JanggiSfx.UnCheck);
        public void HandleCheckOccured(PlayerTeam team)
        {
            _audio.PlaySfxOneShot(JanggiSfx.Check);
            _matchUI.PlayJanggun(team);
        }
        public void HandlePieceMoved(MoveRecord record)
        {
            _boardPresenter.UnHighlight();
            var moveId = record.MovedPiece.Id;
            var to = record.To;
            _boardPresenter.MovePiece(moveId, to);
            if (record.IsCapture)
            {
                _audio.PlaySfxOneShot(JanggiSfx.Capture);
                var id   = record.CapturedPiece.Id;
                var team = record.CapturedPiece.Team;
                _boardPresenter.PlaceCapturedPiece(id, team);
                return;
            }

            _audio.PlaySfxOneShot(JanggiSfx.Move);
        }

        public void HandleTurnChanged(PlayerTeam turn)
        {
            _matchUI.UpdateTurn(turn);

            var participant = _session.BeginNextTurn(turn);
            if (participant is LocalController)
                _audio.PlaySfxOneShot(JanggiSfx.TurnAlert);
        }
        #endregion
        #region ControllerRequestHandlers


        public void HandleSelectionChanged(int? id, IReadOnlyList<Pos> legal, IReadOnlyList<Pos> illegal)
        {
            _boardPresenter.UnHighlight();

            if (!id.HasValue)
                return;

            _boardPresenter.Highlight(id.Value, legal, illegal);
            _audio.PlaySfxOneShot(JanggiSfx.Select);
        }
        #endregion
        #region BindingEvents
        private void BindEvents()
        {
            _session.BindEvents(this);  
            _matchUI.BindEvents(_session.Match);

        }
        private void UnBindEvents()
        {
            _session.UnBindEvents(this);
            _matchUI.UnBindEvents(_session.Match);
        }
        #endregion
    }
}