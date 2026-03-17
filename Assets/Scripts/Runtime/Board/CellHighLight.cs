using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Board
{
    public class CellHighlighter : MonoBehaviour
    {
        Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }
        public void Show()
        {
            _renderer.enabled = true;
        }

        public void Hide()
        {
            _renderer.enabled = false;
        }
        public void MoveTo(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}