using System;
using System.Collections.Generic;
using Yujanggi.Core.Board;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Rule;

namespace Yujanggi.Runtime.Controller
{
    public enum AIMoveStrategyType
    {
        Random,
        Greedy,
        Minimax
    }

    public readonly struct AIMove
    {
        public AIMove(Pos from, Pos to)
        {
            From = from;
            To = to;
        }

        public Pos From { get; }
        public Pos To { get; }
    }

    public interface IAIMoveStrategy
    {
        bool TrySelectMove(IBoardModel board, IJanggiRule rule, PlayerTeam team, out AIMove move);
    }

    public static class AIMoveStrategyFactory
    {
        public static IAIMoveStrategy Create(AIMoveStrategyType type)
        {
            return type switch
            {
                AIMoveStrategyType.Greedy => new GreedyAIMoveStrategy(),
                AIMoveStrategyType.Minimax => new MinimaxAIMoveStrategy(),
                _ => new RandomAIMoveStrategy()
            };
        }
    }

    public sealed class RandomAIMoveStrategy : IAIMoveStrategy
    {
        private readonly Random _random = new();

        public bool TrySelectMove(IBoardModel board, IJanggiRule rule, PlayerTeam team, out AIMove move)
        {
            var moves = AIMoveGenerator.Generate(board, rule, team);
            if (moves.Count == 0)
            {
                move = default;
                return false;
            }

            move = moves[_random.Next(moves.Count)];
            return true;
        }
    }

    public sealed class GreedyAIMoveStrategy : IAIMoveStrategy
    {
        private readonly Random _random = new();

        public bool TrySelectMove(IBoardModel board, IJanggiRule rule, PlayerTeam team, out AIMove move)
        {
            var moves = AIMoveGenerator.Generate(board, rule, team);
            if (moves.Count == 0)
            {
                move = default;
                return false;
            }

            int bestScore = int.MinValue;
            var bestMoves = new List<AIMove>();
            foreach (var candidate in moves)
            {
                int score = AIPieceValue.Get(board.GetPiece(candidate.To).Type);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(candidate);
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(candidate);
                }
            }

            move = bestMoves[_random.Next(bestMoves.Count)];
            return true;
        }
    }

    public sealed class MinimaxAIMoveStrategy : IAIMoveStrategy
    {
        private readonly Random _random = new();
        private readonly int _searchDepth;
        private PlayerTeam _maximizingTeam;

        public MinimaxAIMoveStrategy(int searchDepth = 2)
        {
            _searchDepth = Math.Max(1, searchDepth);
        }

        public bool TrySelectMove(IBoardModel board, IJanggiRule rule, PlayerTeam team, out AIMove move)
        {
            var simulation = new AISimulationBoard(board);
            var moves = AIMoveGenerator.Generate(simulation, rule, team);
            if (moves.Count == 0)
            {
                move = default;
                return false;
            }

            _maximizingTeam = team;
            int bestScore = int.MinValue;
            var bestMoves = new List<AIMove>();
            foreach (var candidate in moves)
            {
                var record = simulation.DoMove(candidate.From, candidate.To);
                int score = Search(simulation, rule, AIPlayerTeam.Opponent(team), _searchDepth - 1, int.MinValue, int.MaxValue);
                simulation.UndoMove(record);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(candidate);
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(candidate);
                }
            }

            move = bestMoves[_random.Next(bestMoves.Count)];
            return true;
        }

        private int Search(IBoardModel board, IJanggiRule rule, PlayerTeam currentTeam, int depth, int alpha, int beta)
        {
            if (depth == 0)
                return Evaluate(board);

            var moves = AIMoveGenerator.Generate(board, rule, currentTeam);
            if (moves.Count == 0)
                return EvaluateTerminal(board, rule, currentTeam);

            bool maximizing = currentTeam == _maximizingTeam;
            int bestScore = maximizing ? int.MinValue : int.MaxValue;
            foreach (var candidate in moves)
            {
                var record = board.DoMove(candidate.From, candidate.To);
                int score = Search(board, rule, AIPlayerTeam.Opponent(currentTeam), depth - 1, alpha, beta);
                board.UndoMove(record);

                if (maximizing)
                {
                    bestScore = Math.Max(bestScore, score);
                    alpha = Math.Max(alpha, bestScore);
                }
                else
                {
                    bestScore = Math.Min(bestScore, score);
                    beta = Math.Min(beta, bestScore);
                }

                if (beta <= alpha)
                    break;
            }

            return bestScore;
        }

        private int EvaluateTerminal(IBoardModel board, IJanggiRule rule, PlayerTeam teamWithoutMove)
        {
            if (rule is not JanggiRule janggiRule || !janggiRule.IsKingInCheck(board, teamWithoutMove))
                return Evaluate(board);

            return teamWithoutMove == _maximizingTeam ? -100_000 : 100_000;
        }

        private int Evaluate(IBoardModel board)
        {
            int score = 0;
            for (int x = 0; x < board.WIDTH; ++x)
            {
                for (int z = 0; z < board.HEIGHT; ++z)
                {
                    var pos = new Pos(x, z);
                    if (!board.HasPiece(pos))
                        continue;

                    var piece = board.GetPiece(pos);
                    int value = AIPieceValue.Get(piece.Type);
                    score += piece.Team == _maximizingTeam ? value : -value;
                }
            }

            return score;
        }
    }

    internal static class AIMoveGenerator
    {
        public static List<AIMove> Generate(IBoardModel board, IJanggiRule rule, PlayerTeam team)
        {
            var moves = new List<AIMove>();
            var selection = new Selection();
            for (int x = 0; x < board.WIDTH; ++x)
            {
                for (int z = 0; z < board.HEIGHT; ++z)
                {
                    var from = new Pos(x, z);
                    if (!board.HasPiece(from) || board.GetPiece(from).Team != team)
                        continue;

                    selection.Clear();
                    selection.FromPos = from;
                    rule.FindWays(board, selection);
                    foreach (var to in selection.LegalCells)
                        moves.Add(new AIMove(from, to));
                }
            }

            return moves;
        }
    }

    internal static class AIPieceValue
    {
        public static int Get(PieceType type)
        {
            return type switch
            {
                PieceType.King => 10_000,
                PieceType.Chariot => 13,
                PieceType.Cannon => 7,
                PieceType.Horse => 5,
                PieceType.Elephant => 3,
                PieceType.Guard => 3,
                PieceType.Soldier => 2,
                _ => 0
            };
        }
    }

    internal sealed class AISimulationBoard : IBoardModel
    {
        private readonly PieceModel[,] _pieces;
        private readonly bool[,] _palaces;
        private Pos _choKingPos;
        private Pos _hanKingPos;

        public AISimulationBoard(IBoardModel source)
        {
            WIDTH = source.WIDTH;
            HEIGHT = source.HEIGHT;
            _pieces = new PieceModel[WIDTH, HEIGHT];
            _palaces = new bool[WIDTH, HEIGHT];
            _choKingPos = source.GetKingPos(PlayerTeam.Cho);
            _hanKingPos = source.GetKingPos(PlayerTeam.Han);

            for (int x = 0; x < WIDTH; ++x)
            {
                for (int z = 0; z < HEIGHT; ++z)
                {
                    var pos = new Pos(x, z);
                    _palaces[x, z] = source.IsPalace(pos);
                    _pieces[x, z] = source.HasPiece(pos) ? source.GetPiece(pos) : PieceModel.None;
                }
            }
        }

        public int WIDTH { get; }
        public int HEIGHT { get; }

        public Pos GetKingPos(PlayerTeam team)
            => team == PlayerTeam.Cho ? _choKingPos : _hanKingPos;
        public bool IsInside(Pos pos)
            => 0 <= pos.X && pos.X < WIDTH && 0 <= pos.Z && pos.Z < HEIGHT;
        public bool HasPiece(Pos pos)
            => !_pieces[pos.X, pos.Z].IsNone;
        public PieceModel GetPiece(Pos pos)
            => _pieces[pos.X, pos.Z];
        public bool IsPalace(Pos pos)
            => IsInside(pos) && _palaces[pos.X, pos.Z];
        public void SetPiece(Pos pos, PieceModel piece)
            => _pieces[pos.X, pos.Z] = piece;

        public MoveRecord DoMove(Pos from, Pos to)
        {
            var moved = GetPiece(from);
            var captured = GetPiece(to);
            SetPiece(from, PieceModel.None);
            SetPiece(to, moved);
            UpdateKingPos(to, moved);
            return new MoveRecord(from, to, moved, captured);
        }

        public void UndoMove(in MoveRecord moveRecord)
        {
            SetPiece(moveRecord.From, moveRecord.MovedPiece);
            SetPiece(moveRecord.To, moveRecord.IsCapture ? moveRecord.CapturedPiece : PieceModel.None);
            UpdateKingPos(moveRecord.From, moveRecord.MovedPiece);
        }

        private void UpdateKingPos(Pos pos, PieceModel piece)
        {
            if (piece.Type != PieceType.King)
                return;

            if (piece.Team == PlayerTeam.Cho)
                _choKingPos = pos;
            else if (piece.Team == PlayerTeam.Han)
                _hanKingPos = pos;
        }
    }

    internal static class AIPlayerTeam
    {
        public static PlayerTeam Opponent(PlayerTeam team)
            => team == PlayerTeam.Cho ? PlayerTeam.Han : PlayerTeam.Cho;
    }
}
