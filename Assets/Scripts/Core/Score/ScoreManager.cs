using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Score
{
    using Yujanggi.Utills.Board;

    public class ScoreManager
    {
        private int _choScore = 72;
        private int _hanScore = 72;

        public void AddCaptureScore(PieceInfo captured)
        {
            int value = BoardHelper.GetPieceScore(captured.Type);

            if (captured.Team == PlayerTeam.Cho)
                _choScore -= value;  
            else
                _hanScore -= value;
        }

        public PlayerTeam Winner()
        {
            if (_choScore == _hanScore)
                return PlayerTeam.None;

            return _choScore < _hanScore ? PlayerTeam.Han : PlayerTeam.Cho;
        }
    }
}