using System;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;

namespace Yujanggi.Runtime.GameSession
{
    public sealed class SessionEndReplayState : SessionStateBase
    {
        private readonly MatchView  _matchView;
        private readonly ReplayView _replayView;
        public SessionEndReplayState(
            ISessionTransition sessionFsm,
            IPlayerController cho, IPlayerController han,
            ILiveMatch liveMatch,
            MatchView matchView,
            ReplayView replayView)
                : base(sessionFsm, cho, han, liveMatch)
        {
            _replayView = replayView;
            _matchView  = matchView;
        }
        public override void Enter()
        {
            Debug.Log("엔드 리플레이진입");
            _replayView.EnterReplayView();
        }
        public override void Exit()
        {
            Debug.Log("엔드 리플레이 종료");
            _replayView.ExitReplayView();
        }
        public override void RequestStepBackward()
        {
            var result = _replayView.TryReplayBackward();
            Debug.Log($"{result}");
            if (result == ReplayResult.Succeeded) return;
            if (result == ReplayResult.RecordIsEmpty) _transition.ToEnd();
            if (result == ReplayResult.Failed) _transition.ToEnd();
        }
        public override void RequestStepForward()
        {
            var result = _replayView.TryReplayForward();
            Debug.Log($"{result}");
            if (result == ReplayResult.Succeeded) return;
            if (result == ReplayResult.IdxAtEnd) _transition.ToEnd();
        }
    }
    public sealed class SessionEndState : SessionStateBase
    {
        private readonly IGameResultContext _resultCtx;
        private readonly MatchView          _matchView;
        public SessionEndState(
            ISessionTransition sessionFsm, 
            IGameResultContext sessionResult,
            IPlayerController cho, IPlayerController han, 
            ILiveMatch liveMatch,
            MatchView matchView) 
            : base(sessionFsm, cho, han, liveMatch)
        {
            _resultCtx  = sessionResult;
            _matchView  = matchView;
        }

        public override void Enter()
        {
            _matchView.SyncBoardState(_liveMatch);
            if (!_resultCtx.GameResult.HasValue) _transition.ToLive();
            Debug.Log("엔드 진입");
            DisableAllControllers();
            var result          = _resultCtx.GameResult.Value;
            var isLocalLose     = GetPlayer(result.Loser).IsLocal();

            _matchView.OnGameEnded(in result, isLocalLose);
            _matchView.ShowResultUI();
        }
        public override void Exit()
        {
            Debug.Log("엔드 종료");
            _matchView.HideResultUI();
        }
        public override void RequestStepBackward()
            => _transition.ToEndReplay();
    }
}