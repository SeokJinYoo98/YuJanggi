
using System;
using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Board
{
    internal class BoardInitializer
    {
        private static int PieceId = 0;
        public  static void SetUpPieces(IBoardModel board, PlayerTeam bottomPlayer)
        {
            PieceId = 0;
            var topPlayer = bottomPlayer == PlayerTeam.Cho ? PlayerTeam.Han : PlayerTeam.Cho;
            SpawnBottom(board, bottomPlayer);
            SpawnTop(board, topPlayer);
        }
        private static void SpawnBottom(IBoardModel board, PlayerTeam bottom)
        {
            Spawn(board, bottom, PieceType.Chariot, 0, 0);
            Spawn(board, bottom, PieceType.Chariot, 8, 0);

            Spawn(board, bottom, PieceType.Elephant, 1, 0);
            Spawn(board, bottom, PieceType.Elephant, 7, 0);

            Spawn(board, bottom, PieceType.Horse, 6, 0);
            Spawn(board, bottom, PieceType.Horse, 2, 0);

            Spawn(board, bottom, PieceType.Guard, 3, 0);
            Spawn(board, bottom, PieceType.Guard, 5, 0);

            Spawn(board, bottom, PieceType.King, 4, 1);

            Spawn(board, bottom, PieceType.Cannon, 1, 2);
            Spawn(board, bottom, PieceType.Cannon, 7, 2);

            Spawn(board, bottom, PieceType.Soldier, 0, 3);
            Spawn(board, bottom, PieceType.Soldier, 2, 3);
            Spawn(board, bottom, PieceType.Soldier, 4, 3);
            Spawn(board, bottom, PieceType.Soldier, 6, 3);
            Spawn(board, bottom, PieceType.Soldier, 8, 3);
        }
        private static void SpawnTop(IBoardModel board, PlayerTeam top)
        {
            Spawn(board, top, PieceType.Chariot, 0, 9);
            Spawn(board, top, PieceType.Chariot, 8, 9);

            Spawn(board, top, PieceType.Elephant, 1, 9);
            Spawn(board, top, PieceType.Elephant, 7, 9);

            Spawn(board, top, PieceType.Horse, 6, 9);
            Spawn(board, top, PieceType.Horse, 2, 9);

            Spawn(board, top, PieceType.Guard, 3, 9);
            Spawn(board, top, PieceType.Guard, 5, 9);

            Spawn(board, top, PieceType.King, 4, 8);

            Spawn(board, top, PieceType.Cannon, 1, 7);
            Spawn(board, top, PieceType.Cannon, 7, 7);

            Spawn(board, top, PieceType.Soldier, 0, 6);
            Spawn(board, top, PieceType.Soldier, 2, 6);
            Spawn(board, top, PieceType.Soldier, 4, 6);
            Spawn(board, top, PieceType.Soldier, 6, 6);
            Spawn(board, top, PieceType.Soldier, 8, 6);

        }
       
        private static void Spawn(IBoardModel board, PlayerTeam team, PieceType type, int x, int z)
            => board.SetPiece(new Pos(x, z), new PieceModel(type, team, PieceId++));
    }
}