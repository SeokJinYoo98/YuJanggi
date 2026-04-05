using System;

using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Runtime.Controller;
using Yujanggi.Runtime.GameMode;
using Yujanggi.Runtime.Input;

namespace Yujanggi.Runtime.GameSession
{
    using Game;
    public enum GameModeType
    {
        Local, AI, Network
    }
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
        public void BindEvents(GameManager manager)
        {
            _cho.BindEvents(manager);
            _han.BindEvents(manager);

            Match.BindEvents();
            var turn = Match.Turn;
            var events = Match.MatchEvent;
            turn.OnTurnChanged        += manager.HandleTurnChanged;
            events.OnSelectionChanged += manager.HandleSelectionChanged;
            events.OnCheckOccurred    += manager.HandleCheckOccured;
            events.OnCheckReleased    += manager.HandleCheckReleased;
            events.OnPieceMoved       += manager.HandlePieceMoved;
            events.OnGameEnded        += manager.HandleGameEnded;
        }
        public void UnBindEvents(GameManager manager)
        {
            _cho.UnBindEvents(manager);
            _han.UnBindEvents(manager);

            Match.UnBindEvents();

            var turn = Match.Turn;
            var events = Match.MatchEvent;
            turn.OnTurnChanged        -= manager.HandleTurnChanged;
            events.OnSelectionChanged -= manager.HandleSelectionChanged;
            events.OnCheckOccurred    -= manager.HandleCheckOccured;
            events.OnCheckReleased    -= manager.HandleCheckReleased;
            events.OnPieceMoved       -= manager.HandlePieceMoved;
            events.OnGameEnded        -= manager.HandleGameEnded;
        }
        public MatchManager Match { get; }

        private readonly IPlayerController   _cho;
        private readonly IPlayerController   _han;
        private readonly GameSessionInfo _info;

        public IPlayerController GetPlayer(PlayerTeam team)
            => team == PlayerTeam.Cho ? _cho : _han;

        public GameSession(GameSessionInfo info, PcInputHandler localInput)
        {
            _info = info;
            Match = new(info.TurnTime);
            SetCamera(localInput);

            _cho = CreateController(_info.Cho, PlayerTeam.Cho, localInput, Match);
            _han = CreateController(_info.Han, PlayerTeam.Han, localInput, Match);
        }
        public IPlayerController BeginNextTurn(PlayerTeam turn)
        {
            if (turn == PlayerTeam.Cho)
            {
                _han.SetInputEnabled(false);
                _cho.SetInputEnabled(true);
                return _cho;
            }
            _cho.SetInputEnabled(false);
            _han.SetInputEnabled(true);
            return _han;
        }
        public          GameResultInfo GiveUp()
        {
            DisableAllControllers();
            return Match.GiveUp();
        }
        public void     DisableAllControllers()
        {
            _cho.SetInputEnabled(false);
            _han.SetInputEnabled(false);
        }
        public void     StartGame()
        {
            _han.SetInputEnabled(false); _cho.SetInputEnabled(true);
            Match.StartGame(_info.ChoFormation, _info.HanFormation);
        }
        public void     ResetGame()
        {
            _han.SetInputEnabled(false); _cho.SetInputEnabled(true);
            Match.ResetGame(_info.ChoFormation, _info.HanFormation);
        }
        private void    SetCamera(PcInputHandler localInput)
        {
            if (_info.Mode == GameModeType.Local) return;

            if (_info.Cho == PlayerType.Local) return;

            localInput.RotateCamera(PlayerTeam.Han);
        }
        private static IPlayerController CreateController(
               PlayerType type,
               PlayerTeam team,
               PcInputHandler input,
               MatchManager match)
        {
            return type switch
            {
                PlayerType.Local    => new LocalController(input, team),
                PlayerType.AI       => new AIController(match.Rule, match.Board, team),
                _                   => throw new ArgumentOutOfRangeException()
            };
        }
    }

}
