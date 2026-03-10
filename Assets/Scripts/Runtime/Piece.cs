using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Yujanggi.Data.Board;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Board;
    using System.Collections.Generic;
    using UnityEngine.UIElements;
    using Yujanggi.Core.Movement;

    public class Piece : MonoBehaviour, IPiece
    {
        [SerializeField] private PieceData _data;

        private Movement _movement;
        public PieceType  Type => _data.Type;
        public PlayerType Team => _data.Team;
        public void Init(PieceData data, int x, int z)
        {
            _data = data;
            transform.position = new(x, 0.1f, z);
            GetComponent<MeshFilter>().sharedMesh = _data.PieceMesh;
            MaterialCheck();

            if (_data.Type == PieceType.Soldier)
                _movement = new SoldierMovement();
        }

        public void MoveTo(int x, int z)
            => transform.position = new(x, 0.1f, z);
        public bool IsOwner(PlayerType type)
            => type == _data.Team;

        public void Highlight()
        {
            SwapMaterial();
        }
        public void FindWays(IBoardState board, int x, int z)
        {
            _movement.FindWays(board, x, z, _data.Team);
        }
        private void MaterialCheck()
        {
            if (_data.Team == PlayerType.Cho)
            {
                if (_data.Type ==  PieceType.Guard)
                {
                    SwapMaterial();
                }
            }

            else if (_data.Team == PlayerType.Han)
            {
                if (_data.Type == PieceType.Soldier || _data.Type == PieceType.Cannon)
                {
                    SwapMaterial();
                }
            }
        }
        private void SwapMaterial()
        {
            var renderer = GetComponent<MeshRenderer>();
            var mats = renderer.sharedMaterials;
            (mats[0], mats[1]) = (mats[1], mats[0]);
            renderer.sharedMaterials = mats;
        }

        
    }

}
