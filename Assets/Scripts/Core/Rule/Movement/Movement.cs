using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    using Utills.Board;
    public enum Step
    { Right, Left, Up, Down, RightUp, RightDown, LeftUp, LeftDown }
    public enum StepResult
    { Block, Empty, Enemy, Team }
    public abstract class Movement
    {
        // 나중에 누가 플레이언지 체크
        private int Forward(PlayerType team, PlayerType bottom)
        {

            return team == bottom ? 1 : -1;
        }
            

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
        protected void ApplyStep(
            Step step,
            ref int x, ref int z)
        {
            var dir = GetDir(step);
            x += dir.x;
            z += dir.z;
        }
        protected (int x, int z) GetDir(Step step)
        {
            var dir = Dirs[(int)step];
            return dir;
        }
        protected void ApplyStep(
            Step step, 
            PlayerType team,
            PlayerType bottom,
            ref int x, ref int z)
        {
            var dir = GetDir(step, team, bottom);
            x += dir.x;
            z += dir.z;
        }
        protected (int x, int z) GetDir(Step step, PlayerType team, PlayerType bottom)
        {
            var dir = Dirs[(int)step];
            dir.z *= Forward(team, bottom);
            return dir;
        }

 
        public abstract List<(int x, int z)> FindWays(
                IBoardState board, 
                PlayerType team, 
                int x, int z);
        protected StepResult CheckCell(
                IBoardState board, 
                PlayerType team, 
                int dx, int dz)
            => BoardHelper.CheckCell(board, team, dx, dz);
    }
}