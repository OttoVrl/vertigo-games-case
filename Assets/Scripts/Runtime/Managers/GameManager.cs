using Runtime.Data;
using UnityEngine;

namespace Runtime.Managers
{
    public sealed class GameManager : MonoBehaviour
    {
        [Header("State Management")] 
        [SerializeField] private GameStateReference gameStateReference;
        
        [SerializeField] private PopUpStateReference popUpStateReference;

        [Header("Config")] 
        [SerializeField] private GlobalConfig globalConfig;
        
        [Header("Managers")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private SfxManager sfxManager;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            //...
            StateManager.Initialize(gameStateReference,popUpStateReference);
            DataSaver.Initialize(globalConfig);
            Factory.Initialize(globalConfig);
            
            
            uiManager.Initialize(globalConfig);
            sfxManager.Initialize(globalConfig);
            //..
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void OnApplicationQuit()
        {
            Dispose();
        }

        private void Dispose()
        {
            StateManager.Dispose();
            uiManager.Dispose();
        }
    }
}
