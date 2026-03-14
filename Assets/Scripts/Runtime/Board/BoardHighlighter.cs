using UnityEngine;

namespace Yujanggi.Runtime.Board
{
    using System.Collections.Generic;
    using UnityEngine.Pool;

    public class BoardHighlighter : MonoBehaviour
    {
        [SerializeField] private CellHighlighter _prefab;
        private ObjectPool<CellHighlighter>      _pool;
        private List<CellHighlighter> _active;
        void Awake()
        {
            _active = new(25);
            _pool = new ObjectPool<CellHighlighter>(
                createFunc     : ()         => Instantiate(_prefab, transform),
                actionOnGet    : obj        => obj.gameObject.SetActive(true),
                actionOnRelease: obj        => obj.gameObject.SetActive(false),
                actionOnDestroy: obj        => Destroy(obj.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize        : 25
            );
        }
        public void Highlight(IReadOnlyList<(int x, int z)> cells)
        {
            Clear();
            foreach (var pos in cells)
            {
                var highlight = _pool.Get();
                highlight.MoveTo(BoardToWorld(pos.x, pos.z));
                _active.Add(highlight);
            }
        }
        public void Clear()
        {
            foreach (var h in _active)
                _pool.Release(h);

            _active.Clear();
        }

        Vector3 BoardToWorld(int x, int z)
        {
            return new Vector3(x, transform.position.y, z);
        }
    }
}