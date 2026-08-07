using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Yujanggi.Runtime.Controller
{
    using Core.Board;
    using Core.Domain;
    using Core.Rule;
    public class AIController : IPlayerController, IAIController
    {
        public PlayerTeam Team { get; }
        public event Action<Pos, Pos> OnMoveRequest;
        public bool IsLocal() => false;

        private readonly IJanggiRule _rule;
        private readonly IBoardModel _boardModel;
        private readonly IAIMoveStrategy _strategy;

        private AIMove _selectedMove;
        private bool _hasSelectedMove;

        public AIController(IJanggiRule rule, IBoardModel board, PlayerTeam team, AIMoveStrategyType strategyType)
        {
            Team                = team;
            _rule               = rule;
            _boardModel         = board;
            _strategy           = AIMoveStrategyFactory.Create(strategyType);
        }
        public void BindEvents(IGameInputReceiver receiver)
        {
            OnMoveRequest += receiver.RequestMove;
        }
        public void UnBindEvents(IGameInputReceiver receiver)
        {
            OnMoveRequest -= receiver.RequestMove;
        }

        public bool TryThink()
        {
            _hasSelectedMove = _strategy.TrySelectMove(_boardModel, _rule, Team, out _selectedMove);
            return _hasSelectedMove;
        }
        public bool TryGetSelectedMove()
        {
            if (!_hasSelectedMove)
                return false;

            OnMoveRequest?.Invoke(_selectedMove.From, _selectedMove.To);
            _hasSelectedMove = false;
            return true;
        }
        public void BeginTurn()
            => BeginAITurn();
        public void EndTurn()
            => CancelAITurn();



        private CancellationTokenSource _aiTurnCts;
        private void BeginAITurn()
        {
            CancelAITurn();

            _aiTurnCts = new CancellationTokenSource();
            ProcessAITurnAsync(_aiTurnCts.Token).Forget();
        }
        private void CancelAITurn()
        {
            if (_aiTurnCts == null)
                return;

            _aiTurnCts.Cancel();
            _aiTurnCts.Dispose();
            _aiTurnCts = null;
        }

        private async UniTask ProcessAITurnAsync(CancellationToken token)
        {
            try
            {
                if (!TryThink())
                    return;

                await UniTask.Delay(500, cancellationToken: token);

                if (!TryGetSelectedMove())
                    return;
            }
            catch (OperationCanceledException)
            {
                // 정상 취소
            }
        }
    }
}
