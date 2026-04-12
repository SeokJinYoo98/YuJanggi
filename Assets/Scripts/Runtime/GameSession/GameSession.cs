using System;

using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;
using Yujanggi.Runtime.Controller;
using Yujanggi.Runtime.Input;

namespace Yujanggi.Runtime.GameSession
{
    using Game;
    using System.Collections;
    using UnityEngine;

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
            events.OnCheckOccurred    += manager.HandleCheckOccured;
            events.OnCheckReleased    += manager.HandleCheckReleased;
            events.OnGameEnded        += manager.HandleGameEnded;

            events.OnPieceMoved       += manager.HandlePieceMoved;

            _cho.OnMoveRequest += Match.TryMove;
            _han.OnMoveRequest += Match.TryMove;
        }
        public void UnBindEvents(GameManager manager)
        {
            _cho.UnBindEvents(manager);
            _han.UnBindEvents(manager);

            Match.UnBindEvents();

            var turn = Match.Turn;
            var events = Match.MatchEvent;
            turn.OnTurnChanged        -= manager.HandleTurnChanged;
            events.OnCheckOccurred    -= manager.HandleCheckOccured;
            events.OnCheckReleased    -= manager.HandleCheckReleased;
            events.OnPieceMoved       -= manager.HandlePieceMoved;
            events.OnGameEnded        -= manager.HandleGameEnded;

            _cho.OnMoveRequest -= Match.TryMove;
            _han.OnMoveRequest -= Match.TryMove;
        }
        public MatchManager Match { get; }

        private readonly IPlayerController   _cho;
        private readonly IPlayerController   _han;
        private readonly GameSessionInfo     _info;

        public IPlayerController GetPlayer(PlayerTeam team)
            => team == PlayerTeam.Cho ? _cho : _han;

        public GameSession(GameSessionInfo info, PcInputHandler localInput, ICoroutineRunner runner)
        {
            _info = info;
            Match = new(info.TurnTime);
            SetCamera(localInput);

            _cho = CreateController(_info.Cho, PlayerTeam.Cho, localInput, Match, runner);
            _han = CreateController(_info.Han, PlayerTeam.Han, localInput, Match, runner);
        }
        public IPlayerController BeginNextTurn(PlayerTeam turn)
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
        public          GameResultInfo GiveUp()
        {
            DisableAllControllers();
            return Match.GiveUp();
        }
        public void     DisableAllControllers()
        {
            _cho.EndTurn();
            _han.EndTurn();
        }
        public void     StartGame()
        {

            Match.StartGame(_info.ChoFormation, _info.HanFormation);
            _cho.BeginTurn();
            _han.EndTurn();

        }
        public void     ResetGame()
        {
            _cho.BeginTurn();
            _han.EndTurn();
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
               MatchManager match,
               ICoroutineRunner runner)
        {
            return type switch
            {
                PlayerType.Local    => new LocalController(match.Rule, match.Board, team, input),
                PlayerType.AI       => new AIController(match.Rule, match.Board, team, runner),
                _                   => throw new ArgumentOutOfRangeException()
            };
        }
    }

}
