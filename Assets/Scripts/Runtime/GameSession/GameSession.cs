using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;


namespace Yujanggi.Runtime.GameSession
{
    using Replay;
    public class GameSession
    {
        #region public Field F
        public GameSession(
            GameSessionInfo  sessionInfo,
            GameSessionPresenter  sessionView,
            MatchManager     sessionMatch,
            IPlayerController cho,
            IPlayerController han)
        {
            _sessionPresenter        = sessionView;
            _sessionInfo        = sessionInfo;
            _sessionMatch       = sessionMatch;
            _playerCho          = cho;
            _playerHan          = han;
            _sessionReplay      = new ReplayManager(_sessionMatch.Record);
        }
        public void BindEvents()
        {
            BindReplayEvents();
            _sessionMatch.BindEvents();

            var events = _sessionMatch.MatchEvent;
            _sessionPresenter.BindLiveEvents(events, _playerCho, _playerHan);
            _sessionPresenter.BindUI(_sessionMatch);

            events.OnGameEnded        += HandleGameEnded;
            events.OnTurnChanged      += HandleTurnChanged;

            _playerCho.BindEvents();
            _playerHan.BindEvents();
            _playerCho.OnMoveRequest  += HandleTryMove;
            _playerHan.OnMoveRequest  += HandleTryMove;
        }
        public void UnBindEvents()
        {
            UnBindReplayEvents();
            _sessionMatch.UnBindEvents();

            var events  = _sessionMatch.MatchEvent;
            _sessionPresenter.UnBindLiveEvents(events, _playerCho, _playerHan);
            _sessionPresenter.UnBindUI(_sessionMatch);

            events.OnTurnChanged      -= HandleTurnChanged;
            events.OnGameEnded        -= HandleGameEnded;

            _playerCho.UnBindEvents();
            _playerHan.UnBindEvents();
            _playerCho.OnMoveRequest -= HandleTryMove;
            _playerHan.OnMoveRequest -= HandleTryMove;
        }
        public void Handicap()
            => _sessionMatch.Handicap();
        public void GiveUp()
        {
            DisableAllControllers();
            var info = _sessionMatch.GiveUp();
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
            StartGame();
            var board = _sessionMatch.Board;
            _sessionPresenter.ResetGame(board);
        }
        public void UnDo()
        {
            if (!_sessionMatch.TryUnDo(out var ctx)) return;
            if (ctx.IsHandicap)
                return;
            _sessionPresenter.UnDo(in ctx);
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
        private readonly ReplayManager          _sessionReplay;
        #endregion

        #region private Field F
        private void              HandleTryMove(Pos from, Pos to)
        {
            _sessionMatch.TryMove(from, to);
        }
        private void              HandleTurnChanged(PlayerTeam next)
        {
            var nextPlayer = BeginNextTurn(next);
            bool isLocal = nextPlayer is ILocalPlayer;
            _sessionPresenter.OnTurnChanged(next, isLocal);
        }
        private void              HandleGameEnded(GameResultInfo info)
        {
            DisableAllControllers();
            var loserIsLocal = GetPlayer(info.Loser).IsLocal();
            _sessionPresenter.OnGameEnded(loserIsLocal, in info);
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
        public void StepForward()
        {

        }
        public void StepBackward()
        {

        
        }
        private void HandleEnterReplay()
        {
            DisableAllControllers();

        }
        private void HandleExitReplay()
        {

        }
        #endregion
    }
}
