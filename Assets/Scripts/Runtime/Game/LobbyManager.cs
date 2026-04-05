using System;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Runtime.GameMode;
using Yujanggi.Runtime.UI;

namespace Yujanggi.Runtime.Game
{
    using GameSession;
    using UnityEngine.SceneManagement;

    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private AIPanelView    _aiPanel;
        [SerializeField] private LocalPanelView _localPanel;
        UIVisible _curr;

        public void HandleClosePanel()
        {
            if (_curr == null) return;
            _curr.Hide();
            _curr = null;
        }
        public void HandleAIPanel()
        {
            if (_curr != null) return;
            _curr = _aiPanel;
            _curr.Show();
        }
        public void HandleLocalPanel()
        {
            if (_curr != null) return;
            _curr = _localPanel;
            _curr.Show();
        }
        public void HandleCreateSession()
        {
            if (_curr == null) return;
            GameSessionInfo info;
            if (_curr is LocalPanelView local)
                info = HandleCreateLocalSession(
                    (Formation)local.ChoFormation,
                    (Formation)local.HanFormation,
                    local.TurnTime);

            else if (_curr is AIPanelView ai)
                info = HandleCreateAISession(
                    (PlayerTeam)ai.LocalPlayer,
                    (Formation)ai.LocalPlayerFormation,
                    ai.TurnTime);

            else return;

            GameSessionStore.Current = info;
            _curr = null;
            SceneManager.LoadScene("Janggi");
        }
        public void HandleQuitGame()
            => Application.Quit();
        public GameSessionInfo HandleCreateLocalSession(Formation choFormation, Formation hanFormation, int time)
        {
            return new GameSessionInfo
            {
                Cho             = PlayerType.Local,
                Han             = PlayerType.Local,
                ChoFormation    = choFormation,
                HanFormation    = hanFormation,
                TurnTime        = ConvertTime(time)
            };
        }

        public GameSessionInfo HandleCreateAISession(PlayerTeam localTeam, Formation localFormation, int time)
        {
            GameSessionInfo info = new();
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
        private int ConvertTime(int value)
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
        private Formation GetRandomFormation()
        {
            int count = Enum.GetValues(typeof(Formation)).Length;
            return (Formation)UnityEngine.Random.Range(0, count);
        }
    }

}
