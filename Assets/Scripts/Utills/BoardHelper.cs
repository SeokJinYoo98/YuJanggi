using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Movement;

namespace Yujanggi.Utills.Board
{
    public static class BoardHelper
    {
        public static StepResult CheckCell(
            IBoardModel board,
            PlayerTeam team,
            Pos pos)
        {
            if (!board.IsInside(pos))
                return StepResult.Block;

            if (!board.HasPiece(pos))
                return StepResult.Empty;

            if (board.GetPiece(pos).Team == team)
                return StepResult.Team;

            return StepResult.Enemy;
        }

        public static Vector3 ToVector3(Pos pos, float y)
            => new Vector3(pos.X, y, pos.Z);
    }
}
