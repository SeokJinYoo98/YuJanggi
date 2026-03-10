namespace Yujanggi.Core.Rule
{
    using System;
    using Domain;
    using System.Collections.Generic;
    using Yujanggi.Core.Board;

    public class JanggiRule
    {
        Dictionary<PieceType, Func<TurnInfo, BoardState, List<(int x, int z)>>> _moveRules;
        Dictionary<PieceType, List<(int, int)>> _ways;
        public JanggiRule()
        {
            _moveRules = new()
            {
                //{PieceType.Soldier, CalcSoldier }
            };
            _ways = new();
            _ways[PieceType.Soldier] = new List<(int x, int z)>
            {
                (+0, 1),
                (-1, 0),
                (+1, 0)
            };
        }

    }
}