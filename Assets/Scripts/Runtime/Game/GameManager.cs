
using UnityEngine;
namespace Yujanggi.Runtime.Game
{
    using Audio;
    using Board;
    using Core.Match;
    using GameSession;
    using Input;
    using UI;
    using UnityEngine.SceneManagement;
    using Yujanggi.Core.Board;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;

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

            _session          = CreateSession(in sessionInfo, sessionView, sessionMatch, _localInput, _runner);
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


        private GameSession CreateSession(
            in GameSessionInfo sessionInfo,
            GameSessionView    sessionView,
            MatchManager       sessionMatch,
            PcInputHandler     localInput,
            ICoroutineRunner   runner)
        {
            return new GameSession(sessionInfo, sessionView, sessionMatch, localInput, runner);
        }
        private MatchManager CreateMatch(float turnTime, out Record record)
        {
            record         = new Record();
            var turn       = new Turn(turnTime);
            var score      = new Score();
            var boardModel = new BoardModel();
            var janggiRule = new JanggiRule();
            return new MatchManager(turn, record, score, boardModel, janggiRule);
        }
        private GameSessionView CreateSessionView()
            => new GameSessionView(_boardPresenter, _resultUI, _matchUI, _audio);
        private GameSessionInfo GetSessionInfo()
            => GameSessionStore.Current;
        
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
        #endregion
    }
}