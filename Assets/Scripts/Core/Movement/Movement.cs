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
        public abstract void FindWays(IBoardState board, int x, int z, PlayerType team);

        protected StepResult CheckCell(IBoardState board, int dx, int dz, PlayerType team)
        {
            if (!board.BoundaryCheck(dx, dz))
                return StepResult.Block;

            if (!board.IsTherePiece(dx, dz, out var pieceTeam))
                return StepResult.Empty;

            if (pieceTeam == team)
                return StepResult.Block;

            return StepResult.Enemy;
        }
    }
    public class SoldierMovement : Movement
    {
        public override void FindWays(IBoardState board, int x, int z, PlayerType team)
        {
            board.ClearWays();

            if (CheckCell(board, x + 1, z, team) != StepResult.Block)
                board.AddWay(x + 1, z);
            if (CheckCell(board, x - 1, z, team) != StepResult.Block)
                board.AddWay(x - 1, z);
            if (CheckCell(board, x, z + 1, team) != StepResult.Block)
                board.AddWay(x, z + 1);

            board.PrintWays();
        }
    }
}