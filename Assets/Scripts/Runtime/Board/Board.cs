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
            if (_turnInfo.Turn == TurnType.Update) 
                return;

            if (_turnInfo.Turn == TurnType.Select)
                SelectPiece(x, z, player);
   
            else if (_turnInfo.Turn == TurnType.Attack)
                SelectWay(x, z, player);
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
        private void        UpdateTurnInfo(TurnType turn, PlayerType player, IPiece piece=null, int x=-100, int z=-100)
        {
            _turnInfo.x = x; _turnInfo.z = z;
            _turnInfo.Turn = turn;
            _turnInfo.Player = player;
            _turnInfo.Piece= piece;

            if (_turnInfo.Turn == TurnType.Update) _update = true;
        }
        private bool        TrySelectPiece(int x, int z, out IPiece piece)
        {
            piece = _boardState.GetPiece(x, z);
            if (piece == null)
                return false;
            if (!piece.IsOwner(_turnInfo.Player))
                return false;

            return true;
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
            var info = _boardState.MovePiece(fromX, fromZ, toX, toZ);
        }
        private PlayerType  NextPlayer()
            => _turnInfo.Player == PlayerType.Cho ? PlayerType.Han : PlayerType.Cho;
    }
}
