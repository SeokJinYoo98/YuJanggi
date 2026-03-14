using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Board
{
    public class CellHighlighter : MonoBehaviour
    {
        private void OnEnable()
        {

        }
        private void OnDisable()
        {
            
        }

        public void MoveTo(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}