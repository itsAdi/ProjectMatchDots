using UnityEngine;

namespace KemothStudios
{
    [CreateAssetMenu(fileName = "PlayerTimeConfig", menuName = "KemothStudios/PlayerTimeConfig")]
    public class PlayerTimeConfigSO : ScriptableObject
    {
        [SerializeField, Tooltip("Total time in seconds a player has for its turns"), Min(1)] private int _maxPlayerTime = 90;
        
        public int MaxPlayerTime => _maxPlayerTime;
    }
}