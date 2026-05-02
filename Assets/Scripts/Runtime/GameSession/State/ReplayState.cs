using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;

namespace Yujanggi.Runtime.GameSession
{
    public sealed class SessionReplayState : SessionStateBase
    {
        ILiveMatch _matchModel;
        ReplayView _replayView;
        public SessionReplayState(
            ISessionTransition sessionFsm, 
            ILiveMatch matchModel, 
            IPlayerController cho, IPlayerController han, 
            ReplayView replayView, 
            MatchView matchView)
            : base(sessionFsm, cho, han, matchView)
        {
            _matchModel  = matchModel;
            _replayView  = replayView;
        }

    }
}