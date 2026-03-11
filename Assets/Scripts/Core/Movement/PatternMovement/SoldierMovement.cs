namespace Yujanggi.Core.Movement
{
    public class SoldierMovement : PatternMovement
    {
        public SoldierMovement()
        {
            _steps = new Step[][]
            {
                new Step[] { Step.Up },
                new Step[] { Step.Left },
                new Step[] { Step.Right }
            };
        }
    }
}