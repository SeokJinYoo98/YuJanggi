

using Mono.Cecil.Cil;
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class PalaceMovement : Movement
    {
        private readonly Dictionary<(int, int), Step[]> _palaceLinks;
        // 추가 대상


        public PalaceMovement()
        {
            int top = 7;
            _palaceLinks = new()
            {
                { (4, 1), new[]     { Step.LeftDown, Step.LeftUp, Step.RightDown, Step.RightUp }},
                { (3, 0), new[]     { Step.RightUp }},
                { (5, 0), new[]     { Step.LeftUp }},
                { (3, 2), new[]     { Step.RightDown }},
                { (5, 2), new[]     { Step.LeftDown }},

                { (4, 1+top), new[] { Step.LeftDown, Step.LeftUp, Step.RightDown, Step.RightUp }},
                { (3, 0+top), new[] { Step.RightUp   }},
                { (5, 0+top), new[] { Step.LeftUp    }},
                { (3, 2+top), new[] { Step.RightDown  }},
                { (5, 2+top), new[] { Step.LeftDown }}
            };
        }
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            List<(int x, int z)> ways = new();
            switch (board.GetPiece(x, z).Type)
            {
                case PieceType.Soldier:
                case PieceType.King:
                case PieceType.Guard:
                    Default(board, team, x, z, ways);
                    break;

                case PieceType.Chariot:
                    Chariot(board, team, x, z, ways);
                    break;
            }

            return ways;
        }


        private void Default(IBoardState board, PlayerType team, int x, int z, List<(int x, int z)> ways)
        {
            if (_palaceLinks.TryGetValue((x, z), out var steps))
            {
                foreach (var step in steps)
                {
                    int dx = x; int dz = z;
                    ApplyStep(step, ref dx, ref dz);
                    var result = CheckCell(board, team, dx, dz);

                    if (result == StepResult.Empty || result == StepResult.Enemy)
                        ways.Add((dx, dz));
                }
            }


        }
        private void Chariot(IBoardState board, PlayerType team, int x, int z, List<(int x, int z)> ways)
        {
            if (_palaceLinks.TryGetValue((x, z), out var steps))
            {
                foreach (var step in steps)
                {
                    int dx = x; int dz = z;
                    ApplyStep(step, ref dx, ref dz);
                    switch (CheckCell(board, team, dx, dz))
                    {
                        case StepResult.Empty:
                            ways.Add((dx, dz));
                            ApplyStep(step, ref dx, ref dz);
                            if (!board.IsPalace(dx, dz)) break;
                            var result = CheckCell(board, team, dx, dz);
                            if (result == StepResult.Empty || result == StepResult.Enemy)
                                ways.Add((dx, dz));
                            break;
                        case StepResult.Enemy:
                            ways.Add((dx, dz));
                            break;
                    }
                }
            }
        }
    }
}