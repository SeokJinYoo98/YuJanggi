using TMPro;
using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.UI
{
    public class ResultUI : UIVisible
    {
        [SerializeField] private TMP_Text _winner;
        [SerializeField] private TMP_Text _cnt;
        [SerializeField] private TMP_Text _result;

        public void EndGame(in GameResultInfo result)
        {
            switch(result.Type)
            {
                case GameResult.CheckMate:
                    CheckMate(in result);
                    break;
                case GameResult.GiveUp:
                    GiveUp(in result);
                    break;
                default:
                    break;
            }

        }
        private void CheckMate(in GameResultInfo result)
        {
            if (result.Loser == PlayerTeam.Cho)
            {
                _winner.color = Color.green;
                _winner.SetText("초");
            }
            else
            {
                _winner.color = Color.red;
                _winner.SetText("한");

            }
            _result.SetText("[외통수]");
            _cnt.SetText("{0}", result.MoveCnt);
        }
        private void GiveUp(in GameResultInfo result)
        {
            if (result.Loser == PlayerTeam.Cho)
            {
                _winner.color = Color.red;
                _winner.SetText("한");
            }
            else
            {
                _winner.color = Color.green;
                _winner.SetText("초");
            }
            _result.SetText("[기권승]");
            _cnt.SetText("{0}", result.MoveCnt);
        }
    }
}
