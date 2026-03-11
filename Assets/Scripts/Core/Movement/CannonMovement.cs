using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;

using UnityEngine;

namespace Yujanggi.Core.Movement
{
    public class CannonMovement : Movement
    {
        public override List<(int x, int z)> FindWays(IBoardState board, PlayerType team, int x, int z)
        {
            List<(int x, int z)> ways = new();
            for (int i = 0; i < Dirs.Length; ++i)
            {
                bool isFind = TryFindBridge(board, i, out var bridge, x, z);
                if (!isFind) continue;
                
                var way = SlideMove(board, i, bridge, team);
                ways.AddRange(way);
            }
            return ways;
        }
        private bool IsCannon(PieceType type)
            => type == PieceType.Cannon;
        private bool TryFindBridge(IBoardState board, int i, out (int x, int z) bridge, int x, int z)
        {
            bridge.x = bridge.z = -100;
            var dir = Dirs[i];
            int dx = x + dir.x;
            int dz = z + dir.z;

            while (board.BoundaryCheck(dx, dz))
            {
                if (board.IsTherePiece(dx, dz, out var _, out var type))
                {
                    if (IsCannon(type))
                        return false;

                    bridge.x = dx;
                    bridge.z = dz;
                    return true; // 브리지를 찾으면 바로 true 반환
                }

                dx += dir.x;
                dz += dir.z;
            }
            return false;
        }
        private List<(int x, int z)> SlideMove(IBoardState board, int i, (int x, int z) bridge, PlayerType team)
        {
            List<(int x, int z)> ways = new();
            var dir = Dirs[i];
            int dx = bridge.x;
            int dz = bridge.z;

            while (true)
            {
                dx += dir.x;
                dz += dir.z;

                var result = CheckCell(board, team, dx, dz);
                if (result == StepResult.Block || result == StepResult.Team)
                    break;

                ways.Add((dx, dz));
                if (result == StepResult.Enemy)
                {
                    if (IsCannon(board.GetPiece(dx, dz).Type))
                        ways.Remove((dx, dz));
                    break;
                }
                    
            }

            return ways;
        }
    }
}