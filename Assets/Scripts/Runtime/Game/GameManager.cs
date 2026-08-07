using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
namespace Yujanggi.Runtime.Game
{

    using Core.Board;
    using Core.Domain;
    using Core.Match;
    using Core.Rule;
    using GameSession;
    using Input;
    using System;
    using Audio;
    using Board;
    using Controller;
    using UI;
    using Particle;

    public class GameManager : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private BoardView     _boardView;
        [SerializeField] private MoveGuideView _moveGuideView;
        [SerializeField] private ParticleView  _particleView;

        [Header("UIs")]
        [SerializeField] private ResultUI   _resultUI;
        [SerializeField] private MatchUI    _matchUI;
        [SerializeField] private TMP_Text   _displayModeText;


        [Header("Inputs")]
        [SerializeField] private InputHandlerBehaviour _localInput;
        [SerializeField] private CoroutineRunner       _runner;

        private GameSession  _session;
        private AudioManager _audio;

        private void Awake()
        {
            _audio = AudioManager.Instance;
            var sessionInfo   = GetSessionInfo();
            var matchView     = CreateMatchView();
            var matchModel    = CreateMatchModel(sessionInfo.TurnTime, out var record);
            var replayView    = CreateReplayView(record);
            var sessionCho    = CreateController(sessionInfo.Cho, PlayerTeam.Cho, _localInput, matchModel);
            var sessionHan    = CreateController(sessionInfo.Han, PlayerTeam.Han, _localInput, matchModel);

            _session          = CreateSession(in sessionInfo, matchView, matchModel, replayView, sessionCho, sessionHan);

            SetCamera(in sessionInfo);
        }
        private void OnEnable()
        {
            _session?.BindEvents();
        }
        private void Start()
        {
            _session.StartGame();
        }
        private void OnDisable()
        {
            _session?.UnBindEvents();
        }
        private void Update()
        {
            _session?.Tick(Time.deltaTime);
        }

        private void SetCamera(in GameSessionInfo sessionInfo)
        {
            if (sessionInfo.Mode == GameModeType.Local) return;
            if (sessionInfo.Cho  == PlayerType.Local) return;

            _boardView.SetDeathPosition(new Vector3(4, 0, 11));
            _localInput.RotateCamera(PlayerTeam.Han);
        }
        private GameSessionInfo GetSessionInfo()
            => GameSessionStore.Current;


        #region SessionFactory       
        private GameSession      CreateSession(
            in GameSessionInfo      sessionInfo,
            MatchView               matchView,
            MatchModel              matchModel,
            ReplayView              replayView,
            IPlayerController       cho,
            IPlayerController       han)
        {
            return new GameSession(
                sessionInfo, 
                matchView, 
                matchModel,
                replayView, 
                cho, han, 
                _localInput);
        }
        private ReplayView           CreateReplayView(Record record)
        {
            return new ReplayView(_boardView, record, _runner, _audio, _displayModeText);
        }
        private MatchModel           CreateMatchModel(float turnTime, out Record record)
        {
            record         = new Record();
            var turn       = new Turn(turnTime);
            var score      = new Score();
            var boardModel = new BoardModel();
            var janggiRule = new JanggiRule();
            return new MatchModel(turn, record, score, boardModel, janggiRule);
        }
        private MatchView            CreateMatchView()
            => new MatchView(_particleView, _moveGuideView, _boardView, _resultUI, _matchUI);
        private IPlayerController    CreateController(
           PlayerType type,
           PlayerTeam team,
           IInputHandler input,
           MatchModel match)
        {
            return type switch
            {
                PlayerType.Local => new LocalController(match.Rule, match.Board, team, input),
                PlayerType.AI => new AIController(match.Rule, match.Board, team, AISessionSettings.Strategy),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        #endregion
        #region UIRequestHandlers        
        public void HandleGiveUp()
        {
            _audio.PlayButton();
            _session.GiveUp();
        }
        public void HandleResetGame()
        {
            _audio.PlayButton();
            _session.ResetGame();
        }
        public void HandleHandicap()
        {
            _audio.PlayButton();
            _session.Handicap();
        }
        public void HandleUndo()
        {
            _audio.PlayButton();
            _session.UnDo();
        }
        public void HandleMainLobby()
        {
            _audio.PlayButton();
            _session.UnBindEvents();
            SceneManager.LoadScene("LobbyScene");
        }
        public void HandleReplayModeEnter()
        {
            _resultUI.Hide();
            HandleReplayBackward();
        }
        public void HandleReplayForward()
        {
            _audio.PlayButton();
            _session.StepForward();
        }
        public void HandleReplayBackward()
        {
            _audio.PlayButton();
            _session.StepBackward();
     
        }
        #endregion
        
    }
}
