

using System;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Turn
{
    public enum TurnType
    {
        Select,
        Attack,
        Update
    }
    public struct TrunInfo
    {
        public PlayerTeam Current;

    }
    public class TurnManager
    {
        public event Action<PlayerTeam> OnTurnChanged;
        public PlayerTeam Current { get; private set; }
        public TurnType   TurnState { get; private set; }

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

            OnTurnChanged?.Invoke(Current);
        }

        private float _timer = 0.0f;
        private const float _maxTurnTime = 30.0f;

        public void Update(float deltaTime)
        {
            if (TurnState == TurnType.Update)
                return;

            _timer += deltaTime;
            if (_timer > _maxTurnTime)
            {
                _timer -= _maxTurnTime;
                // 턴 옮김 처리
            }
        }
    }
}