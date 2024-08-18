using UnityEngine;

namespace KemothStudios.Board
{
    public class BoardDots : MonoBehaviour, IBoardGraphic
    {
        [SerializeField] private BoardDataSO _boardData;
        [SerializeField] private GameObject _dotPrefab;
        public void DrawBoardGraphic()
        {
            Cell cell = _boardData.GetCell(0);
            Vector3 position = new Vector3(cell.CellTransform.x, cell.CellTransform.y + cell.CellTransform.height);
            float originalY = position.y;
            for (int x = 0; x <= _boardData.ColumnsCount; x++)
            {
                for (int y = 0; y <= _boardData.RowsCount; y++)
                {
                    GameObject obj;
                    if (_dotPrefab != null) obj = Instantiate(_dotPrefab);
                    else
                    {
                        obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Destroy(obj.GetComponent<Collider>());
                    }
                    obj.transform.position = position;
                    obj.transform.localScale = Vector3.one * 0.2f;
                    position.y -= cell.CellTransform.height;
                    obj.transform.parent = _boardData.BoardParent;
                }
                position.x += cell.CellTransform.width;
                position.y = originalY;
            }
        }
    }
}