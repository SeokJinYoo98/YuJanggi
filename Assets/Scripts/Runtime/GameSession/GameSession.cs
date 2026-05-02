using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using UnityEngine;

namespace Yujanggi.Runtime.GameSession
{
    using System.Collections.Generic;

    public class GameSession
    {
        #region public Field F
        public GameSession(
            GameSessionInfo         sessionInfo,
            MatchPresenter          matchPresenter,
            MatchManager            sessionMatch,
            ReplayPresenter         sessionReplay,
            IPlayerController       cho,
            IPlayerController       han,
            IInputHandler           localInput)
        {
            _matchPresenter     = matchPresenter;
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
            _matchPresenter.BindUI(_sessionMatch);
            _matchPresenter.BindLiveEvents(events);
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
            _matchPresenter.UnBindUI(_sessionMatch);
            _matchPresenter.UnBindLiveEvents(events);
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
        {
            if (!_sessionReplay.IsLiveMode) return;
            _sessionMatch.Handicap();
        }
        public void GiveUp()
        {
            if (!_sessionReplay.IsLiveMode) return;
            if (!_sessionMatch.TryGiveUp(out var info))
                return;

            DisableAllControllers();
            HandleGameEnded(info);
        }
        public void StartGame()
        {
            _sessionMatch.InitGame(_sessionInfo.ChoFormation, _sessionInfo.HanFormation);
            _matchPresenter.StartGame(_sessionMatch.Board);
            _sessionMatch.StartGame();
        }
        public void ResetGame()
        {
            _sessionReplay.Reset();
            _sessionMatch.InitGame(_sessionInfo.ChoFormation, _sessionInfo.HanFormation);
            _matchPresenter.ResetGame(_sessionMatch.Board);
            _sessionMatch.StartGame();
        }
        public void UnDo()
        {
            if (!_sessionReplay.IsLiveMode) return;
            if (!_sessionMatch.TryUnDo(out var ctx)) return;
            _matchPresenter.UnDo(ctx);
        }
        public void Update(float deltaTime)
            => _sessionMatch.Update(deltaTime);
        #endregion

        #region private Field Member
        private readonly IPlayerController      _playerCho;
        private readonly IPlayerController      _playerHan;
        private readonly GameSessionInfo        _sessionInfo;
        private readonly MatchPresenter         _matchPresenter;
        private readonly MatchManager           _sessionMatch;
        private readonly ReplayPresenter        _sessionReplay;
        private readonly IInputHandler          _localInput;
        #endregion

        #region private Field F

        private void              HandleTurnChanged(PlayerTeam next)
        {
            BeginNextTurn(next); 
            var nextPlayer = GetPlayer(next);
            _matchPresenter.OnTurnChanged(nextPlayer.IsLocal());
        }
        private void              HandleGameEnded(GameResultInfo info)
        {
            _sessionReplay.Reset();
            DisableAllControllers();
            var loserIsLocal = GetPlayer(info.Loser).IsLocal();
            _matchPresenter.OnGameEnded(loserIsLocal, in info);
        }
        private void              HandleSelectionChanged(int? pieceId, IReadOnlyList<Pos> legalCells, IReadOnlyList<Pos> illegalCells)
            => _matchPresenter.OnSelectionChanged(pieceId, legalCells, illegalCells);
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
            if (_sessionReplay.IsLiveMode) _matchPresenter.Clear();

            if (turn == PlayerTeam.Cho)
            {
                Debug.Log("턴: 초");
                _playerHan.EndTurn();
                _playerCho.BeginTurn();
                return _playerCho;
            }
            Debug.Log("턴: 한");
            _playerCho.EndTurn();
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
        public void StepForward()
        {
            _sessionReplay.ReplayForward();
        }
        public void StepBackward()
        {
            _sessionReplay.ReplayBackward();
        }

        private void HandleEnterReplay()
        {
            _matchPresenter.SyncBoardState(_sessionMatch);
            _matchPresenter.UnBindLiveEvents(_sessionMatch.MatchEvent);
            _localInput.Deactivate();
        }
        private void HandleExitReplay()
        {
            if (_sessionMatch.Turn.IsEnd)
            {
                _matchPresenter.ShowResultUI();
            }
            else
            {
                _matchPresenter.SyncBoardState(_sessionMatch);
                _matchPresenter.BindLiveEvents(_sessionMatch.MatchEvent);
                _localInput.Activate();
            }

        }
        #endregion
    }
}
