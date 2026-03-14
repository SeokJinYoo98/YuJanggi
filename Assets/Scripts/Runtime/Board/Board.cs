using UnityEngine;
using System.Collections.Generic;
namespace Yujanggi.Runtime.Board
{
    using Core.Board;
    using System;
    using System.Text;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Rule;

   
    // 보드 컨트롤러가 될 예정
    public class Board : MonoBehaviour, IBoard
    {
        [SerializeField] PieceSpawner     _pieceSpawner;
        [SerializeField] BoardHighlighter _highlighter;

        private BoardState  _boardState;
        private JanggiRule  _janggiRule;
        private BoardInfo    _boardInfo;

        private List<(IPiece, int x, int z)> _hanDied = new();
        private List<(IPiece, int x, int z)> _choDied = new();
        private void Awake()
        {
            _janggiRule = new();
            _boardState = new();
            _boardInfo   = new();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _pieceSpawner.SpawnPieces(_boardState);
        }

        void Update()
        {
  
        }

        public bool SelectPiece(int x, int z, PlayerType team)
        {
            if (!TrySelectPiece(team, x, z, out var piece)) 
                return false;
            UpdateBoardInfo(team, piece, x, z);
            HighlightPiece();
            FindWays();
            HighlightWays();
            return true;
        }
        public bool SelectWay(int x, int z, PlayerType team)
        {
            if (!TrySelectWay(x, z))
            {
                UnSelect(x, z);
                UpdateBoardInfo(_boardInfo.Team, _boardInfo.Piece, x, z);
                return false;
            }
            MovePiece(x, z);
            UnSelect(x, z);
            UpdateBoardInfo(team);
            return true;
        }

        private void UnSelect(int x, int z)
        {
            _boardState.ClearMovable(); 
            HighlightPiece();
            HighlightWays();
        }

        private bool TrySelectPiece(PlayerType team, int x, int z, out IPiece piece)
        {
            piece = _boardState.GetPiece(x, z);
            if (piece == null)
                return false;

            return piece.IsOwner(team);
        }
        private void        HighlightPiece()
        {
            _boardInfo.Piece.Highlight();
        }
        private void        FindWays()
        {
            _janggiRule.FindWays(_boardState, _boardInfo);
        }
        private void HighlightWays()
        {
            _highlighter.Highlight(_boardState.MovableCells);
        }
 
        private bool        TrySelectWay(int x, int z)
        {
            return _boardState.IsMovable(x, z);
        }
        private void        MovePiece(int toX, int toZ)
        {
            int fromX = _boardInfo.x; int fromZ = _boardInfo.z;
            _boardState.MovePiece(fromX, fromZ, toX, toZ, out var killed);
         
        }
        


        /*
         * 
         * 턴 관련
        */
        private void UpdateBoardInfo(PlayerType team, IPiece piece = null, int x = -100, int z = -100)
        {
            
            _boardInfo.x = x; _boardInfo.z = z;
            _boardInfo.Team =   team;
            _boardInfo.Piece  = piece;
            _boardInfo.PieceType = piece == null ? PieceType.None : piece.Type;
        }

    }
}
