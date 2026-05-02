using System.Collections.Generic;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Runtime.Piece;

namespace Yujanggi.Runtime.GameSession
{
    /*
    유지 OK:
    - BindUI
    - Check/UnCheck 표시
    - SelectionChanged
    - MoveRequest는 세션/상태 경유

    줄이거나 제거 추천:
    - OnTurnChanged로 BeginTurn 실행
    - OnGameEnded로 상태 전환/컨트롤러 종료
    - PieceMoved로 핵심 흐름 연결
     */

    public class GameSession : ISessionTransition, IGameInputReceiver
    {
        #region public Field F
        public GameSession(
            GameSessionInfo    sessionInfo,
            MatchView          matchView,
            MatchModel         matchModel,
            ReplayView         replayView,
            IPlayerController  cho,
            IPlayerController  han,
            IInputHandler      localInput)
        {
            _matchView    = matchView;
            _sessionInfo  = sessionInfo;
            _matchModel   = matchModel;
            _replayView   = replayView;
            _playerCho    = cho;
            _playerHan    = han;
            _localInput   = localInput;

            _states = new Dictionary<SessionState, ISessionState>();
            _states[SessionState.Live]   = new SessionLiveState(this, _matchModel, _playerCho, _playerHan, _matchView);
            _states[SessionState.Replay] = new SessionReplayState(this, _matchModel, _playerCho, _playerHan, _replayView, _matchView);
            ChangeState(SessionState.Live);
        }
        public void StartGame()
        {
            _matchModel.InitGame(_sessionInfo.ChoFormation, _sessionInfo.HanFormation);
            _matchView.StartGame(_matchModel.Board);
            _matchModel.StartGame();

            _playerCho.BeginTurn();
            _playerHan.EndTurn();
        }
        public void ResetGame()
        {
            _replayView.ResetGame();
            _matchModel.InitGame(_sessionInfo.ChoFormation, _sessionInfo.HanFormation);
            _matchView.ResetGame(_matchModel.Board);
            _matchModel.StartGame();
            HandleTurnChanged(_matchModel.PlayerTurn);
            ChangeState(SessionState.Live);
        }
        public void BindEvents()
        {
            // 슬슬 이벤트 버스.
            _matchModel.BindEvents();
            _matchView.BindUI(_matchModel);

            var events = _matchModel.MatchEvent;
            events.OnCheckOccurred += HandleCheck;
            events.OnCheckReleased += HandleCheckReleased;
            events.OnGameEnded     += HandleGameEnded;
            events.OnTurnChanged   += HandleTurnChanged;

            _playerCho.BindEvents(this); 
            _playerHan.BindEvents(this);
        }
        public void UnBindEvents()
        {
            _matchModel.UnBindEvents();
            _matchView.UnBindUI(_matchModel);

            var events = _matchModel.MatchEvent;
            events.OnCheckOccurred -= HandleCheck;
            events.OnCheckReleased -= HandleCheckReleased;
            events.OnTurnChanged   -= HandleTurnChanged;
            events.OnGameEnded     -= HandleGameEnded;

            _playerCho.UnBindEvents(this); 
            _playerHan.UnBindEvents(this);
        }

        public void Update(float deltaTime)
            => _matchModel.Update(deltaTime);
        #endregion

        #region private Field Member   
        private SessionState _currState = SessionState.None;
        private readonly Dictionary<SessionState, ISessionState> _states;

        private readonly GameSessionInfo        _sessionInfo;
        private readonly IInputHandler          _localInput;
        private readonly IPlayerController      _playerCho;
        private readonly IPlayerController      _playerHan;

        private readonly ReplayView             _replayView;
        private readonly MatchView              _matchView;
        private readonly MatchModel             _matchModel;


        #endregion

        #region private Field F
        // Events
        private void HandleCheckReleased()
            => _states[_currState].HandleCheckReleased();
        private void HandleCheck(PlayerTeam team)
            => _states[_currState].HandleCheck(team);
        private void HandleTurnChanged(PlayerTeam next)
            => _states[_currState].HandleTurnChanged(next);
        private void HandleGameEnded(GameResultInfo info)
            => _states[_currState].HandleGameEnded(in info);
        // Player
        public void RequestMove(Pos from, Pos to)
            => _states[_currState].HandleTryMove(from, to);
        public void ChangeSelection(int? pieceId, IReadOnlyList<Pos> legal, IReadOnlyList<Pos> illegal)
            => _states[_currState].HandleSelectionChanged(pieceId, legal, illegal);
        // UI
        public void  StepForward()
            => _states[_currState].StepForward();
        public void  StepBackward()
            => _states[_currState].StepBackward();
        public void  Handicap()
            => _states[_currState].Handicap();
        public void  GiveUp()
            => _states[_currState].GiveUp();
        public void  UnDo()
            => _states[_currState].UnDo();
        private void ChangeState(SessionState next)
        {
            if (_currState == next)
                return;

            if (_states.TryGetValue(_currState, out var curr))
                curr.Exit();

            _currState = next;
            _states[_currState].Enter();
        }
        #endregion

        #region Replay

        private void HandleEnterReplay()
        {
            _matchView.SyncBoardState(_matchModel);
            _localInput.Deactivate();
        }
        private void HandleExitReplay()
        {
            if (_matchModel.Turn.IsEnd)
            {
                _matchView.ShowResultUI();
            }
            else
            {
                _matchView.SyncBoardState(_matchModel);
                _localInput.Activate();
            }

        }

        public void ToLive()
            => ChangeState(SessionState.Live);
        public void ToReplay()
            => ChangeState(SessionState.Replay);
        public void ToResult()
            => ChangeState(SessionState.Result);


        #endregion
    }
}
