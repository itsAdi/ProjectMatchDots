using UnityEngine;

namespace KemothStudios.Board
{
    public class BoardDataSO : ScriptableObject
    {
        private Cell[] _cells;
        private int _cellsCount, _rowsCount, _columnsCount;
        private float _cellWidth, _cellHeight;
        private float _boardWidth, _boardHeight;

        public float CellWidth => _cellWidth;
        public float CellHeight => _cellHeight;

        public void GenerateBoardData(int rows, int columns, float cellWidth, float cellHeight)
        {
            _rowsCount = rows;
            _columnsCount = columns;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
            _boardWidth = cellWidth * columns;
            _boardHeight = cellHeight * rows;
            Vector2 cellScale = new Vector2(cellWidth, cellHeight);
            _cellsCount = rows * columns;
            _cells = new Cell[_cellsCount];
            int indexX = 0;
            int indexY = 0;
            Vector3 initialPosition = new Vector3((-_boardWidth * 0.5f) + (cellWidth * 0.5f), (_boardHeight * 0.5f) - (cellHeight * 0.5f), 0f);
            for (int i = 0; i < _cellsCount; i++)
            {
                float posX = initialPosition.x + (cellWidth * indexX);
                float posY = initialPosition.y - (cellHeight * indexY);
                _cells[i] = new Cell(new Vector3(posX, posY, initialPosition.z), cellScale, this);
                indexX++;
                if (indexX == rows)
                {
                    indexX = 0;
                    indexY++;
                }
            }
        }

        public bool TryGetCellIndex(Vector2 cellCoordinates, out int index)
        {
            index = -1;
            cellCoordinates.x += _boardWidth * 0.5f;
            cellCoordinates.y += _boardHeight * 0.5f;
            if(cellCoordinates.x  > _boardWidth || cellCoordinates.y > _boardHeight) return false;
            int x = Mathf.FloorToInt(cellCoordinates.x / _cellWidth);
            int y = Mathf.Abs(Mathf.FloorToInt(cellCoordinates.y / _cellHeight) - (_rowsCount - 1));
            index = _columnsCount * y + x;
            if (x >= 0 && y >= 0 && index < _cellsCount)
            {
                return true;
            }
            return false;
        }

        public Cell GetCell(int index) => _cells[index];
    }
}