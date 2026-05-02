
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
        // UI 알림 (Match → State) 
        void OnTurnChanged(PlayerTeam next);
        void OnGameEnded(in GameResultInfo info);
        void OnCheckOccurred(PlayerTeam team);
        void OnCheckReleased();
        // 입력 (Controller → State)
        void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals);
        void RequestMove(Pos from, Pos to);
        void RequestHandicap();
        void RequestGiveUp();
        void RequestUndo();
        void RequestStepForward();
        void RequestStepBackward();
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

        // UI 알림 (Match → State) 
        public virtual void OnTurnChanged(PlayerTeam next)
        {
            var nextPlayer = GetPlayer(next);
            _matchView.OnTurnChanged(nextPlayer.IsLocal());
        }
        public virtual void OnGameEnded(in GameResultInfo info)
        {
            var loser = GetPlayer(info.Loser);
            _matchView.OnGameEnded(info, loser.IsLocal());
            _matchView.ShowResultUI();
        }
        public virtual void OnCheckOccurred(PlayerTeam team)
            => _matchView.CheckOccured(team);
        public virtual void OnCheckReleased()
            => _matchView.CheckReleased();

        // 입력 (Controller → State)
        public virtual void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals){ }
        public virtual void RequestMove(Pos from, Pos to) { }
        public virtual void RequestGiveUp() { }
        public virtual void RequestHandicap() { }
        public virtual void RequestStepBackward() { }
        public virtual void RequestStepForward() { }
        public virtual void RequestUndo(){ }


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