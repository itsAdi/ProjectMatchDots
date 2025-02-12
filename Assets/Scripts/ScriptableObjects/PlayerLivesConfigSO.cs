using UnityEngine;

namespace KemothStudios
{
    [CreateAssetMenu(fileName = "PlayerLivesConfig", menuName = "KemothStudios/PlayerLivesConfig")]
    public class PlayerLivesConfigSO : ScriptableObject
    {
        [SerializeField, Min(1)] private int _maxLives = 5;
        [SerializeField, Min(0f)] private float _timeToDeductLife = 5f;
        
        public int MaxLives => _maxLives;
        public float TimeToDeductLife => _timeToDeductLife;
    }
}