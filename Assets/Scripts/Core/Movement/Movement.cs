using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public enum StepResult
    {
        Block, // 이동 불가
        Empty, // 이동 가능
        Enemy  // 이동 가능
    }

    public abstract class Movement 
    {
        public abstract List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z);

        protected StepResult CheckCell(IBoardState board, PlayerType team, int dx, int dz)
        {
            if (!board.BoundaryCheck(dx, dz))
                return StepResult.Block;

            if (!board.IsTherePiece(dx, dz, out var pieceTeam))
                return StepResult.Empty;

            if (pieceTeam == team)
                return StepResult.Block;

            return StepResult.Enemy;
        }

        protected int Forward(PlayerType team)
            => team == PlayerType.Cho ? 1 : -1;
    }
    public class SoldierMovement : Movement
    {
        public override List<(int x, int z)> FindWays(
            IBoardState board, 
            PlayerType team, 
            int x, int z)
        {
            List<(int, int)> ways = new();

            if (CheckCell(board, team, x + 1, z) != StepResult.Block)
                ways.Add((x + 1, z));
            if (CheckCell(board, team, x - 1, z) != StepResult.Block)
                ways.Add((x - 1, z));
            if (CheckCell(board, team, x, z + Forward(team)) != StepResult.Block)
                ways.Add((x, z + Forward(team)));

            return ways;
        }
    }
}