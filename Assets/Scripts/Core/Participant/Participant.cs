

namespace Yujanggi.Core.Participant
{
    using Domain;
    using System;

    public interface IParticipantController
    {
        public event Action<Pos> OnBoardClicked;
        public bool CanInput { get; }
        public void SetInputEnabled(bool enabled);
    }

    public class Participant
    {
        public PlayerTeam Team { get; }
        public IParticipantController Controller { get; private set; }

        public Participant(PlayerTeam team)
            => Team = team;
        public void Bind(IParticipantController controller)
        {
            Controller = controller;
            if (Team == PlayerTeam.Cho)
                Controller.SetInputEnabled(true);
            else 
                Controller.SetInputEnabled(false);
        }

    }
}