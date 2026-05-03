using System.Collections.Generic;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using UnityEngine;

namespace Yujanggi.Runtime.GameSession
{

    public sealed class SessionLiveState : SessionStateBase
    {
        private readonly ILiveMatch _matchModel;
        public SessionLiveState(
            ISessionTransition sessionFsm, 
            ILiveMatch matchModel, 
            IPlayerController cho, IPlayerController han, 
            MatchView matchView)
            : base(sessionFsm, cho, han, matchView)
        {
            _matchModel = matchModel;
        }
        public override  void Enter() 
        {
            _matchView.UnHighlight();
            _matchView.SyncBoardState(_matchModel);
        }
        public override  void Exit() 
        {
            _matchView.UnHighlight();
        }

        public override void OnPieceMoved(MoveContext moveCtx)
        {
            _matchView.UnHighlight();
            if (moveCtx.IsHandicap) return;

            _matchView.ApplyMoveView(moveCtx.Record);
        }
        public override void OnTurnChanged(PlayerTeam next)
        {
            var nextPlayer = GetPlayer(next);
            _matchView.OnTurnChanged(nextPlayer.IsLocal());
            BeginNextTurn(next);
        }
        public override void OnGameEnded(in GameResultInfo info)
        {
            DisableAllControllers();
            var loser = GetPlayer(info.Loser);
            _matchView.OnGameEnded(info, loser.IsLocal());
            _matchView.ShowResultUI();
        }
        public override void OnCheckOccurred(PlayerTeam team)
            => _matchView.CheckOccured(team);
        public override void OnCheckReleased()
            => _matchView.CheckReleased();
        //
        public override void OnSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals)
        {
            if (!pieceId.HasValue)
            {
                _matchView.UnHighlight();
                return;
            }
            _matchView.HighlightPiece(pieceId.Value);
            _matchView.HighlightWays(legals, illegals);
        }

        public override void RequestGiveUp()
            => _matchModel.GiveUp();
        public override void RequestHandicap()
        {
            _matchView.UnHighlight();
            _matchModel.Handicap();
        }

        public override void RequestMove(Pos from, Pos to)
            => _matchModel.TryMove(from, to);
        public override void RequestStepBackward() 
        {
            if (_matchModel.RecordCnt == 0) return;
            _transition.ToReplay();
        }
        public override void RequestUndo() 
        {
            if (!_matchModel.TryUnDo(out var moveCtx)) 
                return;

            if (!moveCtx.IsHandicap) 
                _matchView.RevertMoveView(moveCtx.Record);
        }
    }
}