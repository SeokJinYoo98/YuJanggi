using UnityEngine;
using System.Collections.Generic;

namespace Yujanggi.Data.Board
{
    using Core.Domain;
    [CreateAssetMenu(fileName = "PieceData", menuName = "Piece/PieceData")]
    public class PieceData : ScriptableObject
    {
        [SerializeField] private PlayerType      _playerType;
        [SerializeField] private PieceType       _pieceType;
        [SerializeField] private Mesh            _mesh;
        [SerializeField] private int             _pieceValue;

        public Mesh         PieceMesh   => _mesh;
        public PlayerType   Team        => _playerType;
        public PieceType    Type        => _pieceType;
        public int          Value       => _pieceValue;
    }
}
