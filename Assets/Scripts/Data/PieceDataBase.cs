using System.Collections.Generic;
using UnityEngine;
using Yujanggi.Core.Domain;
using Yujanggi.Runtime.Board;

namespace Yujanggi.Data.Board
{
    [CreateAssetMenu(fileName = "PieceDataBase", menuName = "Piece/PieceDataBase")]
    public class PieceDataBase : ScriptableObject
    {
        [SerializeField] private List<Piece>     _prefabs;
        [SerializeField] private Vector3         _baseScale;

        [SerializeField] private List<PieceData> _chos;
        [SerializeField] private List<PieceData> _hans;

        public Vector3 BaseScale => _baseScale;
        public Piece GetPrefab(PlayerType type)
            => _prefabs[(int)type];

        public PieceData GetData(PlayerType playerType, PieceType pieceType)
        {
            var list = playerType == PlayerType.Cho ? _chos : _hans;
            return list[(int)pieceType];
        }
    }
}
