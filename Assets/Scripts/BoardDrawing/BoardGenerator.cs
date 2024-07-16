using UnityEngine;

namespace KemothStudios.Board
{
    public partial class BoardGenerator : MonoBehaviour
    {
        [SerializeField] private BoardConfigSO _boardConfig;
        [SerializeField] private BoardDataSO _boardData;

        private Cell[] _cells;
        private int _cellsCount;
        private Cell _lastCell;

        private void Start()
        {
            _boardData.GenerateBoardData(_boardConfig.rows, _boardConfig.columns, _boardConfig.cellWidth, _boardConfig.cellHeight);   
        }
    }
}