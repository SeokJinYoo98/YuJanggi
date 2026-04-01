

namespace Yujanggi.Core.Participant
{
    using Domain;
    using System;

    using Match;
    using Yujanggi.Core.AI;
    using Yujanggi.Runtime.Manager;
    using Yujanggi.Runtime.Input;

    public interface IAIController
    {
        public event Action<Pos, Pos> OnMoveRequest;
    }

    public interface IParticipantController
    {
        public bool CanInput { get; }
        public void SetInputEnabled(bool enabled);
    }

    public interface IParticipant
    {
        public PlayerTeam Team { get; }
        public PlayerType Type { get; }
    }
    public class Participant : IParticipant
    {
        public PlayerTeam               Team { get; }
        public PlayerType               Type { get; }
        public IParticipantController   Controller { get; private set; }

        public Participant(PlayerTeam team, PlayerType type)
        {
            Team = team;
            Type = type;
        }
        
        public void Init(IParticipantController controller)
        {
            Controller = controller;

            if (Team == PlayerTeam.Cho)
                Controller.SetInputEnabled(true);
            else 
                Controller.SetInputEnabled(false);
        }

        public void BindEvents(GameManager manager)
        {
            if (Controller == null) return;

            if (Controller is LocalPlayerController player)
                player.OnBoardClicked += manager.HandleClick;

            else if (Controller is AIController ai)
                ai.OnMoveRequest += manager.HandleMove;
        }
        public void UnBindEvents(GameManager manager)
        {
            if (Controller == null) return;

            if (Controller is LocalPlayerController player)
                player.OnBoardClicked -= manager.HandleClick;

            else if (Controller is AIController ai)
                ai.OnMoveRequest -= manager.HandleMove;
        }

    }
}