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
        [SerializeField] private BoardPresenter _boardPresenter;

        [SerializeField] private ResultUI        _resultUI;
        [SerializeField] private MatchUI         _matchUI;
        [SerializeField] private AudioManager    _audio;

        [SerializeField] private PcInputHandler  _localInput;

        private GameSession _session;

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
            _session.Match.Update(Time.deltaTime);
        }
        
        private void StartGame()
        {
            _session = new(GameSessionStore.Current, _localInput);
            BindEvents();
            _session.StartGame();
            _boardPresenter.StartGame(_session.Match.Board);

        }
        public void GiveUp()
        {
            var info = _session.GiveUp();
            StopAiTurn();
            _resultUI.Show();
            _resultUI.GiveUp(info);
        }

        #region AI
        private Coroutine _aiRoutine;
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
        #endregion
        #region UIRequestHandlers        
        public void ResetGame()
        {
            _resultUI.Hide();
            _session.ResetGame();
            _boardPresenter.ResetGame(_session.Match.Board);
        }
        public void HandleHandicap()
        {
            _session.Match.Handicap();
        }
        public void HandleUndo()
        {
            StopAiTurn();
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
            UnBindEvents();
            StopAiTurn();
            SceneManager.LoadScene("LobbyScene");
        }
        #endregion
        #region MatchEventHandlers
        public void HandleGameEnded(GameResultInfo info)
        {
            StopAiTurn();
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
        public void HandleSelectionChanged(int? id, IReadOnlyList<Pos> ways)
        {
            if (id == null)
            {
                _boardPresenter.UnHighlight();
                return;
            }
            _audio.PlaySfxOneShot(JanggiSfx.Select);
            _boardPresenter.Highlight(id.Value, ways);
        }
        public void HandleTurnChanged(PlayerTeam turn)
        {
            _matchUI.UpdateTurn(turn);

            var participant = _session.BeginNextTurn(turn);
            if (participant is LocalController local)
                _audio.PlaySfxOneShot(JanggiSfx.TurnAlert);
            else if (participant is AIController ai)
                StartAiTurn(ai);
        }
        #endregion
        #region ControllerRequestHandlers
        public void HandleMoveRequest(Pos from, Pos to)
        {
            _session.Match.TryMove(from, to);
        }
        public void HandleClickRequest(Pos pos)
        {
            Debug.Log($"{pos.X}, {pos.Z}");
            var match = _session.Match;
            if (match.HasSelection)
                match.TryMoveSelected(pos);
            else
                match.TrySelect(pos);
        }

        #endregion
        #region BindingEvents
        private void BindEvents()
        {
            _session.BindEvents(this);  
            _matchUI.BindEvents(_session.Match);
            _localInput.OnBoardClicked += HandleClickRequest;
        }
        private void UnBindEvents()
        {
            _session.UnBindEvents(this);
            _matchUI.UnBindEvents(_session.Match);
            _localInput.OnBoardClicked -= HandleClickRequest;
        }
        #endregion
    }
}