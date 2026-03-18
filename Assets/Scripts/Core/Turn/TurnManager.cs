

using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Turn
{
    public enum TurnType
    {
        Select,
        Attack,
        Update
    }

    public class TurnManager
    {
        public PlayerTeam Current { get; private set; }
        public TurnType TurnState { get; private set; }

        public void StartTurn(PlayerTeam player)
        {
            Current = player;
            TurnState = TurnType.Select;
        }

        public void SetTurn(TurnType type)
            => TurnState = type;

        public void NextTurn()
        {
            Current = (Current == PlayerTeam.Cho)
                ? PlayerTeam.Han
                : PlayerTeam.Cho;

            TurnState = TurnType.Select;
        }
    }
}