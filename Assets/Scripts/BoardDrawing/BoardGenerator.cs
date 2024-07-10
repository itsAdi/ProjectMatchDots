using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace KemothStudios.Board
{
    public class BoardGenerator : MonoBehaviour
    {
        [SerializeField] private BoardConfigSO _boardConfig;

        private Cell[] _cells;
        private int _cellsCount;

        private void Start()
        {
            float totalBoardWidth = _boardConfig.cellWidth * _boardConfig.columns;
            float totalBoardHeight = _boardConfig.cellHeight * _boardConfig.rows;
            _cellsCount = _boardConfig.rows * _boardConfig.columns;
            _cells = new Cell[_cellsCount];
            int indexX = 0;
            int indexY = 0;
            Vector3 initialPosition = new Vector3((-totalBoardWidth * 0.5f) + (_boardConfig.cellWidth * 0.5f), (totalBoardHeight * 0.5f) - (_boardConfig.cellHeight * 0.5f), 0f);
            for (int i = 0; i < _cellsCount; i++)
            {
                float posX = initialPosition.x + (_boardConfig.cellWidth * indexX);
                float posY = initialPosition.y - (_boardConfig.cellHeight * indexY);
                _cells[i] = new Cell(new Vector3(posX, posY, initialPosition.z));
                indexX++;
                if (indexX == _boardConfig.rows)
                {
                    indexX = 0;
                    indexY++;
                }
            }

            BoardInput.OnInput += BoardCellClicked;
        }

        private void OnDestroy()
        {
            BoardInput.OnInput -= BoardCellClicked;
        }

        private void BoardCellClicked(Vector2 vector)
        {
            float totalBoardWidth = _boardConfig.cellWidth * _boardConfig.columns;
            float totalBoardHeight = _boardConfig.cellHeight * _boardConfig.rows;
            vector.x += totalBoardWidth * 0.5f;
            vector.y += totalBoardHeight * 0.5f;
            int x = Mathf.FloorToInt(vector.x / _boardConfig.cellWidth);
            int y = Mathf.Abs(Mathf.FloorToInt(vector.y / _boardConfig.cellHeight) - (_boardConfig.rows - 1));
            int index = _boardConfig.columns * y + x;
            if(x >= 0 && y >= 0 && index < _cellsCount)
            {
                _cells[index].CellClicked();
            }
        }

        private class Cell
        {
            private GameObject _cellVisuals;
            public Cell(Vector3 position)
            {
                _cellVisuals = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _cellVisuals.transform.position = position;
                _cellVisuals.transform.localScale = Vector3.one * 0.5f;
            }

            public void CellClicked()
            {
                _cellVisuals.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }
}