using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Score
{
    using System;
    using Yujanggi.Utills.Board;

    public class ScoreManager
    {
        public event Action<PlayerTeam, int> OnScoreChanged;
        private int _choScore = 72;
        private int _hanScore = 72;

        public void SetScore(PlayerTeam team, int value)
        {
            if (team == PlayerTeam.Cho)
                _choScore -= value;  
            else
                _hanScore -= value;

            OnScoreChanged?.Invoke(team, team == PlayerTeam.Cho ? _choScore : _hanScore);
        }


        public PlayerTeam Winner()
        {
            if (_choScore == _hanScore)
                return PlayerTeam.None;

            return _choScore < _hanScore ? PlayerTeam.Han : PlayerTeam.Cho;
        }
    }
}