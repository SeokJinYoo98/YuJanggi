
using TMPro;
using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.UI
{
    public class MatchUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _recordText;
        [SerializeField] private TMP_Text _turnText;

        [SerializeField] private TMP_Text _choScoreText;
        [SerializeField] private TMP_Text _choTimerText;

        [SerializeField] private TMP_Text _hanScoreText;
        [SerializeField] private TMP_Text _hanTimerText;

        TMP_Text _currTimer = null;
        const float _maxTime = 30;
        float       _time    = 30;
        float       _acc     = 0;
        public void Start()
        {
            _turnText.color = Color.green;
            _currTimer = _choTimerText;
        }
        public void Update()
        {
            if (_currTimer == null) return;
            _acc += Time.deltaTime;

            if (_acc >= 1.0f && 0 <= _time)
            {
                _acc   -= 1.0f;
                _time  -= 1.0f;
                if (_currTimer == _hanTimerText)
                    _currTimer.text = $"{(int)_time}:시간";
                else
                    _currTimer.text = $"시간:{(int)_time}";
            }
        }
        public void UpdateRecord((int now, int cnt) record)
        {
            _recordText.text = $"{record.now}수:{record.cnt}수";
        }
        public void UpdateTurn(PlayerTeam turn)
        {
            _time = _maxTime;
            char turnChar;
            if (turn == PlayerTeam.Cho)
            {
                _currTimer.text = $"{(int)_time}:시간";
                turnChar = '초';
                _turnText.color = Color.green;
                _currTimer = _choTimerText;
            }
            else
            {
                _currTimer.text = $"시간:{(int)_time}";
                turnChar = '한';
                _turnText.color = Color.red;
                _currTimer = _hanTimerText;
            }

            _turnText.text = $"차례:{turnChar}";
        }
        public void UpdateScore(PlayerTeam team, int score)
        {
            if (team == PlayerTeam.Cho)
                _choScoreText.text = $"점수:{score}";
            
            else
                _hanScoreText.text = $"{score}:점수";
            
        }
    }
}