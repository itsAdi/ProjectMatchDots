using System.Collections.Generic;
using System.Text;
using KemothStudios.Utility.Events;
using UnityEngine;

namespace KemothStudios.Board
{
    public class BoardLines : MonoBehaviour, IBoardGraphic
    {
        [SerializeField] private BoardDataSO _boardData;
        [SerializeField] private GameObject _linePrefab;

        private Dictionary<int, GameObject> _lineVisuals;
        private EventBinding<DrawLineEvent> _drawLineEvent;
        
        void Start()
        {
            _drawLineEvent = new EventBinding<DrawLineEvent>(ShowLine);
            EventBus<DrawLineEvent>.RegisterBinding(_drawLineEvent);
        }

        private void OnDestroy()
        {
            EventBus<DrawLineEvent>.UnregisterBinding(_drawLineEvent);
        }

        private void ShowLine(DrawLineEvent lineData)
        {
            _lineVisuals[lineData.Line.GetHashCode()].SetActive(true);
        }

        public void DrawBoardGraphic()
        {
            _lineVisuals = new();
            foreach (Line line in _boardData.Lines)
            {
                GameObject obj;
                if (_linePrefab != null) obj = Instantiate(_linePrefab);
                else
                {
                    obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(obj.GetComponent<Collider>());
                }
                obj.transform.position = line.LinePosition;
                obj.transform.localScale = new Vector3(line.LineScale.x, line.LineScale.y, 1f);
                obj.SetActive(false);
                _lineVisuals.Add(line.GetHashCode(), obj);
                obj.transform.parent = _boardData.BoardParent;
            }
        }
    }
}