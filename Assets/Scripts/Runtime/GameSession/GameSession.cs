using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;


namespace Yujanggi.Runtime.GameSession
{
    using Replay;
    using System;
    using System.Collections.Generic;
    public class GameSession
    {
        #region public Field F
        public GameSession(
            GameSessionInfo         sessionInfo,
            GameSessionPresenter    sessionView,
            MatchManager            sessionMatch,
            ReplayPresenter         sessionReplay,
            IPlayerController       cho,
            IPlayerController       han,
            IInputHandler           localInput)
        {
            _sessionPresenter   = sessionView;
            _sessionInfo        = sessionInfo;
            _sessionMatch       = sessionMatch;
            _sessionReplay      = sessionReplay;
            _playerCho          = cho;
            _playerHan          = han;
            _localInput         = localInput;
   
        }
        public void BindEvents()
        {
            // 슬슬 이벤트 버스.
            BindReplayEvents();
            _sessionMatch.BindEvents();
            var events = _sessionMatch.MatchEvent;
            _sessionPresenter.BindUI(_sessionMatch);
            _sessionPresenter.BindLiveEvents(events);
            events.OnGameEnded        += HandleGameEnded;
            events.OnTurnChanged      += HandleTurnChanged;
            _playerCho.BindEvents(); 
            _playerHan.BindEvents();
            _playerCho.OnMoveRequest  += HandleTryMove; 
            _playerHan.OnMoveRequest  += HandleTryMove;
            if (_playerCho.IsLocal()) ((ILocalPlayer)_playerCho).OnSelectionChanged += HandleSelectionChanged;
            if (_playerHan.IsLocal()) ((ILocalPlayer)_playerHan).OnSelectionChanged += HandleSelectionChanged;
        }
        public void UnBindEvents()
        {
            UnBindReplayEvents();
            _sessionMatch.UnBindEvents();
            var events = _sessionMatch.MatchEvent;
            _sessionPresenter.UnBindUI(_sessionMatch);
            _sessionPresenter.UnBindLiveEvents(events);
            events.OnTurnChanged -= HandleTurnChanged;
            events.OnGameEnded   -= HandleGameEnded;
            _playerCho.UnBindEvents(); 
            _playerHan.UnBindEvents();
            _playerCho.OnMoveRequest -= HandleTryMove; 
            _playerHan.OnMoveRequest -= HandleTryMove;
            if (_playerCho.IsLocal()) ((ILocalPlayer)_playerCho).OnSelectionChanged -= HandleSelectionChanged;
            if (_playerHan.IsLocal()) ((ILocalPlayer)_playerHan).OnSelectionChanged -= HandleSelectionChanged;
        }
        public void Handicap()
            => _sessionMatch.Handicap();
        public void GiveUp()
        {
            if (!_sessionMatch.TryGiveUp(out var info))
                return;

            DisableAllControllers();
            HandleGameEnded(info);
        }
        public void StartGame()
        {
            _sessionMatch.StartGame(_sessionInfo.ChoFormation, _sessionInfo.HanFormation);
            _playerCho.BeginTurn();
            _playerHan.EndTurn();
            _sessionPresenter.StartGame(_sessionMatch.Board);
        }
        public void ResetGame()
        {
            _sessionMatch.StartGame(_sessionInfo.ChoFormation, _sessionInfo.HanFormation);
            _sessionPresenter.ResetGame(_sessionMatch.Board);
            _playerCho.BeginTurn();
            _playerHan.EndTurn();
        }
        public void UnDo()
        {
            if (!_sessionMatch.TryUnDo(out var ctx)) return;
            if (ctx.IsHandicap)
                return;
            _sessionPresenter.UnDo(ctx);
        }
        public void Update(float deltaTime)
            => _sessionMatch.Update(deltaTime);
        #endregion

        #region private Field Member
        private readonly IPlayerController      _playerCho;
        private readonly IPlayerController      _playerHan;
        private readonly GameSessionInfo        _sessionInfo;
        private readonly GameSessionPresenter   _sessionPresenter;
        private readonly MatchManager           _sessionMatch;
        private readonly ReplayPresenter        _sessionReplay;
        private readonly IInputHandler          _localInput;
        #endregion

        #region private Field F

        private void              HandleTurnChanged(PlayerTeam next)
        {
            var nextPlayer = GetPlayer(next);
            _sessionPresenter.OnTurnChanged(nextPlayer.IsLocal());
            BeginNextTurn(next);
        }
        private void              HandleGameEnded(GameResultInfo info)
        {
            _sessionReplay.Reset();
            DisableAllControllers();
            var loserIsLocal = GetPlayer(info.Loser).IsLocal();
            _sessionPresenter.OnGameEnded(loserIsLocal, in info);
        }
        private void              HandleSelectionChanged(int? pieceId, IReadOnlyList<Pos> legalCells, IReadOnlyList<Pos> illegalCells)
            => _sessionPresenter.OnSelectionChanged(pieceId, legalCells, illegalCells);
        private void              HandleTryMove(Pos from, Pos to)
        {
            _sessionMatch.TryMove(from, to);
        }
        private void              DisableAllControllers()
        {
            _playerCho.EndTurn(); _playerHan.EndTurn();
        }
        private IPlayerController BeginNextTurn(PlayerTeam turn)
        {
            DisableAllControllers();
            if (turn == PlayerTeam.Cho)
            {
                _playerCho.BeginTurn();
                return _playerCho;
            }
            _playerHan.BeginTurn();
            return _playerHan;
        }
        private IPlayerController GetPlayer(PlayerTeam team)
            => team == PlayerTeam.Cho ? _playerCho : _playerHan;
        #endregion

        #region Replay
        private void BindReplayEvents()
        {
            _sessionReplay.OnReplayEntered += HandleEnterReplay;
            _sessionReplay.OnReplayExited  += HandleExitReplay;
        }
        private void UnBindReplayEvents()
        {
            _sessionReplay.OnReplayEntered -= HandleEnterReplay;
            _sessionReplay.OnReplayExited  -= HandleExitReplay;
        }
        public void TryStepForward()
        {
            _sessionReplay.ReplayForward();
        }
        public void TryStepBackward()
        {
            _sessionReplay.ReplayBackward();
        }

        private void HandleEnterReplay()
        {
            _sessionPresenter.SyncBoardState(_sessionMatch);
            _sessionPresenter.UnBindLiveEvents(_sessionMatch.MatchEvent);
            _localInput.Deactivate();
        }
        private void HandleExitReplay()
        {
            _sessionPresenter.SyncBoardState(_sessionMatch);
            _sessionPresenter.BindLiveEvents(_sessionMatch.MatchEvent);
            _localInput.Activate();
        }
        #endregion
    }
}
