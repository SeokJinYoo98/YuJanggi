using UnityEngine;
namespace Yujanggi.Runtime.Piece
{
    using System.Collections;
    using Yujanggi.Core.Domain;
    using Yujanggi.Data.Board;
    using Yujanggi.Runtime.Input;

    public interface IPieceView
    {
        public void Highlight();
        public void MoveTo(Pos toPos);
    }

    public class PieceView : MonoBehaviour, IPieceView, IBoardClickable
    {
        public Pos BoardPos => _boardPos;
        
        private Pos          _boardPos;
        private MeshCollider _meshCollider;
        private MeshFilter   _meshFilter;
        private MeshRenderer _meshRenderer;
        private Coroutine    _moveRoutine;
        private bool         _highlight;

        void Awake()
        {
            _meshCollider = GetComponent<MeshCollider>();
            _meshFilter   = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        public void Init(PieceData data, Pos pos)
        {
            _boardPos                = pos;
            _meshCollider.sharedMesh = data.PieceMesh;
            _meshFilter.sharedMesh   = data.PieceMesh;

            var team = data.Team;
            var type = data.Type;

            MaterialCheck(team, type);
            transform.position = new Vector3(pos.X, 1, pos.Z);
            transform.Rotate(new Vector3(0, 180, 0));
        }
        public void  MoveTo(Vector3 toPos)
        {
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            _moveRoutine = StartCoroutine(CoMove(toPos, 0.16f)); 
        }
        public void  MoveTo(Pos toPos)
        {
            _boardPos = toPos;
            MoveTo(new Vector3(toPos.X, 1, toPos.Z));
        }
        public void SetDead(bool dead)
        {
            _meshCollider.enabled = !dead;
            UnHighlight();
        }
        public void  Highlight()
        {
            if (_highlight) return;
            SwapMaterial();
            _highlight = !_highlight;
        }
        public void  UnHighlight()
        {
            if (!_highlight) return;
            SwapMaterial();
            _highlight = !_highlight;
        }
        private void MaterialCheck(PlayerTeam team, PieceType type)
        {
            if (team == PlayerTeam.Cho)
            {
                if (type ==  PieceType.Guard)
                {
                    SwapMaterial();
                }
            }

            else if (team == PlayerTeam.Han)
            {
                if (type == PieceType.Soldier || type == PieceType.Cannon)
                {
                    SwapMaterial();
                }
            }
        }
        private void SwapMaterial()
        {
            var mats = _meshRenderer.sharedMaterials;

            if (mats.Length < 2)
                return;

            (mats[0], mats[1]) = (mats[1], mats[0]);
            _meshRenderer.sharedMaterials = mats;
        }
        private IEnumerator CoMove(Vector3 targetPos, float duration)
        {
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
            _moveRoutine = null;
        }
    }

}
