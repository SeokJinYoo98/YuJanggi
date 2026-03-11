using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public enum Step
    { Right, Left, Up, Down, RightUp, RightDown, LeftUp, LeftDown }
    public enum StepResult
    { Block, Empty, Enemy, Team }
    public abstract class Movement
    {
        private int Forward(PlayerType team)
            => team == PlayerType.Cho ? 1 : -1;

        protected static readonly (int x, int z)[] Dirs =
        {
            (+1, +0), // 우
            (-1, +0), // 좌
            (+0, +1), // 상
            (+0, -1), // 하
            (+1, +1), // 우상
            (+1, -1), // 우하
            (-1, +1), // 좌상
            (-1, -1)  // 좌하
        };

        protected void ApplyStep(Step step, PlayerType team, ref int x, ref int z)
        {
            var dir = GetDir(step, team);
            x += dir.x;
            z += dir.z;
        }
        protected (int x, int z) GetDir(Step step, PlayerType team)
        {
            var dir = Dirs[(int)step];
            dir.z *= Forward(team);
            return dir;
        }

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
        /// <summary>
        /// /////////////////////////////
        /// </summary>




        // 항상 배열 마지막만 체크

  


        public abstract List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z);
        protected StepResult CheckCell(IBoardState board, PlayerType team, (int dx, int dz) point)
            => CheckCell(board, team, point.dx, point.dz);
    }
}