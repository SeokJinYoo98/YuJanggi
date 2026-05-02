
using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;

namespace Yujanggi.Runtime.GameSession
{
    public enum SessionState
    {
        None,
        Initializing,
        Live,
        Replay,
        Result
    }
    public interface ISessionState
    {
        void Enter();
        void Exit();

        void HandleTurnChanged(PlayerTeam next);
        void HandleGameEnded(in GameResultInfo info);
        void HandleCheck(PlayerTeam team);
        void HandleCheckReleased();

        void HandleSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals);
        void HandleTryMove(Pos from, Pos to);

        void Handicap();
        void GiveUp();
        void UnDo();

        void StepForward();
        void StepBackward();
    }

    public abstract class SessionStateBase : ISessionState
    {
        protected SessionStateBase(ISessionTransition sessionFsm, IPlayerController cho, IPlayerController han, MatchView matchView)
        {
            _transition = sessionFsm;
            _matchView  = matchView;
            _cho        = cho;
            _han        = han;
        }


        private readonly MatchView            _matchView;
        private readonly IPlayerController    _cho;
        private readonly IPlayerController    _han;

        public virtual void Enter() { }
        public virtual void Exit() { }

        public virtual void GiveUp() { }
        public virtual void Handicap() { }

        // UI
        public virtual void HandleTurnChanged(PlayerTeam next)
        {
            var nextPlayer = GetPlayer(next);
            _matchView.OnTurnChanged(nextPlayer.IsLocal());
        }
        public virtual void HandleGameEnded(in GameResultInfo info)
        {
            var loser = GetPlayer(info.Loser);
            _matchView.OnGameEnded(info, loser.IsLocal());
        }
        public virtual void HandleCheck(PlayerTeam team){ }
        public virtual void HandleCheckReleased() { }

        // Input
        public virtual void HandleSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals){ }
        public virtual void HandleTryMove(Pos from, Pos to) { }

        public virtual void StepBackward() { }
        public virtual void StepForward() { }
        public virtual void UnDo(){ }


        protected readonly ISessionTransition _transition;
        protected virtual IPlayerController BeginNextTurn(PlayerTeam turn)
        {
            if (turn == PlayerTeam.Cho)
            {
                _han.EndTurn();
                _cho.BeginTurn();
                return _cho;
            }

            _cho.EndTurn();
            _han.BeginTurn();
            return _han;
        }
        protected void                      DisableAllControllers()
        {
            _cho.EndTurn(); _han.EndTurn();
        }
        protected IPlayerController         GetPlayer(PlayerTeam team)
            => team == PlayerTeam.Cho ? _cho : _han;


    }
}