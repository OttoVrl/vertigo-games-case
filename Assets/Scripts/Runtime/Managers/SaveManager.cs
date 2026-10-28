using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Runtime.Managers
{
    public static class SaveManager
    {
        public static void SaveData<T>(T obj, string fileName, string directory)
        {
            var path = Path.Combine(directory, fileName);

            var binaryFormatter = new BinaryFormatter();

            using (var fileStream = File.Create(path))
            {
                try
                {
                    binaryFormatter.Serialize(fileStream, obj);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    throw;
                }
                finally
                {
                    fileStream.Close();
                }
            }
        }

        public static void SaveData<T>(T obj, string fileName)
        {
            SaveData(obj, fileName, Application.persistentDataPath);
        }


        public static T LoadData<T>(string fileName, string directory)
        {
            var path = Path.Combine(directory, fileName);

            var binaryFormatter = new BinaryFormatter();

            using (var fileStream = File.OpenRead(path))
            {
                T data;

                try
                {
                    data = (T)binaryFormatter.Deserialize(fileStream);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    throw;
                }
                finally
                {
                    fileStream.Close();
                }
                
                return data;
            }
        }

        public static T LoadData<T>(string fileName)
        {
            return LoadData<T>(fileName, Application.persistentDataPath);
        }
        
        
        
    }
}