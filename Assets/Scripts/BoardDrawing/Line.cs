using System.Collections.Generic;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios.Board
{
    public sealed class Line
    {
        private Cell[] _sharedByCells;
        private Vector2 _linePosition;
        private Vector2 _lineScale;
        
        public bool Clicked{get; private set; }

        public Vector2 LinePosition => _linePosition;

        public Vector2 LineScale => _lineScale;

        public Line(Vector3 position, Vector2 scale, Cell cell)
        {
            _linePosition = position;
            _lineScale = scale;
            _sharedByCells = new Cell[] { cell };
            cell.BoardData.AddLineToCollection(this);
        }

        ~Line()
        {
            _sharedByCells = null;
        }

        public void LineClicked()
        {
            if (!Clicked)
            {
                Clicked = true;
                EventBus<DrawLineEvent>.RaiseEvent(new DrawLineEvent(this));
            }
        }

        // Because we know that one line can only have two shared cells that is why we are just updating array in this way
        public void AddSharedCell(Cell cell)
        {
            Cell cellAlreadyInCollection = _sharedByCells[0];
            _sharedByCells = new[] { cellAlreadyInCollection, cell };
        }

        public IEnumerable<Cell> SharedCells => _sharedByCells;
    }
}