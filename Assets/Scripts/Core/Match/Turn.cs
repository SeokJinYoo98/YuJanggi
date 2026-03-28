

using System;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Match
{
    public enum TurnType
    {
        Select,
        Attack,
        Update,
        End,
        Replay
    }
    public class Turn
    {
        public event Action<PlayerTeam> OnTurnChanged;
        public PlayerTeam Current { get; private set; }
        public TurnType   TurnState { get; private set; }
        public bool IsEnd => TurnState == TurnType.End;

        public Turn(float maxTime)
            => _maxTurnTime = maxTime;
        public void StartGame(PlayerTeam player)
        {
            Current = player;
            TurnState = TurnType.Select;
            OnTurnChanged?.Invoke(Current);
            _turnTime = _maxTurnTime;
        }

        public void SetTurn(TurnType type)
        {
            TurnState = type;
        }

        public void NextTurn()
        {
            _turnTime = _maxTurnTime;
            OnTimeChanged?.Invoke((Current, (int)_turnTime));

            Current = (Current == PlayerTeam.Cho)
                ? PlayerTeam.Han
                : PlayerTeam.Cho;

            OnTimeChanged?.Invoke((Current, (int)_turnTime));
            TurnState = TurnType.Select;
            OnTurnChanged?.Invoke(Current);
        }
        private float           _timer = 0;
        private float           _turnTime = 30;
        private readonly float  _maxTurnTime = 30;
        public event Action<(PlayerTeam team, int time)> OnTimeChanged;
        public event Action OnTurnEnd;
        public void Update(float deltaTime)
        {
            if (TurnState != TurnType.Select && TurnState != TurnType.Attack)
                return;

            _timer += deltaTime;
            if (1 <= _timer)
            {
                _turnTime -= _timer; _timer = 0;
                int remainingTime = Math.Max(0, (int)Math.Ceiling(_turnTime));
                OnTimeChanged?.Invoke((Current, remainingTime));
                if (_turnTime <= 0) OnTurnEnd?.Invoke(); 
            }
        }
    }
}