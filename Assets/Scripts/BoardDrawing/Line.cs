using UnityEngine;

namespace KemothStudios.Board
{
    public sealed class Line
    {
        private Material _lineMaterial;
        private Cell[] _sharedByCells;

        public Line(Vector3 position, Vector3 scale, Cell cell)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.transform.position = position;
            line.transform.localScale = scale;
            _lineMaterial = line.GetComponent<Renderer>().material;
            _sharedByCells = new Cell[] { cell, null };
            line.name = "Line";
        }

        ~Line()
        {
            _sharedByCells = null;
            _lineMaterial = null;
        }

        public void LineClicked()
        {
            _lineMaterial.color = Color.green;
        }

        public void AddSharedCell(Cell cell) => _sharedByCells[1] = cell;
    }
}