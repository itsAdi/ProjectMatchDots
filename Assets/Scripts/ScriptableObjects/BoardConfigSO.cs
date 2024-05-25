using UnityEngine;

namespace KemothStudios.Board
{
    public class BoardConfigSO : ScriptableObject
    {
        [SerializeField, Min(1)] private int _rows = 1;
        [SerializeField, Min(1)] private int _columns = 1;
        [SerializeField, Min(0f)] private float _cellWidth = 1f;
        [SerializeField, Min(0f)] private float _cellHeight = 1f;

        public int rows => _rows;
        public int columns => _columns;
        public float cellWidth => _cellWidth;
        public float cellHeight => _cellHeight;
    }
}