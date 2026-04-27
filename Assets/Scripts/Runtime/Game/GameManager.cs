using UnityEngine;
using UnityEngine.SceneManagement;
namespace Yujanggi.Runtime.Game
{
    using Audio;
    using Board;
    using Core.Board;
    using Core.Domain;
    using Core.Match;
    using Core.Rule;
    using GameSession;
    using Input;
    using System;
    using UI;
    using Yujanggi.Runtime.Controller;
    using Yujanggi.Runtime.Replay;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardPresenter  _boardPresenter;
        [SerializeField] private CoroutineRunner _runner;
        [SerializeField] private ResultUI        _resultUI;
        [SerializeField] private MatchUI         _matchUI;
        [SerializeField] private PcInputHandler  _localInput;

        private GameSession  _session;
        private AudioManager _audio;

        private void Awake()
        {
            _audio            = AudioManager.Instance;

            var sessionInfo   = GetSessionInfo();
            var sessionView   = CreateSessionView();
            var sessionMatch  = CreateMatch(sessionInfo.TurnTime, out var record);
            var sessionReplay = CreateReplayManager(record);

            var sessionCho    = CreateController(sessionInfo.Cho, PlayerTeam.Cho, _localInput, sessionMatch, _runner);
            var sessionHan    = CreateController(sessionInfo.Han, PlayerTeam.Han, _localInput, sessionMatch, _runner);

            _session          = CreateSession(in sessionInfo, sessionView, sessionMatch, sessionReplay, sessionCho, sessionHan);

            SetCamera(in sessionInfo);
        }
        private void Start()
        {
            _session.BindEvents();
            _session.StartGame();
        }
        private void OnDestroy()
        {
            _session.UnBindEvents();
        }
        private void Update()
        {
            _session.Update(Time.deltaTime);
        }

        private void SetCamera(in GameSessionInfo sessionInfo)
        {
            if (sessionInfo.Mode == GameModeType.Local) return;
            if (sessionInfo.Cho  == PlayerType.Local) return;

            _localInput.RotateCamera(PlayerTeam.Han);
        }
        private GameSessionInfo GetSessionInfo()
            => GameSessionStore.Current;


        #region SessionFactory       
        private GameSession      CreateSession(
            in GameSessionInfo      sessionInfo,
            GameSessionPresenter    sessionView,
            MatchManager            sessionMatch,
            ReplayPresenter           sessionReplay,
            IPlayerController       cho,
            IPlayerController       han)
        {
            return new GameSession(
                sessionInfo, 
                sessionView, 
                sessionMatch,
                sessionReplay, 
                cho, han, 
                _localInput);
        }
        private ReplayPresenter          CreateReplayManager(Record record)
        {
            return new ReplayPresenter(_boardPresenter, record, _runner, _audio);
        }
        private MatchManager           CreateMatch(float turnTime, out Record record)
        {
            record         = new Record();
            var turn       = new Turn(turnTime);
            var score      = new Score();
            var boardModel = new BoardModel();
            var janggiRule = new JanggiRule();
            return new MatchManager(turn, record, score, boardModel, janggiRule);
        }
        private GameSessionPresenter   CreateSessionView()
            => new GameSessionPresenter(_boardPresenter, _resultUI, _matchUI, _audio);
        private IPlayerController      CreateController(
           PlayerType type,
           PlayerTeam team,
           PcInputHandler input,
           MatchManager match,
           ICoroutineRunner runner)
        {
            // 매치를 참조하지 말고 그냥 룰과 보드를 보내주는 방향으로
            return type switch
            {
                PlayerType.Local => new LocalController(match.Rule, match.Board, team, input),
                PlayerType.AI => new AIController(match.Rule, match.Board, team, runner),
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
        public void HandleReplayForward()
        {
            Debug.Log("Replay F");
            _audio.PlayButton();
            _session.StepForward();
        }
        public void HandleReplayBackward()
        {
            Debug.Log("Replay B");
            _audio.PlayButton();
            _session.StepBackward();
        }
        #endregion
    }
}