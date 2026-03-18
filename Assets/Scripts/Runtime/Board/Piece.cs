using UnityEngine;
namespace Yujanggi.Runtime.Board
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
        [SerializeField] private PieceData _data;
        public void Init(PieceData data, Pos pos, PlayerTeam bottom)
        {
            _data = data;
            GetComponent<MeshFilter>().sharedMesh = _data.PieceMesh;
            MaterialCheck();
            transform.position = BoardHelper.ToVector3(pos, transform.position.y);

            if (_data.Team == bottom)
                transform.Rotate(new Vector3(0, 180, 0));
        }
        public void  MoveTo(Pos toPos)
        {
            if (_moveRoutine != null)
                StopCoroutine(_moveRoutine);
            Vector3 targetWorldPos = BoardHelper.ToVector3(toPos, transform.position.y);
            _moveRoutine = StartCoroutine(CoMove(targetWorldPos, 0.16f));
        }
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
