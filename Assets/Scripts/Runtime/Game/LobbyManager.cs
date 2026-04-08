using System;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Runtime.UI;

namespace Yujanggi.Runtime.Game
{
    using GameSession;
    using UnityEngine.SceneManagement;
    using Audio;
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private AIPanelView    _aiPanel;
        [SerializeField] private LocalPanelView _localPanel;
        UIVisible _curr;

        private AudioManager _audio;
        private void Awake()
        {
            Application.targetFrameRate = 144;
        }
        private void Start()
        {
            _audio = AudioManager.Instance;
        }
        public void HandleClosePanel()
        {
            _audio.PlayButton();
            if (_curr == null) return;
            _curr.Hide();
            _curr = null;
        }
        public void HandleAIPanel()
        {
            _audio.PlayButton();
            if (_curr != null) return;
            _curr = _aiPanel;
            _curr.Show();
        }
        public void HandleLocalPanel()
        {
            _audio.PlayButton();
            if (_curr != null) return;
            _curr = _localPanel;
            _curr.Show();
        }
        public void HandleCreateSession()
        {
            _audio.PlayButton();
            if (_curr == null) return;
            GameSessionInfo info;
            if (_curr is LocalPanelView local)
                info = CreateLocalSession(
                    (Formation)local.ChoFormation,
                    (Formation)local.HanFormation,
                    local.TurnTime);

            else if (_curr is AIPanelView ai)
                info = CreateAISession(
                    (PlayerTeam)ai.LocalPlayer,
                    (Formation)ai.LocalPlayerFormation,
                    ai.TurnTime);

            else return;

            GameSessionStore.Current = info;
            _curr = null;
            SceneManager.LoadScene("JanggiScene");
        }
        public void HandleQuitGame()
        {
            _audio.PlayButton(); 
            Application.Quit();
        }
        private GameSessionInfo CreateLocalSession(Formation choFormation, Formation hanFormation, int time)
        {
            return new GameSessionInfo
            {
                Mode            = GameModeType.Local,
                Cho             = PlayerType.Local,
                Han             = PlayerType.Local,
                ChoFormation    = choFormation,
                HanFormation    = hanFormation,
                TurnTime        = ConvertTime(time)
            };
        }
        private GameSessionInfo CreateAISession(PlayerTeam localTeam, Formation localFormation, int time)
        {
            GameSessionInfo info = new();
            info.Mode = GameModeType.AI;
            if (localTeam == PlayerTeam.Cho)
            {
                info.Cho                = PlayerType.Local;
                info.ChoFormation       = localFormation;
                info.Han                = PlayerType.AI;
                info.HanFormation       = GetRandomFormation();
            }
            else
            {
                info.Cho                = PlayerType.AI;
                info.ChoFormation       = GetRandomFormation();
                info.Han                = PlayerType.Local;
                info.HanFormation       = localFormation;
            }

            info.TurnTime = ConvertTime(time);
            return info;
        }
        private int             ConvertTime(int value)
        {
            return value switch
            {
                0 => 0,   // 무제한
                1 => 10,
                2 => 20,
                3 => 30,
                4 => 40,
                5 => 50,
                6 => 60,
                _ => 30
            };
        }
        private Formation       GetRandomFormation()
        {
            int count = Enum.GetValues(typeof(Formation)).Length;
            return (Formation)UnityEngine.Random.Range(0, count);
        }
    }

}
