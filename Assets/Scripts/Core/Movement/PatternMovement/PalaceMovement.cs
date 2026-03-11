namespace Yujanggi.Core.Movement
{
    public class PalaceMovement : PatternMovement
    {
        public PalaceMovement()
        {
            _steps = new Step[][]
            {
                new [] { Step.Up},
                new [] { Step.Down},
                new [] { Step.Left},
                new [] { Step.Right },
                new [] {Step.LeftUp},
                new [] {Step.LeftDown},
                new [] {Step.RightUp},
                new [] {Step.RightDown}
            };
        }
    }
}
