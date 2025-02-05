using System;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios.Board
{
    public sealed class Cell
    {
        private Line[] _lines;
        private Rect _cellTransform;
        private BoardDataSO _boardData;
        private bool _cellCompleted;
        
        public Rect CellTransform => _cellTransform;
        public BoardDataSO BoardData => _boardData;

        public Cell(Vector3 position, Vector2 scale, BoardDataSO boardData)
        {
            _boardData = boardData;
            _cellTransform = new Rect(position.x - scale.x * 0.5f, position.y - scale.y * 0.5f, scale.x, scale.y);
            CreateLines();
        }

        ~Cell()
        {
            _boardData = null;
            _lines = null;
        }

        public void CellClicked(Vector2 clickPoint)
        {
            Line verticalLine = _lines[clickPoint.x > _cellTransform.center.x ? 1 : 3];
            Line horizontalLine = _lines[clickPoint.y > _cellTransform.center.y ? 0 : 2];
            Vector2 pointOnVerticalLine = new Vector2(verticalLine.LinePosition.x, clickPoint.y);
            Vector2 pointOnHorizontalLine = new Vector2(clickPoint.x, horizontalLine.LinePosition.y);

            if ((pointOnVerticalLine - clickPoint).sqrMagnitude > (pointOnHorizontalLine - clickPoint).sqrMagnitude)
                horizontalLine.LineClicked();
            else verticalLine.LineClicked();
        }

        private void CreateLines()
        {
            _lines = new Line[4];

            // TOP LINE, if we got a cell above us then get bottom line from that cell and add ourselves as shared cell else simply add new top line
            if (_boardData.TryGetCellIndexInDirection(_cellTransform, Direction.Up, out int topCellIndex))
            {
                Cell cell = _boardData.GetCell(topCellIndex);
                _lines[0] = cell.GetLineBottom;
                _lines[0].AddSharedCell(this);
            }
            else
                _lines[0] = new Line(CellTransform.center + Vector2.up * (CellTransform.height * 0.5f), new Vector3(CellTransform.width, 0.1f, 0.1f), this);

            // RIGHT LINE
            _lines[1] = new Line(CellTransform.center + Vector2.right * (CellTransform.width * 0.5f), new Vector3(0.1f, CellTransform.height, 0.1f), this);

            // BOTTOM LINE
            _lines[2] = new Line(CellTransform.center + Vector2.down * (CellTransform.height * 0.5f), new Vector3(CellTransform.width, 0.1f, 0.1f), this);

            // LEFT LINE, if we got a cell on our left then get right line from that cell and add ourselves as shared cell else simply add new left line
            if (_boardData.TryGetCellIndexInDirection(_cellTransform, Direction.Left, out int leftCellIndex))
            {
                Cell cell = _boardData.GetCell(leftCellIndex);
                _lines[3] = cell.GetLineRight;
                _lines[3].AddSharedCell(this);
            }
            else
                _lines[3] = new Line(CellTransform.center + Vector2.left * (CellTransform.width * 0.5f), new Vector3(0.1f, CellTransform.height, 0.1f), this);
        }

        public Line GetLineRight => _lines[1];
        public Line GetLineBottom => _lines[2];

        /// <summary>
        /// Checks if cell is completed
        /// </summary>
        public bool IsCellCompleted
        {
            get
            {
                if(_cellCompleted)
                    return true;
                bool result = true;
                foreach (Line line in _lines)
                {
                    if (!line.Clicked)
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Marks a cell completed and raises cell completed event
        /// </summary>
        public void MarkCellCompleted()
        {
            if(_cellCompleted) return;
            _cellCompleted = true;
            EventBus<CellAcquiredEvent>.RaiseEvent(new CellAcquiredEvent(this));
        }
    }
}