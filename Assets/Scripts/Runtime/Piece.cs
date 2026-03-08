using UnityEngine;
namespace Yujanggi.Runtime.Board
{
    using Yujanggi.Data.Board;
    using Yujanggi.Core.Domain;

    public class Piece : MonoBehaviour, IPiece
    {
        [SerializeField] private PieceData _data;
     
        public PlayerType Team  => _data.Team;
        public PieceType  Type  => _data.Type;

        private void Awake()
        {
   
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
            GetComponent<MeshFilter>().sharedMesh = _data.PieceMesh;

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
        public void SwapMaterial()
        {
            var renderer = GetComponent<MeshRenderer>();
            var mats = renderer.sharedMaterials;
            (mats[0], mats[1]) = (mats[1], mats[0]);
            renderer.sharedMaterials = mats;
        }
    }

}
