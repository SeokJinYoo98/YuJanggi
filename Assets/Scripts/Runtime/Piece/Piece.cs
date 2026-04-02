using UnityEngine;
namespace Yujanggi.Runtime.Piece
{
    using System.Collections;
    using Yujanggi.Core.Domain;
    using Yujanggi.Data.Board;
    using Yujanggi.Utills.Board;
    
    public interface IPiece
    {
        public void Highlight();
        public void MoveTo(Pos toPos);
    }

    public class Piece : MonoBehaviour, IPiece
    {
        private Coroutine _moveRoutine;
        private bool      _highligt;
        public void Init(PieceData data, Pos pos)
        {
            var team = data.Team;
            var type = data.Type;
            GetComponent<MeshFilter>().sharedMesh = data.PieceMesh;
            MaterialCheck(team, type);
            transform.position = new Vector3(pos.X, 1, pos.Z);
            transform.Rotate(new Vector3(0, 180, 0));
        }
        public void  MoveTo(Pos toPos)
        {
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            Vector3 targetWorldPos = new Vector3(toPos.X, transform.position.y, toPos.Z);
            _moveRoutine = StartCoroutine(CoMove(targetWorldPos, 0.16f));
        }
        public void  Highlight()
        {
            if (_highligt) return;
            SwapMaterial();
            _highligt = !_highligt;
        }
        public void UnHighlight()
        {
            if (!_highligt) return;
            SwapMaterial();
            _highligt = !_highligt;
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
            var renderer = GetComponent<MeshRenderer>();
            var mats = renderer.sharedMaterials;
            (mats[0], mats[1]) = (mats[1], mats[0]);
            renderer.sharedMaterials = mats;
        }
        private IEnumerator CoMove(Vector3 targetPos, float duration)
        {
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
            _moveRoutine = null;
        }
    }

}
