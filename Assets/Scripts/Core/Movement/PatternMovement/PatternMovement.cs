using System.Collections.Generic;
using Unity.VisualScripting;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class PatternMovement : Movement
    {
        protected Step[][] _steps;
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            List<(int x, int z)> ways = new();

            foreach (var steps in _steps)
                ProcessDirection(ways, board, team, x, z, steps);

            return ways;
        }

    }
}
