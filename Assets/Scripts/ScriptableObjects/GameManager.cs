using KemothStudios.Board;
using System;
using UnityEngine;

namespace KemothStudios
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Action<Line> OnLineClicked;

        private void Awake()
        {
            Instance = this;
        }
    }
}