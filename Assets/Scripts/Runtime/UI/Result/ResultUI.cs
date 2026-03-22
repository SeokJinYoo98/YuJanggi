using TMPro;
using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _winner;
        [SerializeField] private TMP_Text _cnt;
        private void Awake()
        {
            Hide();
        }
        public void Show()
            => gameObject.SetActive(true);
        public void Hide()
            => gameObject.SetActive(false);
        public void EndGame(GameResultInfo result)
        {
            if (result.Winner == PlayerTeam.Cho)
            {
                _winner.color = Color.green;
                _winner.text = "초";
            }
            else
            {
                _winner.color = Color.red;
                _winner.text = "한";
            }

            _cnt.text = result.MoveCnt.ToString();
        }
        public void GiveUp(GameResultInfo result)
        {
            if (result.Winner == PlayerTeam.Cho)
            {
                _winner.color = Color.red;
                _winner.text = "한";
            }
            else
            {
                _winner.color = Color.green;
                _winner.text = "초";
            }
            _cnt.text = result.MoveCnt.ToString();
        }
    }
}
