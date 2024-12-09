using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios.Board
{
    public class BoardDataSO : ScriptableObject
    {
        private Cell[] _cells;
        private Line[] _lines;
        private int _cellsCount, _linesCount;
        private int _rowsCount, _columnsCount;
        private float _cellWidth, _cellHeight;
        private float _boardWidth, _boardHeight;

        public float CellWidth => _cellWidth;
        public float CellHeight => _cellHeight;

        public int RowsCount => _rowsCount;
        public int ColumnsCount => _columnsCount;
        public int TotalCellsCount => _cellsCount;

        public Transform BoardParent {  get; private set; }             

        public IEnumerable<Line> Lines => _lines;

        public void GenerateBoardData(int rows, int columns, float cellWidth, float cellHeight, Transform boardParent)
        {
            _rowsCount = rows;
            _columnsCount = columns;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
            _boardWidth = cellWidth * columns;
            _boardHeight = cellHeight * rows;
            BoardParent = boardParent;
            InitializeLinesCollection();
            CreateCells();
        }

        public void ClearBoardData()
        {
            _cellsCount = 0;
            _linesCount = 0;
            _rowsCount = 0;
            _columnsCount = 0;

            _cellWidth = 0f;
            _cellHeight = 0f;
            _boardWidth = 0f;
            _boardHeight = 0f;

            _cells = null;
            _lines = null;

            BoardParent = null;
        }

        public bool TryGetCellIndex(Vector2 cellCoordinates, out int index)
        {
            index = -1;
            cellCoordinates.x += _boardWidth * 0.5f;
            cellCoordinates.y += _boardHeight * 0.5f;
            if (cellCoordinates.x > _boardWidth || cellCoordinates.y > _boardHeight) return false;
            int x = Mathf.FloorToInt(cellCoordinates.x / _cellWidth);
            int y = Mathf.Abs(Mathf.FloorToInt(cellCoordinates.y / _cellHeight) - (_rowsCount - 1));
            index = _columnsCount * y + x;
            if (x >= 0 && y >= 0 && index < _cellsCount)
            {
                return true;
            }
            return false;
        }

        public bool TryGetCellIndex(Cell cell, out int index)
        {
            index = -1;
            if (cell == null)
            {
                Debug.LogError("Could not pass a null cell to get its index");
                return false;
            }
            int flaggedIndex = -1;
            foreach (Cell c in _cells)
            {
                    flaggedIndex++;
                if(c == cell)
                {
                    index = flaggedIndex; break;
                }
            }
            return index >= 0;
        }

        public Cell GetCell(int index) => _cells[index];

        public void AddLineToCollection(Line line)
        {
            _lines[_linesCount] = line;
            _linesCount++;
        }

        private void CreateCells()
        {
            Vector2 cellScale = new Vector2(CellWidth, CellHeight);
            _cellsCount = RowsCount * ColumnsCount;
            _cells = new Cell[_cellsCount];
            int indexX = 0;
            int indexY = 0;
            Vector3 initialPosition = new Vector3((-_boardWidth * 0.5f) + (CellWidth * 0.5f), (_boardHeight * 0.5f) - (CellHeight * 0.5f), 0f);
            for (int i = 0; i < _cellsCount; i++)
            {
                float posX = initialPosition.x + (CellWidth * indexX);
                float posY = initialPosition.y - (CellHeight * indexY);
                _cells[i] = new Cell(new Vector3(posX, posY, initialPosition.z), cellScale, this);
                indexX++;
                if (indexX == RowsCount)
                {
                    indexX = 0;
                    indexY++;
                }
            }
        }

        private void InitializeLinesCollection()
        {
            int firstCell = 4; // because first cell will create all four lines around it
            int remainingFirstCellsOfColumns = (ColumnsCount - 1) * 3; // because every first cell in each column will create 3 news lines and sharing right most line of their neighboring cell
            int remainingFirstCellOfRows = (RowsCount - 1) * 3; // because every first cell in each row will create 3 new lines and sharing bottom line from the cell above them
            int remainingCells = (ColumnsCount - 1) * (RowsCount - 1) * 2;// because remainig cells will create 2 new lines and sharing one line from the cell on their right and one line from the cell above them
            _lines = new Line[firstCell + remainingFirstCellsOfColumns + remainingFirstCellOfRows + remainingCells];
        }
    }
}