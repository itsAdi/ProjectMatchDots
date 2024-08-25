using UnityEngine;

namespace KemothStudios.Board
{
    public sealed class Cell
    {
        private Line[] _lines;
        private Rect _cellTransform;
        private BoardDataSO _boardData;

        public Rect CellTransform => _cellTransform;
        public BoardDataSO BoardData => _boardData;

        public Cell(Vector3 position, Vector2 scale, BoardDataSO boardData)
        {
            _boardData = boardData;
            _cellTransform = new Rect(position.x - scale.x * 0.5f, position.y - scale.y * 0.5f, scale.x, scale.y);
            _boardData.TryGetCellIndex(_cellTransform.center, out int index);
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
            Vector2 pointOnHoriontalLine = new Vector2(clickPoint.x, horizontalLine.LinePosition.y);

            if ((pointOnVerticalLine - clickPoint).sqrMagnitude > (pointOnHoriontalLine - clickPoint).sqrMagnitude)
                horizontalLine.LineClicked();
            else verticalLine.LineClicked();
        }

        private void CreateLines()
        {
            _lines = new Line[4];

            // TOP LINE, if we got a cell above us then get bottom line from that cell and add ourselves as shared cell else simply add new top line
            if (_boardData.TryGetCellIndex(_cellTransform.center + Vector2.up * _boardData.CellHeight, out int topCellIndex))
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

            // LEFT LINE, if we got a cell on our left then get righ line from that cell and add ourselves as shared cell else simply add new left line
            if (_boardData.TryGetCellIndex(_cellTransform.center + Vector2.left * _boardData.CellWidth, out int leftCellIndex))
            {
                Cell cell = _boardData.GetCell(leftCellIndex);
                _lines[3] = cell.GetLineRigth;
                _lines[3].AddSharedCell(this);
            }
            else
                _lines[3] = new Line(CellTransform.center + Vector2.left * (CellTransform.width * 0.5f), new Vector3(0.1f, CellTransform.height, 0.1f), this);
        }

        public Line GetLineRigth => _lines[1];
        public Line GetLineBottom => _lines[2];

        public bool IsCellCompleted
        {
            get
            {
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
    }
}