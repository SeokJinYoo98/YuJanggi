using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.GameMode
{
    public enum Formation { HEHE, EHEH, EHHE, HEEH }
    public enum PlayerType { Local, AI, Network }
    public readonly struct PlayerInfo
    {
        public PlayerInfo(PlayerType type,Formation formation)
        {
            Formation   = formation;
            PlayerType  = type;
        }
        public readonly Formation  Formation;
        public readonly PlayerType PlayerType;
    }
    public readonly struct MatchOptions
    {
        public MatchOptions(
            PlayerType choType, Formation choFormation,
            PlayerType hanType, Formation hanFormation,
            float turnTime)
        {
            Cho = new(choType, choFormation);
            Han = new(hanType, hanFormation);
            TurnTime = turnTime;
        }
        public readonly PlayerInfo Cho;
        public readonly PlayerInfo Han;
        public readonly float TurnTime;
    }
    public interface IGameMode
    {
        public void SetUpGame();
    }
    public class LocalVsLocalMode : IGameMode
    {
        IPlayerController player;
        public void SetUpGame()
        {

        }
    }
    public class LocalVsAiMode : IGameMode
    {
        public void SetUpGame()
        {

        }
    }

    public class LocalVsNetworkMode : IGameMode
    {
        public void SetUpGame()
        {

        }
    }
}