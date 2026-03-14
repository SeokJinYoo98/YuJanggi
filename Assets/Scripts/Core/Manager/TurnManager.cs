

using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Manager
{
    struct GameTurnInfo
    {
        public GameTurnInfo(PlayerType player = PlayerType.Cho, TurnType turn = TurnType.Select)
        {
            Player = player; Turn = turn;
        }
        public PlayerType Player;
        public TurnType Turn;
    }
}