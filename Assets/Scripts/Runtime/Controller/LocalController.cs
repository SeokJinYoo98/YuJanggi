using System;

using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Controller
{
    public class LocalController : IPlayerController, ILocalPlayer
    {
        public  PlayerTeam     Team { get; }
        private readonly IInputHandler  _input;

        public void SetInputEnabled(bool enabled)
            => _input.SetInputEnabled(enabled);

        public void BindEvents(IGameInputHandler manager)
        {
            //_input.OnBoardClicked += manager.HandleClickRequest;
        }

        public void UnBindEvents(IGameInputHandler manager)
        {
            //_input.OnBoardClicked -= manager.HandleClickRequest;
        }

        public LocalController(IInputHandler input, PlayerTeam team)
        {
            Team = team;
            _input = input;
        }
    }
}