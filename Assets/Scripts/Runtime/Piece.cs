using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Yujanggi.Data.Board;
    using Yujanggi.Core.Domain;

    public class Piece : MonoBehaviour
    {
        [SerializeField] private PieceData _data;

        private MeshFilter   _mesh;
        private void Awake()
        {
            _mesh = GetComponent<MeshFilter>();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(PieceData data, int x, int z)
        {
            _data = data;
            transform.position = new(x, 0.1f, z);
            _mesh.sharedMesh = _data.PieceMesh;

            MaterialCheck();
            
        }
        private void MaterialCheck()
        {
            if (_data.Team == PlayerType.Cho)
            {
                if (_data.Type ==  PieceType.Guard) {
                    SwapMaterial();
                }        
            }

            else if (_data.Team == PlayerType.Han) {
                if (_data.Type == PieceType.Soldier || _data.Type == PieceType.Cannon) {
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
