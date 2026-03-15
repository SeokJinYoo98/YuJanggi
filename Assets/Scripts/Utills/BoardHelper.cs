using UnityEngine;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Movement;

namespace Yujanggi.Utills.Board
{
    public static class BoardHelper
    {
        public static StepResult CheckCell(
            IBoardState board,
            PlayerType team,
            Pos pos)
        {
            if (!board.BoundaryCheck(pos))
                return StepResult.Block;

            if (!board.IsTherePiece(pos, out var piece))
                return StepResult.Empty;

            if (piece.Team == team)
                return StepResult.Team;

            return StepResult.Enemy;
        }

        public static bool IsBottomPlayer(IBoardState board, PlayerType team)
            => board.BottomPlayer == team;
        public static Vector3 ToVector3(Pos pos, float y)
            => new Vector3(pos.X, y, pos.Z);
    }
}
