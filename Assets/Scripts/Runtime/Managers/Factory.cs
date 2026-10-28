using System;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using Runtime.Data;
using UnityEngine;

namespace Runtime.Managers
{
    public class Factory
    {
        private static GlobalConfig _globalConfig;
        private static Transform _defaultParent;
        private static bool _isInitialized;
        
        
        private static readonly Dictionary<Type,List<GameObject>> _spawnedObjectsByType = new();
        private static readonly Dictionary<Type,List<GameObject>> _destroyedObjectsByType = new();


        public static void Initialize(GlobalConfig globalConfig, Transform defaultParent = null)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("Already Initialized");
            }
            
            _globalConfig = globalConfig;
            _defaultParent = defaultParent;
            _isInitialized = true;
        }


        public static T Create<T>(T prefab, Transform parent = null, bool usePooling = true) where T : Component
        {
            CheckInitialized();

            if (!prefab)
            {
                Debug.LogWarning("prefab is null");
                return null;
            }

            T instance;
            Transform targetParent = parent ?? _defaultParent;

            if (usePooling)
            {
                instance = LeanPool.Spawn(prefab, targetParent);
            }
            else
            {
                instance = UnityEngine.Object.Instantiate(prefab, parent);
            }

            if (!instance)
            {
                Debug.LogWarning("instance is null");
                return null;
            }
            
            TrackObject<T>(instance.gameObject,usePooling);
            return instance;
        }
        
        public static GameObject Create(GameObject prefab, Transform parent = null, bool usePooling = true)
        {
            CheckInitialized();

            if (!prefab)
            {
                Debug.LogWarning("prefab is null");
                return null;
            }

            GameObject instance;
            Transform targetParent = parent ?? _defaultParent;

            if (usePooling)
            {
                instance = LeanPool.Spawn(prefab, targetParent);
            }
            else
            {
                instance = UnityEngine.Object.Instantiate(prefab, parent);
            }

            if (!instance)
            {
                Debug.LogWarning("instance is null");
                return null;
            }
            
            TrackObject<GameObject>(instance.gameObject,usePooling);
            
            return instance;
        }

        private static void TrackObject<T>(GameObject obj, bool usePooling)
        {
            if (!obj) return;
            
            Type type = typeof(T);
            Dictionary<Type, List<GameObject>> targetDictionary = usePooling ? _spawnedObjectsByType : _destroyedObjectsByType;

            if (!targetDictionary.ContainsKey(type))
            {
                targetDictionary[type] = new  List<GameObject>();
            }
            
            targetDictionary[type].Add(obj);
        }

        public static void RemoveTrackedObject<T>(GameObject obj, bool usePooling = true)
        {
            if (!obj) return;
            
            Type type = typeof(T);
            Dictionary<Type,List<GameObject>> targetDictionary = usePooling ? _spawnedObjectsByType : _destroyedObjectsByType;

            if (targetDictionary.ContainsKey(type))
            {
                targetDictionary[type].Remove(obj);
            }
        }

        public static List<GameObject> GetTrackedObjects<T>(bool usePooling = true)
        {
            Type type = typeof(T);
            Dictionary<Type,List<GameObject>> targetDictionary = usePooling ? _spawnedObjectsByType : _destroyedObjectsByType;

            if (targetDictionary.ContainsKey(type))
            {
                return new List<GameObject>(targetDictionary[type]);
            }
            
            return new List<GameObject>();
        }

        public static void DestroyAll()
        {
            foreach (var pair in  _spawnedObjectsByType)
            {
                foreach (var obj in pair.Value)
                {
                    if (obj)
                    {
                        obj.transform.DOKill();
                        LeanPool.Despawn(obj);
                    }
                }
                pair.Value.Clear();
            }
            
            foreach (var pair in  _destroyedObjectsByType)
            {
                foreach (var obj in pair.Value)
                {
                    if (obj)
                    {
                        obj.transform.DOKill();
                        UnityEngine.Object.Destroy(obj);
                    }
                }
                pair.Value.Clear();
            }
            
            _spawnedObjectsByType.Clear();
            _destroyedObjectsByType.Clear();
        }

        public static void DestroyAll<T>(bool usePooling = true)
        {
            Type type = typeof(T);
            Dictionary<Type,List<GameObject>> targetDictionary = usePooling ? _spawnedObjectsByType : _destroyedObjectsByType;

            if (targetDictionary.ContainsKey(type))
            {
                foreach (var obj in targetDictionary[type])
                {
                    if (obj)
                    {
                        obj.transform.DOKill();

                        if (usePooling)
                        {
                            LeanPool.Despawn(obj);
                        }
                        else
                        {
                            UnityEngine.Object.Destroy(obj);
                        }
                    }
                }
                targetDictionary[type].Clear();
            }
        }

        public static void Destroy(GameObject obj, bool usePooling = true)
        {
            if (!obj) return;
            
            obj.transform.DOKill();

            if (usePooling)
            {
                LeanPool.Despawn(obj);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
        
            
        public static void Dispose()
        {
            if (!_isInitialized) return;

            DestroyAll();
            
            _globalConfig = null;
            _defaultParent = null;
            _isInitialized = false;
        }

        private static void CheckInitialized()
        {
            if (!_isInitialized)
            {
                Debug.LogError("Factory Not Initialized");
                throw new InvalidOperationException("Factory Not Initialized,Call Initialize()");
            }
        }
    }
}
