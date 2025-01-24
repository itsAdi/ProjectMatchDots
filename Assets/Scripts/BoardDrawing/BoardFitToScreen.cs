using UnityEngine;

namespace KemothStudios.Board
{
    public class BoardFitToScreen : MonoBehaviour
    {
        [SerializeField] private BoardConfigSO _boardConfig;
        
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            
            float boardWidth = _boardConfig.cellWidth * _boardConfig.columns + 5f;
            float boardHeight = _boardConfig.cellHeight * _boardConfig.rows + 5f;
            float screenRatio = (float)Screen.width / Screen.height;
            float targetRatio = boardWidth / boardHeight;
            if (screenRatio >= targetRatio)
                _camera.orthographicSize = boardHeight / 2f;
            else
            {
                float difference = targetRatio / screenRatio;
                _camera.orthographicSize = boardHeight / 2f * difference;
            }
        }
    }
}