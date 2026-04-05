

using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Runtime.GameMode;

namespace Yujanggi.Runtime.GameSession
{
    public enum GameModeType
    {
        Local, AI, Network
    }
    public struct GameSessionInfo
    {
        public GameModeType Mode;
        public PlayerType Cho;
        public Formation ChoFormation;
        public PlayerType Han;
        public Formation HanFormation;
        public float TurnTime;
    }
    public static class GameSessionStore
    {
        public static GameSessionInfo Current;
    }
}
