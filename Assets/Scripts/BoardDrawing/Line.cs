using UnityEngine;

namespace KemothStudios.Board
{
    public sealed class Line
    {
        private Cell[] _sharedByCells;
        private Vector2 _linePosition;
        private Vector2 _lineScale;
        private bool _clicked;

        public Vector2 LinePosition => _linePosition;

        public Vector2 LineScale => _lineScale;

        public Line(Vector3 position, Vector2 scale, Cell cell)
        {
            _linePosition = position;
            _lineScale = scale;
            _sharedByCells = new Cell[] { cell, null };
            cell.BoardData.AddLineToCollection(this);
        }

        ~Line()
        {
            _sharedByCells = null;
        }

        public void LineClicked()
        {
            if (!_clicked)
            {
                _clicked = true;
                GameManager.Instance.OnLineClicked(this);
            }
        }

        public void AddSharedCell(Cell cell) => _sharedByCells[1] = cell;
    }
}