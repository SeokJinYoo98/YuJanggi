namespace Yujanggi.Core.Movement
{
    public class PalaceMovement : PatternMovement
    {
        public PalaceMovement()
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
