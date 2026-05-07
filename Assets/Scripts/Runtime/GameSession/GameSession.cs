using System.Collections.Generic;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Runtime.Piece;

namespace Yujanggi.Runtime.GameSession
{
    /*
    1. Request~
아직 확정되지 않은 행동을 모델에 시도시키는 함수

    2. On~
모델에서 이미 확정된 결과를 받아 후처리하는 함수

    3. 예외: 선택/하이라이트
선택은 게임 상태가 아니라 입력 피드백입니다.
그래서 이건 모델 이벤트가 아니어도 됩니다.
     */

    public class GameSession : ISessionTransition, IGameInputReceiver, IGameResultContext
    {
        #region public Field F
        public GameSession(
            GameSessionInfo    sessionInfo,
            MatchView          matchView,
            MatchModel         matchModel,
            ReplayView         replayView,
            IPlayerController  cho, IPlayerController  han,
            IInputHandler      localInput)
        {
            _matchView    = matchView;
            _sessionInfo  = sessionInfo;
            _matchModel   = matchModel;
            _replayView   = replayView;
            _playerCho    = cho;
            _playerHan    = han;
            _localInput   = localInput;
            _states       = CreateStates();
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

        public void BindEvents()
        {
            // 슬슬 이벤트 버스.
            _matchModel.BindEvents();
            _matchView.BindUI(_matchModel);

            var events = _matchModel.MatchEvent;
            events.OnPieceMoved    += OnPieceMoved;
            events.OnCheckOccurred += OnCheckOccured;
            events.OnCheckReleased += OnCheckReleased;
            events.OnGameEnded     += OnGameEnded;
            events.OnTurnChanged   += OnTurnChanged;

            _playerCho.BindEvents(this);
            _playerHan.BindEvents(this);
        }
        public void UnBindEvents()
        {
            _matchModel.UnBindEvents();
            _matchView.UnBindUI(_matchModel);

            var events = _matchModel.MatchEvent;
            events.OnPieceMoved    -= OnPieceMoved;
            events.OnCheckOccurred -= OnCheckOccured;
            events.OnCheckReleased -= OnCheckReleased;
            events.OnTurnChanged   -= OnTurnChanged;
            events.OnGameEnded     -= OnGameEnded;

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

        public GameResultInfo? GameResult { get; private set; }

        #endregion

        #region private Field F
        // Events
        private void OnPieceMoved(MoveContext moveCtx)
        => _states[_currState].OnPieceMoved(in moveCtx);
        private void OnCheckReleased()
            => _states[_currState].OnCheckReleased();
        private void OnCheckOccured(PlayerTeam team)
            => _states[_currState].OnCheckOccurred(team);
        private void OnTurnChanged(PlayerTeam next)
            => _states[_currState].OnTurnChanged(next);
        private void OnGameEnded(GameResultInfo info)
        {
            GameResult = info; 
            _states[_currState].OnGameEnded(in info);
        }
        // Player
        public void  RequestMove(Pos from, Pos to)
            => _states[_currState].RequestMove(from, to);
        public void  ChangeSelection(int? pieceId, IReadOnlyList<Pos> legal, IReadOnlyList<Pos> illegal)
            => _states[_currState].OnSelectionChanged(pieceId, legal, illegal);
        // UI
        public void  StepForward()
            => _states[_currState].RequestStepForward();
        public void  StepBackward()
            => _states[_currState].RequestStepBackward();
        public void  Handicap()
            => _states[_currState].RequestHandicap();
        public void  GiveUp()
            => _states[_currState].RequestGiveUp();
        public void  UnDo()
            => _states[_currState].RequestUndo();
        public void  ResetGame()
        {
            GameResult = null;
            _states[_currState].RequestResetGame(_sessionInfo, _matchModel, _matchView, _replayView);
        }
        #endregion

        #region State
        private Dictionary<SessionState, ISessionState> CreateStates()
        {
            var states = new Dictionary<SessionState, ISessionState>();
            states[SessionState.Live]   = new SessionLiveState(this, _matchModel, _playerCho, _playerHan, _matchView);
            states[SessionState.Replay] = new SessionReplayState(this, _matchModel, _playerCho, _playerHan, _replayView, _matchView);
            states[SessionState.End]    = new SessionEndState(this, this, _playerCho, _playerHan, _matchModel, _matchView);
            states[SessionState.EndReplay] = new SessionEndReplayState(this, _playerCho, _playerHan, _matchModel, _matchView, _replayView);
            return states;
        }
        private void ChangeState(SessionState next)
        {
            if (_currState == next)
                return;

            if (_states.TryGetValue(_currState, out var curr))
                curr.Exit();

            _currState = next;
            _states[_currState].Enter();
        }
        public void ToLive()
        {
            ChangeState(SessionState.Live);
            _localInput.Activate();
        }
        public void ToReplay()
        {
            _localInput.Deactivate();
            ChangeState(SessionState.Replay);
        }
        public void ToEnd()
        {
            _localInput.Deactivate();
            ChangeState(SessionState.End);
        }
        public void ToEndReplay()
            => ChangeState(SessionState.EndReplay);
        #endregion
    }
}
