
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
        void OnPieceMoved(in MoveContext moveCtx);
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
        protected SessionStateBase(ISessionTransition sessionFsm, IPlayerController cho, IPlayerController han, ILiveMatch liveMatch)
        {
            _liveMatch  =  liveMatch;
            _transition = sessionFsm;
            _cho        = cho;
            _han        = han;
        }
        protected readonly ILiveMatch           _liveMatch;
        protected readonly ISessionTransition   _transition;
        protected readonly IPlayerController    _cho;
        protected readonly IPlayerController    _han;

        public virtual void Enter() { }
        public virtual void Exit() { }

        #region Match -> Session -> View
        public virtual void OnPieceMoved(in MoveContext moveCtx) { }
        public virtual void OnTurnChanged(PlayerTeam next) { }
        public virtual void OnGameEnded(in GameResultInfo info) { }
        public virtual void OnCheckOccurred(PlayerTeam team) { }
        public virtual void OnCheckReleased() { }
        #endregion

        #region Input -> Session -> View
        public virtual void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals) { }
        #endregion

        #region Input -> Session -> Model
        public virtual void RequestMove(Pos from, Pos to) { }
        public virtual void RequestUndo() { }
        public virtual void RequestGiveUp() { }
        public virtual void RequestHandicap() { }
        public virtual void RequestStepBackward() { }
        public virtual void RequestStepForward() { }
        #endregion

        protected virtual IPlayerController BeginNextTurn(PlayerTeam turn)
        {
            DisableAllControllers();
            if (turn == PlayerTeam.Cho)
            {
                _cho.BeginTurn();
                return _cho;
            }
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