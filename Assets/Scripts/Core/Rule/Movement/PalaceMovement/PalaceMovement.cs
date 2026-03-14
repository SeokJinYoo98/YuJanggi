

using Mono.Cecil.Cil;
using System;
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

                case PieceType.Cannon:
                    Cannon(board, team, x, z, ways);
                    break;
                default:
                    break;
            }

            return ways;
        }
        private void Default(IBoardState board, PlayerType team, int x, int z, List<(int, int)> ways)
        {
            ProcessPalaceMove(board, team, x, z, ways, DefaultStep);
        }

        private void Chariot(IBoardState board, PlayerType team, int x, int z, List<(int, int)> ways)
        {
            ProcessPalaceMove(board, team, x, z, ways, ChariotStep);
        }

        private void Cannon(IBoardState board, PlayerType team, int x, int z, List<(int, int)> ways)
        {
            ProcessPalaceMove(board, team, x, z, ways, CannonStep);
        }
        private void ProcessPalaceMove(
            IBoardState board,
            PlayerType team,
            int x,
            int z,
            List<(int x, int z)> ways,
            Func<IBoardState, PlayerType, Step, int, int, List<(int, int)>, bool> handler)
        {
            if (!_palaceLinks.TryGetValue((x, z), out var steps))
                return;

            foreach (var step in steps)
            {
                handler(board, team, step, x, z, ways);
            }
        }
        private bool DefaultStep(
            IBoardState board,
            PlayerType team,
            Step step,
            int x,
            int z,
            List<(int, int)> ways)
        {
            int dx = x;
            int dz = z;

            ApplyStep(step, ref dx, ref dz);

            var result = CheckCell(board, team, dx, dz);

            if (result == StepResult.Empty || result == StepResult.Enemy)
                ways.Add((dx, dz));

            return true;
        }
        private bool ChariotStep(
            IBoardState board,
            PlayerType team,
            Step step,
            int x,
            int z,
            List<(int, int)> ways)
        {
            int dx = x;
            int dz = z;

            ApplyStep(step, ref dx, ref dz);

            switch (CheckCell(board, team, dx, dz))
            {
                case StepResult.Empty:
                    ways.Add((dx, dz));

                    ApplyStep(step, ref dx, ref dz);

                    if (!board.IsPalace(dx, dz))
                        return true;

                    var result = CheckCell(board, team, dx, dz);

                    if (result == StepResult.Empty || result == StepResult.Enemy)
                        ways.Add((dx, dz));

                    break;

                case StepResult.Enemy:
                    ways.Add((dx, dz));
                    break;
            }

            return true;
        }
        private bool CannonStep(
            IBoardState board,
            PlayerType team,
            Step step,
            int x,
            int z,
            List<(int, int)> ways)
        {
            int dx = x;
            int dz = z;

            ApplyStep(step, ref dx, ref dz);

            switch (CheckCell(board, team, dx, dz))
            {
                case StepResult.Team:
                case StepResult.Enemy:

                    ApplyStep(step, ref dx, ref dz);

                    if (!board.IsPalace(dx, dz))
                        return true;

                    var result = CheckCell(board, team, dx, dz);

                    if (result == StepResult.Empty || result == StepResult.Enemy)
                        ways.Add((dx, dz));

                    break;
            }

            return true;
        }
    }
}