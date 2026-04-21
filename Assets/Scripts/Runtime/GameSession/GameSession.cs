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
            GameSessionInfo sessionInfo,
            GameSessionView sessionView,
            MatchManager sessionMatch,
            PcInputHandler localInput, 
            ICoroutineRunner runner)
        {
            _sessionView = sessionView;
            _info        = sessionInfo;
            _match       = sessionMatch;
            SetCamera(localInput);

            _cho = CreateController(_info.Cho, PlayerTeam.Cho, localInput, _match, runner);
            _han = CreateController(_info.Han, PlayerTeam.Han, localInput, _match, runner);
        }
        
        public void              BindEvents()
        {
            _match.BindEvents();
            _sessionView.BindEvents(_match);
            _cho.BindEvents(_sessionView);
            _han.BindEvents(_sessionView);

            var events = _match.MatchEvent;
            var turn = _match.Turn;
            turn.OnTurnChanged        += HandleTurnChanged;
            events.OnGameEnded        += HandleGameEnded;

            _cho.OnMoveRequest += _match.TryMove;
            _han.OnMoveRequest += _match.TryMove;
        }
        public void              UnBindEvents()
        {
            _match.UnBindEvents();
            _sessionView.UnBindEvents(_match);
            _cho.UnBindEvents(_sessionView);
            _han.UnBindEvents(_sessionView);

            var events  = _match.MatchEvent;
            var turn    = _match.Turn; 
            turn.OnTurnChanged        -= HandleTurnChanged;
            events.OnGameEnded        -= HandleGameEnded;

            _cho.OnMoveRequest -= _match.TryMove;
            _han.OnMoveRequest -= _match.TryMove;
        }
        public void              Handicap()
            => _match.Handicap();
        public void              GiveUp()
        {
            DisableAllControllers();
            var info = _match.GiveUp();
            _sessionView.ShowResultUI(in info);
        }
        public void              StartGame()
        {
            _match.StartGame(_info.ChoFormation, _info.HanFormation);
            _cho.BeginTurn();
            _han.EndTurn();
            _sessionView.StartGame(_match.Board);
        }
        public void              ResetGame()
        {
            StartGame();
            var board = _match.Board;
            _sessionView.ResetGame(board);
        }
        public void              UnDo()
        {
            if (!_match.TryUnDo(out var ctx)) return;
            if (ctx.IsHandicap)
                return;
            _sessionView.UnDo(in ctx);
        }
        public void              Update(float deltaTime)
            => _match.Update(deltaTime);

        private readonly MatchManager       _match;
        private readonly IPlayerController  _cho;
        private readonly IPlayerController  _han;
        private readonly GameSessionInfo    _info;
        private readonly GameSessionView    _sessionView;
        private IPlayerController BeginNextTurn(PlayerTeam turn)
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
            _match.Turn.SetTurn(TurnType.End);
            DisableAllControllers();
        }
        private IPlayerController GetPlayer(PlayerTeam team)
            => team == PlayerTeam.Cho ? _cho : _han;
        private void              DisableAllControllers()
        {
            _cho.EndTurn(); _han.EndTurn();
        }
        private void              SetCamera(PcInputHandler localInput)
        {
            if (_info.Mode == GameModeType.Local) return;
            if (_info.Cho  == PlayerType.Local) return;

            localInput.RotateCamera(PlayerTeam.Han);
        }
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
