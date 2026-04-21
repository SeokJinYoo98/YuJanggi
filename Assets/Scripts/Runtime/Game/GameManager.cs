using System.Collections.Generic;
using UnityEngine;
namespace Yujanggi.Runtime.Game
{
    using GameSession;
    using Audio;
    using Board;
    using Input;
    using UI;
    using UnityEngine.SceneManagement;

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
            _audio          = AudioManager.Instance;
            var sessionView = new GameSessionView(_boardPresenter, _resultUI, _matchUI, _audio);
            var sessionInfo = GameSessionStore.Current;
            _session        = new GameSession(sessionInfo, _localInput, _runner, sessionView);
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