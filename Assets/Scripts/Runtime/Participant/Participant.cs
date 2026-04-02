

namespace Yujanggi.Runtime.Participant
{
    using Core.Domain;
    using Yujanggi.Core.Controller;
    using Yujanggi.Runtime.Game;
    using Yujanggi.Runtime.Input;



    public class Participant
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