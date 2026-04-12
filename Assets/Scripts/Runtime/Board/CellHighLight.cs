using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Board
{
    public class CellHighlighter : MonoBehaviour
    {
        Renderer _renderer;
        [SerializeField] private Material _legalMat;
        [SerializeField] private Material _illegalMat;

        private bool _isLegalState;
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.sharedMaterial = _legalMat;
            _isLegalState = true;
        }
        public void Show(bool isLegal)
        {
            if (isLegal != _isLegalState)
            {
                _renderer.sharedMaterial = isLegal ? _legalMat : _illegalMat;
                _isLegalState = isLegal;
            }
            
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