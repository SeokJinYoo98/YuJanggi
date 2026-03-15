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
        public Piece GetPrefab(PlayerTeam type)
            => _prefabs[(int)type];

        public PieceData GetData(PlayerTeam playerType, PieceType pieceType)
        {
            var list = playerType == PlayerTeam.Cho ? _chos : _hans;
            return list[(int)pieceType];
        }
    }
}
