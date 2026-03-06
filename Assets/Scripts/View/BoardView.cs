using UnityEngine;
namespace Yujanggi.View.Board
{
    public class BoardView : MonoBehaviour
    {
        public Transform Test;
        int x = -1, y;
        float time = 1.0f;
        Vector3 _pos = new Vector3(0, 2, 0);

        Core.Board.BoardData _boardData;
        private void Awake()
        {
            _boardData = new();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (y >= 10) return;
           if (time >= 1.0f)
           {
                time -= 1.0f;
                x += 1;
                if (x >= 9)
                {
                    x = 0;
                    y += 1;
                   
                }
                if (y >= 10) return;
                var pos = _boardData[y, x].Position;
                Debug.Log($"{pos.X}, {pos.Z}");
                _pos.x = pos.X; _pos.z = pos.Z;
                Test.transform.position = _pos;
           }
           else
            {
                time += Time.deltaTime;
            }
        }
        

    }

}
