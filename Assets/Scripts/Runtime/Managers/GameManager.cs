using System;
using Runtime.Data;
using Runtime.Enums;
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

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            //...
            StateManager.Initialize(gameStateReference,popUpStateReference);
            
            uiManager.Initialize(globalConfig);
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
