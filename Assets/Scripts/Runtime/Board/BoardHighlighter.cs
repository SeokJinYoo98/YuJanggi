using UnityEngine;

namespace Yujanggi.Runtime.Board
{ 
    using Core.Domain;
    using System.Collections.Generic;
    using UnityEngine.Pool;

    public class BoardHighlighter : MonoBehaviour
    {
        [SerializeField] private CellHighlighter _prefab;
        private ObjectPool<CellHighlighter>      _pool;
        private List<CellHighlighter> _active;
        void Awake()
        {
            _active = new List<CellHighlighter>(25);

            _pool = new ObjectPool<CellHighlighter>(
                ()  => Instantiate(_prefab, transform),
                obj => obj.Show(),
                obj => obj.Hide(),
                obj => Destroy(obj.gameObject),
                false,
                25,
                25
            );

            for (int i = 0; i < 25; i++)
                _pool.Release(_pool.Get());
        }
        public void ShowHighlight(IReadOnlyList<Pos> cells)
        {
            int length = cells.Count;
            for (int i = 0; i < length; ++i)
            {
                var pos = cells[i];
                var highlight = _pool.Get();
                
                highlight.MoveTo(new Vector3(pos.X, transform.position.y, pos.Z));
                _active.Add(highlight);
            }
        }
        public void HideHighlight()
        {
            foreach (var h in _active)
                _pool.Release(h);

            _active.Clear();
        }

    }
}