using System.Collections.Generic;
using System.Drawing;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    /*
    ├ StepMovement   (한칸 이동)
    ├ SlideMovement  (직선 이동)
    ├ JumpMovement   (포) 
    ├ Dialog         (말, 상) 
    */

    public enum StepResult
    {
        Block, // 이동 불가
        Empty, // 이동 가능
        Enemy, // 이동 가능
        Team    
    }
    public abstract class Movement 
    {
        protected static readonly (int x, int z)[] Dirs =
        {
            (+1, +0),
            (-1, +0),
            (+0, +1),
            (+0, -1)
        };
        public abstract List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z);
        protected StepResult CheckCell(IBoardState board, PlayerType team, int dx, int dz)
        {
            if (!board.BoundaryCheck(dx, dz))
                return StepResult.Block;

            if (!board.IsTherePiece(dx, dz, out var pieceTeam, out var _))
                return StepResult.Empty;

            if (pieceTeam == team)
                return StepResult.Team;

            return StepResult.Enemy;
        }
        protected StepResult CheckCell(IBoardState board, PlayerType team, (int dx, int dz) point)
            => CheckCell(board, team, point.dx, point.dz);
        protected int Forward(PlayerType team)
            => team == PlayerType.Cho ? 1 : -1;
    }
    public class StepMovement : Movement
    {
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            throw new System.NotImplementedException();
        }
    }
    public class SlideMovement : StepMovement
    {
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            throw new System.NotImplementedException();
        }
    }
    public class DialogMovement : StepMovement
    {
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            throw new System.NotImplementedException();
        }
    }
}