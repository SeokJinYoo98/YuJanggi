using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Yujanggi.Data.Board;
    using Yujanggi.Core.Domain;
    using Yujanggi.Utills.Board;

    public interface IPiece
    {
        bool IsOwner(PlayerTeam type);

        public void Highlight();
        public void MoveTo(Pos toPos);

    }

    public class Piece : MonoBehaviour, IPiece
    {    
        [SerializeField] private PieceData _data;
        public void Init(PieceData data, Pos pos)
        {
            _data = data;
            GetComponent<MeshFilter>().sharedMesh = _data.PieceMesh;
            MaterialCheck();
            MoveTo(pos);
        }

        public void MoveTo(Pos toPos)
            => transform.position = BoardHelper.ToVector3(toPos, transform.position.y);
        public bool IsOwner(PlayerTeam type)
            => type == _data.Team;
        public void  Highlight()
        {
            SwapMaterial();
        }

        private void MaterialCheck()
        {
            if (_data.Team == PlayerTeam.Cho)
            {
                if (_data.Type ==  PieceType.Guard)
                {
                    SwapMaterial();
                }
            }

            else if (_data.Team == PlayerTeam.Han)
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
