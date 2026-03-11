using System.Collections.Generic;

using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

using UnityEngine;

namespace Yujanggi.Core.Movement
{
    public enum Step
    { Right, Left, Up, Down, RightUp, RightDown, LeftUp, LeftDown }
    public enum StepResult
    { Block, Empty, Enemy, Team }
    public abstract class Movement
    {
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
        protected void ProcessDirection(
            List<(int x, int z)> ways,
            IBoardState board,
            PlayerType team,
            int x,
            int z,
            Step[] steps)
        {
            int dx = x;
            int dz = z;

            int len = steps.Length;

            for (int j = 0; j < len; ++j)
            {
                ApplyStep(steps[j], team, ref dx, ref dz);

                if (j < len - 1)
                {
                    if (IsBlocked(board, team, dx, dz))
                        return;
                }
                else
                {
                    if (CanLand(board, team, dx, dz))
                        ways.Add((dx, dz));
                }
            }
        }
        
        protected bool IsBlocked(IBoardState board, PlayerType team, int dx, int dz)
        {
            var result = CheckCell(board, team, dx, dz);
            return result != StepResult.Empty;
        }
        protected bool CanLand(IBoardState board, PlayerType team, int dx, int dz)
        {
            var result = CheckCell(board, team, dx, dz);
            return (result == StepResult.Empty) || (result == StepResult.Enemy);
        }
        protected void ApplyStep(Step step, PlayerType team, ref int x, ref int z)
        {
            var dir = GetDir(step, team);
            x += dir.x;
            z += dir.z;
        }
        private int Forward(PlayerType team)
            => team == PlayerType.Cho ? 1 : -1;
        private (int x, int z) GetDir(Step step, PlayerType team)
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

            if (pieceTeam == team) return StepResult.Team;

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