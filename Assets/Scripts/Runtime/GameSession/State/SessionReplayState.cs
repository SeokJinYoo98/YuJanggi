using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;

namespace Yujanggi.Runtime.GameSession
{
    public sealed class SessionReplayState : SessionStateBase
    {
        private readonly MatchView  _matchView;
        private readonly ReplayView _replayView;
        public SessionReplayState(
            ISessionTransition      sessionFsm, 
            ILiveMatch              matchModel, 
            IPlayerController cho, IPlayerController han, 
            ReplayView replayView, 
            MatchView  matchView)
            : base(sessionFsm, cho, han, matchModel)
        {
            _matchView   = matchView;
            _replayView  = replayView;
        }
        // 리플레이 준비
        public override void Enter()
        {
            Debug.Log("리플레이진입"); 
            _replayView.EnterReplayView();
        }
        // 리플레이 정리
        public override void Exit()
        {
            Debug.Log("리플레이 종료");
            _replayView.ExitReplayView();
        }
        public override void OnTurnChanged(PlayerTeam next)
        {
            var nextPlayer = GetPlayer(next);
            _matchView.OnTurnChanged(nextPlayer.IsLocal());
            BeginNextTurn(next);
        }
        // public override void OnPieceMoved(in MoveContext moveCtx) { }
        public override void OnGameEnded(in GameResultInfo info)
            => _transition.ToEnd();
        // public override void OnCheckOccurred(PlayerTeam team) { }
        // public override void OnCheckReleased() { }

        // 입력 (Controller → State)
        // public override void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals) { }
        public override void RequestMove(Pos from, Pos to)
            => _liveMatch.TryMove(from, to);
        // public override void RequestUndo() { }
        // public override void RequestGiveUp() { }
        // public override void RequestHandicap() { }
        public override void RequestStepBackward()
        {
            var result = _replayView.TryReplayBackward();
            Debug.Log($"{result}");
            if (result == ReplayResult.Succeeded) return;
            if (result == ReplayResult.RecordIsEmpty) _transition.ToLive();
            if (result == ReplayResult.Failed) _transition.ToLive();
        }
        public override void RequestStepForward() 
        {
            var result = _replayView.TryReplayForward();
            Debug.Log($"{result}");
            if (result == ReplayResult.Succeeded) return;
            if (result == ReplayResult.IdxAtEnd) _transition.ToLive();
        }
    }
}