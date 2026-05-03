using System.Collections.Generic;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;

namespace Yujanggi.Runtime.GameSession
{
    public sealed class SessionReplayState : SessionStateBase
    {
        ILiveMatch _matchModel;
        ReplayView _replayView;
        public SessionReplayState(
            ISessionTransition      sessionFsm, 
            ILiveMatch              matchModel, 
            IPlayerController cho, IPlayerController han, 
            ReplayView replayView, 
            MatchView  matchView)
            : base(sessionFsm, cho, han, matchView)
        {
            _matchModel  = matchModel;
            _replayView  = replayView;
        }
        public override void Enter()
        {
            
        }
        public override void Exit()
        {
            _matchView.UnHighlight();
            _matchView.SyncBoardState(_matchModel);
        }
        // public override void OnTurnChanged(PlayerTeam next) { }
        // public override void OnGameEnded(in GameResultInfo info) { }
        public override void OnCheckOccurred(PlayerTeam team) { }
        public override void OnCheckReleased() { }

        // 입력 (Controller → State)
        // public override void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals) { }
        public override void RequestMove(Pos from, Pos to)
            => _matchModel.TryMove(from, to);
        // public override void RequestUndo() { }
        // public override void RequestGiveUp() { }
        // public override void RequestHandicap() { }
        public override void RequestStepBackward() { }
        public override void RequestStepForward() { }

    }
}