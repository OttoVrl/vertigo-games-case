using Runtime.Data;
using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "GlobalConfig", menuName = "Data/Config/GlobalConfig", order = 0)]
    public class GlobalConfig : ScriptableObject
    {
        [SerializeField] private GameDatabaseConfig databaseConfig;
        [SerializeField] private SpinDatabaseConfig spinDatabaseConfig;
        [SerializeField] private AudioConfig audioConfig;
        
        public GameDatabaseConfig DatabaseConfig => databaseConfig;
        public SpinDatabaseConfig SpinDatabaseConfig => spinDatabaseConfig;
        
        public AudioConfig AudioConfig => audioConfig;
    }
}
