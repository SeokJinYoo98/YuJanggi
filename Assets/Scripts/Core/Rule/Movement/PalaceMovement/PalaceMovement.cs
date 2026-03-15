using System;
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Movement
{
    public class PalaceMovement : Movement
    {
        private readonly Dictionary<Pos, Step[]> _palaceLinks;
        // 추가 대상


        public PalaceMovement()
        {
            int top = 7;
            _palaceLinks = new()
            {
                { new Pos (4, 1), new[]     { Step.LeftDown, Step.LeftUp, Step.RightDown, Step.RightUp }},
                { new Pos (3, 0), new[]     { Step.RightUp }},
                { new Pos (5, 0), new[]     { Step.LeftUp }},
                { new Pos (3, 2), new[]     { Step.RightDown }},
                { new Pos (5, 2), new[]     { Step.LeftDown }},

                { new Pos (4, 1+top), new[] { Step.LeftDown, Step.LeftUp, Step.RightDown, Step.RightUp }},
                { new Pos (3, 0+top), new[] { Step.RightUp   }},
                { new Pos (5, 0+top), new[] { Step.LeftUp    }},
                { new Pos (3, 2+top), new[] { Step.RightDown  }},
                { new Pos (5, 2+top), new[] { Step.LeftDown }}
            };
        }
        public override List<Pos> FindWays(IBoardState board, SelectionState selectInfo)
        {
            List<Pos> ways = new(); 
            var selectedPiece   = selectInfo.SelectedPiece;
            var team            = selectedPiece.Team;
            var pos             = selectInfo.SelectedPos;

            switch (selectedPiece.Type)
            {
                case PieceType.Soldier:
                case PieceType.King:
                case PieceType.Guard:
                    Default(board, team, pos, ways);
                    break;

                case PieceType.Chariot:
                    Chariot(board, team, pos, ways);
                    break;

                case PieceType.Cannon:
                    Cannon(board, team, pos, ways);
                    break;
                default:
                    break;
            }

            return ways;
        }
        private void Default(IBoardState board, PlayerTeam team, Pos pos, List<Pos> ways)
        {
            ProcessPalaceMove(board, team, pos, ways, DefaultStep);
        }

        private void Chariot(IBoardState board, PlayerTeam team, Pos pos, List<Pos> ways)
        {
            ProcessPalaceMove(board, team, pos, ways, ChariotStep);
        }

        private void Cannon(IBoardState board, PlayerTeam team, Pos pos, List<Pos> ways)
        {
            ProcessPalaceMove(board, team, pos, ways, CannonStep);
        }
        private void ProcessPalaceMove(
            IBoardState board,
            PlayerTeam team,
            Pos pos,
            List<Pos> ways,
            Func<IBoardState, PlayerTeam, Step, Pos, List<Pos>, bool> handler)
        {
            if (!_palaceLinks.TryGetValue(pos, out var steps))
                return;

            foreach (var step in steps)
            {
                handler(board, team, step, pos, ways);
            }
        }
        private bool DefaultStep(
            IBoardState board,
            PlayerTeam team,
            Step step,
            Pos pos,
            List<Pos> ways)
        {

            var dPos = ApplyStep(step, pos);

            var result = CheckCell(board, team, dPos);

            if (result == StepResult.Empty || result == StepResult.Enemy)
                ways.Add(dPos);

            return true;
        }
        private bool ChariotStep(
            IBoardState board,
            PlayerTeam team,
            Step step,
            Pos pos,
            List<Pos> ways)
        {
 

            var dPos = ApplyStep(step, pos);

            switch (CheckCell(board, team, dPos))
            {
                case StepResult.Empty:
                    ways.Add(dPos);

                    var ddPos = ApplyStep(step, dPos);

                    if (!board.IsPalace(ddPos))
                        return true;

                    var result = CheckCell(board, team, ddPos);

                    if (result == StepResult.Empty || result == StepResult.Enemy)
                        ways.Add(ddPos);

                    break;

                case StepResult.Enemy:
                    ways.Add(dPos);
                    break;
            }

            return true;
        }
        private bool CannonStep(
            IBoardState board,
            PlayerTeam team,
            Step step,
            Pos pos,
            List<Pos> ways)
        {
            var dPos = ApplyStep(step, pos);

            switch (CheckCell(board, team, dPos))
            {
                case StepResult.Team:
                case StepResult.Enemy:

                    dPos = ApplyStep(step, dPos);

                    if (!board.IsPalace(dPos))
                        return true;

                    var result = CheckCell(board, team, dPos);

                    if (result == StepResult.Empty || result == StepResult.Enemy)
                        ways.Add((dPos));

                    break;
            }

            return true;
        }
    }
}