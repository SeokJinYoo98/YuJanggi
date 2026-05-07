
using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;

namespace Yujanggi.Runtime.GameSession
{
    public enum SessionState
    {
        Base,
        Live,
        Replay,
        End,
        EndReplay
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
        // UI 입력
        void RequestResetGame(GameSessionInfo info, MatchModel matchModel, MatchView matchView, ReplayView replayView);

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
        protected readonly ILiveMatch _liveMatch;
        protected readonly ISessionTransition _transition;
        protected readonly IPlayerController _cho;
        protected readonly IPlayerController _han;

        public virtual void Enter() { Debug.Log($"{StateName()}_Start"); }
        public virtual void Exit() { Debug.Log($"{StateName()}_End"); }

        #region Match -> Session -> View
        public virtual void OnPieceMoved(in MoveContext moveCtx)
        {
            var record = moveCtx.Record;
            var from = record.From;
            var to = record.To;
            Debug.Log($"{StateName()}_OnPieceMoved: {from} => {to}");
            if (record.IsCapture)
            {
                from = record.To;
                Debug.Log($"{StateName()}_Captured: {from}");
            }
        }
        public virtual void OnTurnChanged(PlayerTeam next) { Debug.Log($"{StateName()}_OnTurnChanged:{next}"); }
        public virtual void OnGameEnded(in GameResultInfo info) { Debug.Log($"{StateName()}_OnGameEnded"); }
        public virtual void OnCheckOccurred(PlayerTeam team) { Debug.Log($"{StateName()}_OnCheckOccured"); }
        public virtual void OnCheckReleased() { Debug.Log($"{StateName()}_OnCheckReleased"); }
        #endregion

        #region Input -> Session -> View
        public virtual void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals) { Debug.Log($"{StateName()}_OnSelectionChanged"); }
        #endregion

        #region Input -> Session -> Model
        public virtual void RequestMove(Pos from, Pos to) { Debug.Log($"{StateName()}_RequestMove:{from} -> {to}"); }
        public virtual void RequestUndo() { Debug.Log($"{StateName()}_RequestUndo"); }
        public virtual void RequestGiveUp() { Debug.Log($"{StateName()}_RequestGiveUp"); }
        public virtual void RequestHandicap() { Debug.Log($"{StateName()}_RequestHandicap"); }
        public virtual void RequestStepBackward() { Debug.Log($"{StateName()}_RequestStepBackward"); }
        public virtual void RequestStepForward() { Debug.Log($"{StateName()}_RequestStepForward"); }
        #endregion

        #region UIRequest
        // UI 입력
        public void RequestResetGame(GameSessionInfo info, MatchModel matchModel, MatchView matchView, ReplayView replayView)
        {
            Debug.Log($"{StateName()}_ResetGame");
            replayView.ResetGame();
            matchModel.InitGame(info.ChoFormation, info.HanFormation);
            matchView.ResetGame(matchModel.Board);
            matchModel.StartGame();
            BeginNextTurn(matchModel.PlayerTurn);
            _transition.ToLive();
        }
        #endregion

        protected virtual IPlayerController BeginNextTurn(PlayerTeam turn)
        {
            Debug.Log($"{StateName()}_BeginNextTurn:{turn}");
            DisableAllControllers();
            if (turn == PlayerTeam.Cho)
            {
                _cho.BeginTurn();
                return _cho;
            }
            _han.BeginTurn();
            return _han;
        }
        protected void DisableAllControllers()
        {
            Debug.Log($"{StateName()}_DisableAllControllers");
            _cho.EndTurn(); _han.EndTurn();
        }
        protected IPlayerController GetPlayer(PlayerTeam team)
        {
            Debug.Log($"{StateName()}_GetPlayer: {team}");
            return team == PlayerTeam.Cho ? _cho : _han;
        }

        protected virtual SessionState StateName() => SessionState.Base;
    }
}