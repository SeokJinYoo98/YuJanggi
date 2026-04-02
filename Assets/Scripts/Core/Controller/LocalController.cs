using System;

using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Controller
{
    public class LocalController : IPlayerController, ILocalPlayer
    {
        public PlayerTeam Team { get; }
        private readonly IInputHandler _input;

        public void SetInputEnabled(bool enabled)
            => _input.SetInputEnabled(enabled);

        public void BindEvents(IGameManager manager)
        {
            _input.OnBoardClicked += manager.HandleClick;
        }

        public void UnBindEvents(IGameManager manager)
        {
            _input.OnBoardClicked -= manager.HandleClick;
        }

        public LocalController(IInputHandler input, PlayerTeam team)
        {
            Team = team;
            _input = input;   
        }
    }
}