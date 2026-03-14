using UnityEngine;
using System.Collections.Generic;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using System.Text;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;
 


    public class Board : MonoBehaviour, IBoard
    {
        [SerializeField] PieceSpawner     _pieceSpawner;
        [SerializeField] BoardHighlighter _highlighter;
        private BoardState  _boardState;
        private JanggiRule  _janggiRule;
        private TurnInfo    _turnInfo;
        private bool _update = false;

        private List<(IPiece, int x, int z)> _hanDied = new();
        private List<(IPiece, int x, int z)> _choDied = new();
        private void Awake()
        {
            _janggiRule = new();
            _boardState = new();
            _turnInfo   = new();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _pieceSpawner.SpawnPieces(_boardState);
        }

        void Update()
        {
            if (_update)
            {
                _update = UpdateBoard();
            }
        }
        public  void HandleClick(int x, int z, PlayerType player)
        {
            switch(_turnInfo.Turn)
            {
                case TurnType.Update:
                    break;
                case TurnType.Select:
                    SelectPiece(x, z, player);
                    break;
                case TurnType.Attack:
                    SelectWay(x, z, player);
                    break;
            }
        }
        private void SelectPiece(int x, int z, PlayerType player)
        {
            if (!IsValidInput(player)
                || !TrySelectPiece(x, z, out var piece)) 
                return;
            UpdateTurnInfo(TurnType.Attack, _turnInfo.Player, piece, x, z);
            HighlightPiece();
            FindWays();
            HighlightWays();
        }
        private void SelectWay(int x, int z, PlayerType player)
        {
            if (!IsValidInput(player)) return;
            if (!TrySelectWay(x, z))
            {
                UnSelect(x, z);
                UpdateTurnInfo(TurnType.Select, _turnInfo.Player);
                return;
            }
            MovePiece(x, z);
            UnSelect(x, z);
            UpdateTurnInfo(TurnType.Update, _turnInfo.Player);
        }
        private bool UpdateBoard()
        {
            UpdateTurnInfo(TurnType.Select, NextPlayer());
            return false;
        }

        private void UnSelect(int x, int z)
        {
            _boardState.ClearMovable(); 
            HighlightPiece();
            HighlightWays();
        }
        private void UpdateTurnInfo(TurnType turn, PlayerType player, IPiece piece=null, int x=-100, int z=-100)
        {
            _turnInfo.x = x; _turnInfo.z = z;
            _turnInfo.Turn   = turn;
            _turnInfo.Player = player;
            _turnInfo.Piece  = piece;

            if (_turnInfo.Turn == TurnType.Update) _update = true;
        }
        private bool        TrySelectPiece(int x, int z, out IPiece piece)
        {
            piece = _boardState.GetPiece(x, z);
            if (piece == null)
                return false;

            return piece.IsOwner(_turnInfo.Player);
        }
        private void        HighlightPiece()
        {
            _turnInfo.Piece.Highlight();
        }
        private void        FindWays()
        {
            _janggiRule.FindWays(_boardState, _turnInfo);
        }
        private void HighlightWays()
        {
            _highlighter.Highlight(_boardState.MovableCells);
        }
        private bool        IsValidInput(PlayerType player)
        {
            if (_turnInfo.Turn == TurnType.Update) return false;

            return true; //player == _turnInfo.Player;
        }
        private bool        TrySelectWay(int x, int z)
        {
            return _boardState.IsMovable(x, z);
        }
        private void        MovePiece(int toX, int toZ)
        {
            int fromX = _turnInfo.x; int fromZ = _turnInfo.z;
            _boardState.MovePiece(fromX, fromZ, toX, toZ, out var killed);
            
            // 임시 코드
            if (killed != null)
            {
                var type = killed.Team;
                switch (type)
                {
                    case PlayerType.Cho:
                        if (_choDied.Count > 0)
                        {
                            var last = _choDied[^1];
                            _choDied.Add((killed, last.Item2 + 1, last.Item3));
                        }
                        else
                        {
                            _choDied.Add((killed, 0, -4)); // 초기 좌표 예시
                        }
                        var x = _choDied[^1];
                        killed.MoveTo(x.x, x.z);
                        break;

                    case PlayerType.Han:
                        if (_hanDied.Count > 0)
                        {
                            var last = _hanDied[^1];
                            _hanDied.Add((killed, last.Item2 + 1, last.Item3));
                        }
                        else
                        {
                            _hanDied.Add((killed, 0, -3)); // 초기 좌표 예시
                        }
                        var z = _hanDied[^1];
                        killed.MoveTo(z.x, z.z);
                        break;
                }
            }
        }
        private PlayerType  NextPlayer()
            => _turnInfo.Player == PlayerType.Cho ? PlayerType.Han : PlayerType.Cho;
    }
}
