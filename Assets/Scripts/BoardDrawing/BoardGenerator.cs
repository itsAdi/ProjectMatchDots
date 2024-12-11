using KemothStudios.Utility.Attributes;
using UnityEngine;

namespace KemothStudios.Board
{
    public partial class BoardGenerator : MonoBehaviour
    {
        [SerializeField] private BoardConfigSO _boardConfig;
        [SerializeField] private BoardDataSO _boardData;
        [SerializeField, RequireInterface(typeof(IBoardGraphic))] private Object[] _boardGraphicDrawer;

        private void Start()
        {
            _boardData.GenerateBoardData(_boardConfig.rows, _boardConfig.columns, _boardConfig.cellWidth, _boardConfig.cellHeight, transform);
            foreach (Object graphicDrawer in _boardGraphicDrawer)
            {
                ((IBoardGraphic)graphicDrawer).DrawBoardGraphic();
            }
        }

        private void OnDestroy()
        {
            _boardData.ClearBoardData();
        }
    }
}