using System.Collections.Generic;
using KemothStudios.Utility;
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
        private Rect _boardRect, _thresholdBoardRect;

        public float CellWidth => _cellWidth;
        public float CellHeight => _cellHeight;

        public int RowsCount => _rowsCount;
        public int ColumnsCount => _columnsCount;
        public int TotalCellsCount => _cellsCount;
        public int CompletedCellsCount { get; set; }

        public Transform BoardParent {  get; private set; }             

        public IEnumerable<Line> Lines => _lines;

        public void GenerateBoardData(int rows, int columns, float cellWidth, float cellHeight, float boardClickThreshold, Transform boardParent)
        {
            _rowsCount = rows;
            _columnsCount = columns;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
            _boardWidth = cellWidth * columns;
            _boardHeight = cellHeight * rows;
            BoardParent = boardParent;
            CompletedCellsCount = 0;
            InitializeLinesCollection();
            CreateCells();
            
            // rects to detect click area and threshold click area
            // threshold click area is the area bigger than click area and is used in case where user clicked outside of the click area and we have to map that
            // input to the nearest edge on the click area
            Cell cell = GetCell(TotalCellsCount - (ColumnsCount - 1) - 1);
            
            // Deducting a small amount of value from width height because if we map click area exactly to the board then
            // clicking in the threshold area will return back exact values sticking on the edge of the board
            // and this will mess up the index calculation
            _boardRect = new Rect(cell.CellTransform.x, cell.CellTransform.y, _boardWidth - 0.01f, _boardHeight - 0.01f);
            _thresholdBoardRect = new Rect(_boardRect);
            _thresholdBoardRect.xMin -= 1f;
            _thresholdBoardRect.xMax += 1f;
            _thresholdBoardRect.yMin -= 1f;
            _thresholdBoardRect.yMax += 1f;
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

            CompletedCellsCount = 0;
            BoardParent = null;
        }

        public bool TryGetCellIndex(Vector2 cellCoordinate, out int index)
        {
            index = -1;
            // check if click point is directly on the board, if not then choose the closest point on the board edge
            bool validClick = _boardRect.Contains(cellCoordinate); 
            if (!validClick && _thresholdBoardRect.Contains(cellCoordinate))
            {
                validClick = true;
                var vertex0 = _boardRect.min;
                var vertex1 = new Vector2(_boardRect.xMin, _boardRect.yMax);
                var vertex2 = _boardRect.max;
                var vertex3 = new Vector2(_boardRect.xMax, _boardRect.yMin);

                // Find the closest point/edge and save in cellCoordinate.
                Vector2 closestPoint = Vector2.zero;
                var closestSqrDistance = float.MaxValue;
                CheckBestEdge(vertex0, vertex1, cellCoordinate, ref closestPoint, ref closestSqrDistance);
                CheckBestEdge(vertex1, vertex2, cellCoordinate, ref closestPoint, ref closestSqrDistance);
                CheckBestEdge(vertex2, vertex3, cellCoordinate, ref closestPoint, ref closestSqrDistance);
                CheckBestEdge(vertex3, vertex0, cellCoordinate, ref closestPoint, ref closestSqrDistance);
                cellCoordinate = closestPoint;
            }

            if (!validClick) return false;
            cellCoordinate.x += _boardWidth * 0.5f;
            cellCoordinate.y += _boardHeight * 0.5f;
            int x = Mathf.FloorToInt(cellCoordinate.x / _cellWidth);
            int y = Mathf.Abs(Mathf.FloorToInt(cellCoordinate.y / _cellHeight) - (_rowsCount - 1));
            index = _columnsCount * y + x;
            if (x >= 0 && y >= 0 && index < _cellsCount)
            {
                return true;
            }
            return false;
        }

        public bool TryGetCellIndexInDirection(Rect cellTransform, Direction direction, out int index)
        {
            Statics.Assert(()=>direction != Direction.None, $"Invalid direction {direction}");
            index = -1;
            Vector2 cellCoordinates = cellTransform.center + direction.ConvertDirectionToVector() * (direction is Direction.Down or Direction.Up ? CellHeight : CellWidth);
            cellCoordinates.x += _boardWidth * 0.5f;
            cellCoordinates.y += _boardHeight * 0.5f;
            if (cellCoordinates.x > _boardWidth || cellCoordinates.y > _boardHeight || cellCoordinates.x < 0f || cellCoordinates.y < 0f) return false;
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

        private void CheckBestEdge(
            Vector2 edgeStart, Vector2 edgeEnd, Vector2 point,
            ref Vector2 bestPoint, ref float bestSqrDistance)
        {
            var edgePoint = edgeStart;
 
            var edgeSegment = edgeEnd - edgeStart;
            var length = Vector2.SqrMagnitude(edgeSegment);
            if (length > Mathf.Epsilon)
                edgePoint = edgeStart + Mathf.Clamp01(Vector2.Dot(edgeSegment, point - edgeStart) / length) * edgeSegment;

            var sqrDistance = Vector2.SqrMagnitude(edgePoint - point);
            if (sqrDistance < bestSqrDistance)
            {
                bestPoint = edgePoint;
                bestSqrDistance = sqrDistance;
            }
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
                if (indexX == ColumnsCount)
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
            int remainingCells = (ColumnsCount - 1) * (RowsCount - 1) * 2;// because remaining cells will create 2 new lines and sharing one line from the cell on their right and one line from the cell above them
            _lines = new Line[firstCell + remainingFirstCellsOfColumns + remainingFirstCellOfRows + remainingCells];
        }
    }
}