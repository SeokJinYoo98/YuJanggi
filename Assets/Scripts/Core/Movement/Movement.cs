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
        Enemy  // 이동 가능
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

            if (!board.IsTherePiece(dx, dz, out var pieceTeam))
                return StepResult.Empty;

            if (pieceTeam == team)
                return StepResult.Block;

            return StepResult.Enemy;
        }
        protected StepResult CheckCell(IBoardState board, PlayerType team, (int dx, int dz) point)
            => CheckCell(board, team, point.dx, point.dz);
        protected int Forward(PlayerType team)
            => team == PlayerType.Cho ? 1 : -1;
    }

    public class MovementRule
    {
        Dictionary<PieceType, Movement> _rules;

        public MovementRule()
        {
            _rules = new()
            {
                {PieceType.Soldier, new SoldierMovement() },
                {PieceType.Chariot, new ChariotMovement() }
            };
        }

        public List<(int x, int z)> CandidateWays(
            IBoardState board,
            TurnInfo info)
        {
            if (!_rules.TryGetValue(info.Piece.Type, out var rule))
                return new List<(int, int)>();

            return rule.FindWays(board, info.Player, info.x, info.z);
        }
    }
}