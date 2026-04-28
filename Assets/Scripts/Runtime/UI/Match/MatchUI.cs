
using TMPro;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;

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



        [SerializeField] private TMP_Text _janggunText;
        private bool _janggunAnim = false;
        private float _speed = 10;

        int _totalTurn = 0;
        int _currTurn  = 0;
        public void BindEvents(IMatchViewData match)
        {
            var turn = match.Turn;
            var record = match.Record;
            var score = match.Score;

            turn.OnTimeChanged         += UpdateTimer;
            turn.OnTurnChanged         += UpdateTurn;
            record.OnRecordChanged     += UpdateTotalTurn;
            score.OnScoreChanged       += UpdateScore;
        }
        public void UnBindEvents(IMatchViewData match)
        {
            var turn    = match.Turn;
            var record  = match.Record;
            var score   = match.Score;

            turn.OnTimeChanged         -= UpdateTimer;
            turn.OnTurnChanged         -= UpdateTurn;
            record.OnRecordChanged     -= UpdateTotalTurn;
            score.OnScoreChanged       -= UpdateScore;
        }
        public void Start()
        {
            _turnText.color = Color.green;
 
        }
        public void Update()
        {
            if (_janggunAnim)
            {
                var pos = _janggunText.transform.localPosition;
                pos.x += _speed;
                if (900 <= pos.x)
                {
                    _janggunAnim = false;
                    pos.x = -700;

                }
                _janggunText.transform.localPosition = pos;
            }
          
        }
        public void UpdateTotalTurn(int currTurn, int totalTurn)
        {
            _currTurn = currTurn;
            _totalTurn = totalTurn;
            UpdateRecord();
        }
        public void UpdateCurrTurn(int currTurn)
        {
            _currTurn = currTurn;
            UpdateRecord();
        }
        private void UpdateRecord()
            => _recordText.text = $"{_currTurn}수:{_totalTurn}수";



        public void UpdateTurn(PlayerTeam turn)
        {
            char turnChar;
            if (turn == PlayerTeam.Cho)
            {
                turnChar = '초';
                _turnText.color = Color.green;

            }
            else
            {
                turnChar = '한';
                _turnText.color = Color.red;
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
        public void UpdateTimer((PlayerTeam team, int time) info)
        {
            if (info.team == PlayerTeam.Han)
                _hanTimerText.text = $"{info.time}:시간";
            else
                _choTimerText.text = $"시간:{info.time}";
        }
        public void PlayJanggun(PlayerTeam team)
        {
            if (team == PlayerTeam.Cho)
                _janggunText.color = Color.green;
            else
                _janggunText.color = Color.red;

            _janggunText.transform.localPosition = new Vector2(-700, 0);
            _janggunAnim = true;
        }

    }
}