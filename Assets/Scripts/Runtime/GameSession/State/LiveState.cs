using System.Collections.Generic;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using UnityEngine;

namespace Yujanggi.Runtime.GameSession
{

    public sealed class SessionLiveState : SessionStateBase
    {
        private readonly ILiveMatch _matchModel;
        private readonly MatchView  _matchView;
        public SessionLiveState(
            ISessionTransition sessionFsm, 
            ILiveMatch matchModel, 
            IPlayerController cho, IPlayerController han, 
            MatchView matchView)
            : base(sessionFsm, cho, han, matchView)
        {
            _matchModel = matchModel;
            _matchView  = matchView;
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
        public override void GiveUp() 
        {
            if (!_matchModel.TryGiveUp(out var info))
                return;

            DisableAllControllers();
            HandleGameEnded(info);
        }
        public override void HandleGameEnded(in GameResultInfo info)
        {
            var loser = GetPlayer(info.Loser);
            _matchView.OnGameEnded(info, loser.IsLocal());
        }
        public override void HandleCheck(PlayerTeam team)
            => _matchView.CheckOccured(team);
        public override void HandleCheckReleased()
            => _matchView.CheckReleased();

        public override void Handicap() 
        {
            _matchView.UnHighlight();
            _matchModel.Handicap(out var nextPlayer);
            BeginNextTurn(nextPlayer);
        }
    

        public override void HandleSelectionChanged(int? pieceId, IReadOnlyList<Pos> legals, IReadOnlyList<Pos> illegals) 
        {
            if (!pieceId.HasValue)
            {
                _matchView.UnHighlight();
                return;
            }
            _matchView.HighlightPiece(pieceId.Value);
            _matchView.HighlightWays(legals, illegals);
        }
        public override void HandleTryMove(Pos from, Pos to) 
        {
            if (!_matchModel.TryMove(from, to, out var moveCtx))
                return;

            var record = moveCtx.Record;
            _matchView.ApplyMoveView(record);
            _matchView.UnHighlight();

            if (moveCtx.EndGame)
            {
                DisableAllControllers();
                //_transition.ToResult();
                return;
            }
           
            BeginNextTurn(moveCtx.NextPlayer);
        }
        public override void StepBackward() 
        {
            if (_matchModel.RecordCnt == 0) return;
            _transition.ToReplay();
        }
        public override void UnDo() 
        {
            if (!_matchModel.TryUnDo(out var moveCtx)) return;
            if (!moveCtx.IsHandicap)
                _matchView.RevertMoveView(moveCtx.Record);

            var nextPlayer = BeginNextTurn(moveCtx.MovePlayer);
            _matchView.OnTurnChanged(nextPlayer.IsLocal());
        }
    }
}