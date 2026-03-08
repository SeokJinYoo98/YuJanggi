using System;
using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Board
{
    public class BoardData
    {
        public const int WIDTH  = 9;
        public const int HEIGHT = 10;

        private CellData[,] _cellDatas;

        public BoardData()
        {
            _cellDatas = new CellData[HEIGHT, WIDTH];

            for (int z = 0; z < HEIGHT; ++z)
            {
                for (int x = 0; x < WIDTH; ++x)
                {
                    _cellDatas[z, x] = new CellData
                    {
                        Position = new(x, z)
                    };
                }
            }
        }
        public CellData this[int x, int z]
        {
            get { return _cellDatas[z, x]; }
        }

    }
}
