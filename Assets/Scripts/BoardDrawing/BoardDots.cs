using UnityEngine;

namespace KemothStudios.Board
{
    public class BoardDots : MonoBehaviour, IBoardGraphic
    {
        [SerializeField] private BoardDataSO _boardData;
        public void DrawBoardGraphic()
        {
            Cell cell = _boardData.GetCell(0);
            Vector3 position = new Vector3(cell.CellTransform.x, cell.CellTransform.y + cell.CellTransform.height);
            float originalY = position.y;
            for (int x = 0; x <= _boardData.ColumnsCount; x++)
            {
                for (int y = 0; y <= _boardData.RowsCount; y++)
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Destroy(obj.GetComponent<Collider>());
                    obj.transform.position = position;
                    obj.transform.localScale = Vector3.one * 0.2f;
                    position.y -= cell.CellTransform.height;
                }
                position.x += cell.CellTransform.width;
                position.y = originalY;
            }
        }
    }
}