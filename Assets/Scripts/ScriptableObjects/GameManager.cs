using KemothStudios.Board;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KemothStudios
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Action<Line> OnLineClicked;
        public Action<IEnumerable<Cell>> OnCellCompleted;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameStates.Instance.CurrentState = GameStates.States.MAIN_MENU;
        }
    }
}