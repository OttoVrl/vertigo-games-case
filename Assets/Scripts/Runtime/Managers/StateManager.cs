
using System;
using Runtime.Data;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Managers
{
    public static class StateManager
    {
       private static GameStateReference _gameStateRef;
       private static PopUpStateReference _popupStateRef;
       private static bool _isInitialized;
       
       public static event Action<GameState> OnGameStateChanged;
       public static event Action<PopUpState> OnPopUpStateChanged;

       public static GameState GameState
       {
           get
           {
               CheckIsInitialized();
               return _gameStateRef != null ? _gameStateRef.Value : GameState.None;
           }
           private set
           {
               CheckIsInitialized();
               if (_gameStateRef != null)
               {
                   var currentState = _gameStateRef.Value;
                   if (currentState != value)
                   {
                       Debug.Log($"state changed from {currentState} to {value}");
                       _gameStateRef.Value = value;
                   }
                   else
                   {
                       Debug.LogWarning($"gamestate is already {value}");
                   }
               }
           }
       }

       public static PopUpState PopUpState
       {
           get
           {
               CheckIsInitialized();
               return _popupStateRef != null ? _popupStateRef.Value : PopUpState.None;
           }
           private set
           {
               CheckIsInitialized();
               if (_popupStateRef != null && _popupStateRef.Value != value)
               {
                   _popupStateRef.Value = value;
                   OnPopUpStateChanged?.Invoke(value);
               }
           }
       }
       
       
       public static void Initialize(GameStateReference gameStateRef, PopUpStateReference popUpStateRef)
       {
           if (_isInitialized)
           {
               Debug.LogWarning("State Managee already  initialized");
               return;
           }
           
           _gameStateRef = gameStateRef;
           _popupStateRef = popUpStateRef;

           if (_gameStateRef != null)
           {
               _gameStateRef.UpdateEvent += OnGameStateRefChanged;
           }

           if (_popupStateRef != null)
           {
               _popupStateRef.UpdateEvent += OnPopUpStateRefChanged;
           }
           _isInitialized = true;
       }

       
       public static void SetGameState(GameState gameState) => GameState = gameState;
       public static void SetPopUpState(PopUpState popUpState) => PopUpState = popUpState;
       public static GameState GetGameState() => GameState;
       public static PopUpState GetPopUpState() => PopUpState;
       
       
       private static void ResetGameState()
       {
           CheckIsInitialized();
           _gameStateRef?.Reset();
       }

       private static void ResetPopUpState()
       {
           CheckIsInitialized();
           _popupStateRef?.Reset();
       }
       
       private static void ResetAll()
       {
           ResetGameState();
           ResetPopUpState();
       }

       private static void CheckIsInitialized()
       {
           if (!_isInitialized)
           {
               Debug.LogError("state Manager has not been initialized");
               throw new Exception("call Initialize()");
           }
       }
       
       private static void OnGameStateRefChanged(GameState newState)
       {
           if (OnGameStateChanged != null)
           {
               var subscribers = OnGameStateChanged.GetInvocationList();
               foreach (var subscriber in subscribers)
               {
                   try
                   {
                       ((Action<GameState>)subscriber).Invoke(newState);
                   }
                   catch (Exception ex)
                   {
                       Debug.LogError("Error invoking");
                   }
               }
           }
       }

       private static void OnPopUpStateRefChanged(PopUpState newState)
       {
           OnPopUpStateChanged?.Invoke(newState);
       }
       
       public static void Dispose()
       {
           if (!_isInitialized) return;
           
           if(_gameStateRef != null)
               _gameStateRef.UpdateEvent -= OnGameStateRefChanged;
           if (_popupStateRef != null)
               _popupStateRef.UpdateEvent -= OnPopUpStateRefChanged;
           
           _gameStateRef = null;
           _popupStateRef = null;
           _isInitialized = false;
           
           OnGameStateChanged = null;
           OnPopUpStateChanged = null;
       }
    }
}
