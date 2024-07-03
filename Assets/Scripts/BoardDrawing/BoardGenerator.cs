using KemothStudios.Board;
using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios.Board
{
    public class BoardGenerator : MonoBehaviour
    {
        [SerializeField] private BoardConfigSO _boardConfig;
        [SerializeField] private Camera _camera;

        private Cell[] _cells;

        private void Start()
        {
            float totalBoardWidth = _boardConfig.cellWidth * _boardConfig.columns;
            float totalBoardHeight = _boardConfig.cellHeight * _boardConfig.rows;
            int cellCount = _boardConfig.rows * _boardConfig.columns;
            _cells = new Cell[cellCount];
            int indexX = 0;
            int indexY = 0;
            Vector3 initialPosition = new Vector3((-totalBoardWidth * 0.5f) + (_boardConfig.cellWidth * 0.5f), (totalBoardHeight * 0.5f) - (_boardConfig.cellHeight * 0.5f), 0f);
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = initialPosition;
            for (int i = 0; i < cellCount; i++)
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

            SetCameraSize(totalBoardWidth, totalBoardHeight);
        }

        private void SetCameraSize(float boardWidth, float boardHeight)
        {
            float screenRatio = (float)Screen.width / Screen.height;
            float targetRatio = boardWidth / boardHeight;
            if (screenRatio >= targetRatio)
                _camera.orthographicSize = boardHeight / 2f;
            else
            {
                float difference = targetRatio/screenRatio;
                _camera.orthographicSize = boardHeight/2f*difference;
            }
        }

        private class Cell
        {
            public Cell(Vector3 position)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.position = position;
                obj.transform.localScale = Vector3.one * 0.5f;
            }
        }
    }
}