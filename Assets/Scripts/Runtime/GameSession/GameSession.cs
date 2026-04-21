using System;

using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Runtime.Controller;
using Yujanggi.Runtime.Input;

namespace Yujanggi.Runtime.GameSession
{
    public struct GameSessionInfo
    {
        public GameModeType     Mode;
        public PlayerType       Cho;
        public Formation        ChoFormation;
        public PlayerType       Han;
        public Formation        HanFormation;
        public float            TurnTime;
    }
    public static class GameSessionStore
    {
        public static GameSessionInfo Current;
    }
    public class GameSession
    {
        public GameSession(
            GameSessionInfo  sessionInfo,
            GameSessionView  sessionView,
            MatchManager     sessionMatch,
            PcInputHandler   localInput, 
            ICoroutineRunner runner)
        {
            _sessionView = sessionView;
            _sessionInfo        = sessionInfo;
            _sessionMatch       = sessionMatch;

            SetCamera(localInput);

            _playerCho = CreateController(_sessionInfo.Cho, PlayerTeam.Cho, localInput, _sessionMatch, runner);
            _playerHan = CreateController(_sessionInfo.Han, PlayerTeam.Han, localInput, _sessionMatch, runner);
        }
        
        public void BindEvents()
        {
            _sessionMatch.BindEvents();
            _sessionView.BindEvents(_sessionMatch);
            _playerCho.BindEvents(_sessionView);
            _playerHan.BindEvents(_sessionView);

            var events = _sessionMatch.MatchEvent;
            var turn = _sessionMatch.Turn;
            turn.OnTurnChanged        += HandleTurnChanged;
            events.OnGameEnded        += HandleGameEnded;

            _playerCho.OnMoveRequest += _sessionMatch.TryMove;
            _playerHan.OnMoveRequest += _sessionMatch.TryMove;
        }
        public void UnBindEvents()
        {
            _sessionMatch.UnBindEvents();
            _sessionView.UnBindEvents(_sessionMatch);
            _playerCho.UnBindEvents(_sessionView);
            _playerHan.UnBindEvents(_sessionView);

            var events  = _sessionMatch.MatchEvent;
            var turn    = _sessionMatch.Turn; 
            turn.OnTurnChanged        -= HandleTurnChanged;
            events.OnGameEnded        -= HandleGameEnded;

            _playerCho.OnMoveRequest -= _sessionMatch.TryMove;
            _playerHan.OnMoveRequest -= _sessionMatch.TryMove;
        }
        public void Handicap()
            => _sessionMatch.Handicap();
        public void GiveUp()
        {
            DisableAllControllers();
            var info = _sessionMatch.GiveUp();
            _sessionView.ShowResultUI(in info);
        }
        public void StartGame()
        {
            _sessionMatch.StartGame(_sessionInfo.ChoFormation, _sessionInfo.HanFormation);
            _playerCho.BeginTurn();
            _playerHan.EndTurn();
            _sessionView.StartGame(_sessionMatch.Board);
        }
        public void ResetGame()
        {
            StartGame();
            var board = _sessionMatch.Board;
            _sessionView.ResetGame(board);
        }
        public void UnDo()
        {
            if (!_sessionMatch.TryUnDo(out var ctx)) return;
            if (ctx.IsHandicap)
                return;
            _sessionView.UnDo(in ctx);
        }
        public void Update(float deltaTime)
            => _sessionMatch.Update(deltaTime);

        private readonly IPlayerController  _playerCho;
        private readonly IPlayerController  _playerHan;
        private readonly GameSessionInfo    _sessionInfo;
        private readonly GameSessionView    _sessionView;
        private readonly MatchManager       _sessionMatch;
        private void              HandleTurnChanged(PlayerTeam next)
        {
            var nextPlayer = BeginNextTurn(next);
            bool isLocal = nextPlayer is LocalController;
            _sessionView.OnTurnChanged(next, isLocal);
        }
        private void              HandleGameEnded(GameResultInfo info)
        {
            EndGame();
            var isLocalWin = GetPlayer(info.Winner) is LocalController;
            _sessionView.OnGameEnded(isLocalWin, in info);
        }
        private void              EndGame()
        {
            DisableAllControllers();
            _sessionMatch.Turn.SetTurn(TurnType.End);
  
        }
        private void              DisableAllControllers()
        {
            _playerCho.EndTurn(); _playerHan.EndTurn();
        }
        private void              SetCamera(PcInputHandler localInput)
        {
            if (_sessionInfo.Mode == GameModeType.Local) return;
            if (_sessionInfo.Cho  == PlayerType.Local) return;

            localInput.RotateCamera(PlayerTeam.Han);
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
        private IPlayerController CreateController(
           PlayerType type,
           PlayerTeam team,
           PcInputHandler input,
           MatchManager match,
           ICoroutineRunner runner)
        {
            return type switch
            {
                PlayerType.Local => new LocalController(match.Rule, match.Board, team, input),
                PlayerType.AI => new AIController(match.Rule, match.Board, team, runner),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
