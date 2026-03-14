namespace Yujanggi.Core.Movement
{
    public class KingGuardMovement : PatternMovement
    {
        public KingGuardMovement()
        {
            _steps = new Step[][]
            {
                new [] { Step.Up },
                new [] { Step.Down },
                new [] { Step.Left },
                new [] { Step.Right },
            };
        }
    }
}
