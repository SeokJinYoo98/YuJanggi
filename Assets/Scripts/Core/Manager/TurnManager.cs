

using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Manager
{
    struct GameTurnInfo
    {
        public GameTurnInfo(PlayerTeam player = PlayerTeam.Cho, TurnType turn = TurnType.Select)
        {
            Player = player; Turn = turn;
        }
        public PlayerTeam Player;
        public TurnType Turn;
    }
}