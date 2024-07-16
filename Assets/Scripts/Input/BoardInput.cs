using KemothStudios.Board;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardInput : MonoBehaviour
{
    [SerializeField] private BoardDataSO _boardData;

    private void Update()
    {
        Pointer pointer;
#if UNITY_EDITOR
        pointer = Mouse.current;
#else
        pointer = Touchscreen.current;
#endif
        if (pointer.press.wasPressedThisFrame)
        {
            Ray r = Camera.main.ScreenPointToRay(pointer.position.value);
            if (new Plane(Vector3.back, Vector3.zero).Raycast(r, out float hit))
            {
                Vector3 hitPoint = r.GetPoint(hit);
                if(_boardData.TryGetCellIndex(hitPoint, out int cellIndex))
                    _boardData.GetCell(cellIndex).CellClicked();
            }
        }
    }
}
