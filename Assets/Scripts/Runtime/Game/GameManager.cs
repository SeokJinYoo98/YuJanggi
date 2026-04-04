using System.Collections.Generic;
using UnityEngine;
namespace Yujanggi.Runtime.Game
{
    using Audio;
    using Board;
    using Controller;
    using Core.Domain;
    using Core.Match;
    using Input;
    using System.Collections;
    using UI;
    using GameMode;
    public class GameManager : MonoBehaviour, IGameInputHandler
    {
        [SerializeField] private BoardPresenter _boardPresenter;

        [SerializeField] private ResultUI        _resultUI;
        [SerializeField] private MatchUI         _matchUI;
        [SerializeField] private AudioManager    _audio;

        [SerializeField] private PcInputHandler  _localInput;

        MatchOptions _gameOption;

        private void Awake()
        {
            Application.targetFrameRate = 144;
            _gameOption = new(
                PlayerType.AI, Formation.HEHE,
                PlayerType.Local, Formation.HEEH,
                30f);
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
            _han    = new LocalController(_localInput, PlayerTeam.Han);
            _cho    = new AIController(_match.Rule, _match.Board, PlayerTeam.Cho);

            BindEvents();
            _match.StartGame(_gameOption.Cho, _gameOption.Han);
            _boardPresenter.StartGame(_match.Board);

            if (_cho is AIController ai)
                StartAiTurn(ai);
        }

        #region Session
        private MatchManager _match;
        private IPlayerController _cho;
        private IPlayerController _han; 
        private IPlayerController GetPlayer(PlayerTeam team)
            => team == PlayerTeam.Cho ? _cho : _han;

        public void GiveUp()
        {
            _cho?.SetInputEnabled(false);
            _han?.SetInputEnabled(false);
            StopAiTurn();
            var info = _match.GiveUp();

            _resultUI.Show();
            _resultUI.GiveUp(info);
        }
        #endregion
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
            _cho?.SetInputEnabled(true);
            _han?.SetInputEnabled(false);
            _match.ResetGame(_gameOption.Cho, _gameOption.Han);
            _boardPresenter.ResetGame(_match.Board);
        }
        public void HandleHandicap()
        {
            _match.Handicap();
        }
        public void HandleUndo()
        {
            StopAiTurn();
            if (!_match.TryUnDo(out var ctx))
                return;

            if (ctx.IsHandicap)
                return;
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
        #endregion
        #region MatchEventHandlers
        public void HandleGameEnded(GameResultInfo info)
        {
            StopAiTurn();
            _cho?.SetInputEnabled(false);
            _han?.SetInputEnabled(false);
            _resultUI.Show();
            _resultUI.EndGame(info);
            if (GetPlayer(info.Winner) is LocalController)
                _audio.PlayWin();
            else
                _audio.PlayLose();
        }
        public void HandleCheckReleased()
        {
            _audio.PlayMunggun();
        }
        public void HandleCheckOccured(PlayerTeam team)
        {
            _audio.PlayJanggun();
            _matchUI.PlayJanggun(team);
        }
        public void HandlePieceMoved(MoveRecord record)
        {
            var moveId = record.MovedPiece.Id;
            var to = record.To;
            _boardPresenter.MovePiece(moveId, to);
            if (record.IsCapture)
            {
                _audio.PlayCapture();
                var id   = record.CapturedPiece.Id;
                var team = record.CapturedPiece.Team;
                _boardPresenter.PlaceCapturedPiece(id, team);
                return;
            }
            _audio.PlayMove();    
        }
        public void HandleSelectionChanged(int? id, IReadOnlyList<Pos> ways)
        {
            if (id == null)
            {
                _boardPresenter.UnHighlight();
                return;
            }
            _audio.PlaySelect();
            _boardPresenter.Highlight(id.Value, ways);
        }
        public void HandleTurnChanged(PlayerTeam turn)
        {
            _matchUI.UpdateTurn(turn);
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
            if (participant is LocalController local)
                _audio.PlayTurn();
            else if (participant is AIController ai)
                StartAiTurn(ai);
        }
        #endregion
        #region ControllerRequestHandlers
        public void HandleMoveRequest(Pos from, Pos to)
        {
            _match.TryMove(from, to);
        }
        public void HandleClickRequest(Pos pos)
        {
            if (_match.HasSelection)
                _match.TryMoveSelected(pos);
            else
                _match.TrySelect(pos);
        }
        #endregion
        #region BindingEvents
        private void BindEvents()
        {
            BindMatchEvents();
            BindControllerEvents();
        }
        private void BindControllerEvents()
        {
            _cho.BindEvents(this);
            _han.BindEvents(this);
        }
        private void BindMatchEvents()
        {
            _match.BindEvents();

            var turn = _match.Turn;
            turn.OnTurnChanged += HandleTurnChanged;

            var events = _match.MatchEvent;
            events.OnSelectionChanged += HandleSelectionChanged;
            events.OnCheckOccurred            += HandleCheckOccured;
            events.OnCheckReleased          += HandleCheckReleased;
            events.OnPieceMoved       += HandlePieceMoved;
            events.OnGameEnded          += HandleGameEnded;

            _matchUI.BindEvents(_match);
        }
        // UnBind
        private void UnBindEvents()
        {
            UnBindControllerEvents();
            UnBindMatchEvents();
        }
        private void UnBindMatchEvents()
        {
            _match.UnBindEvents();

            var turn = _match.Turn;
            turn.OnTurnChanged -= HandleTurnChanged;

            var events = _match.MatchEvent;
            events.OnSelectionChanged -= HandleSelectionChanged;
            events.OnCheckOccurred            -= HandleCheckOccured;
            events.OnCheckReleased          -= HandleCheckReleased;
            events.OnPieceMoved       -= HandlePieceMoved;
            events.OnGameEnded          -= HandleGameEnded;

            _matchUI.UnBindEvents(_match);
        }
        private void UnBindControllerEvents()
        {
            _cho.UnBindEvents(this);
            _han.UnBindEvents(this);
        }
        #endregion
    }
}