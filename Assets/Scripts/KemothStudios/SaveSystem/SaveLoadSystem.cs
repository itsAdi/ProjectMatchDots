using System;
using System.IO;
using UnityEngine;

namespace KemothStudios.Utility.SaveSystem
{
    /// <summary>
    /// Data to save/load from
    /// </summary>
    public interface ISaveData
    {
        string FileName { get; }
    }

    public sealed class FileDataService<TData, TSerializer> : IDataService<TData> where TSerializer : ISerializer<TData> where TData : class, ISaveData
    {
        private TSerializer _serializer;
        private TData _data;
        private string _filePath;
        private string _fileExtention;

        public FileDataService(TData data, TSerializer serializer)
        {
            _serializer = serializer;
            _filePath = Application.persistentDataPath;
            _fileExtention = ".json";
            _data = data;
        }
        
        private string GetFilePath() => Path.Combine(_filePath, string.Concat(_data.FileName, _fileExtention));

        public void Save(bool overwrite = false)
        {
            try
            {
                string path = GetFilePath();
                if(!overwrite && File.Exists(path))
                    throw new IOException($"file {path} already exists and cannot be overwritten.");
                File.WriteAllText(path, _serializer.Serialize(_data));
            }
            catch (Exception e)
            {
                DebugUtility.LogException(e);
                throw;
            }
        }

        public TData Load()
        {
            try
            {
                string path = GetFilePath();
                if(!File.Exists(path))
                    throw new FileNotFoundException($"Loading data failed because file {path} not found.");
                _data = _serializer.Deserialize(File.ReadAllText(path));
                return _data;
            }
            catch (Exception e)
            {
                DebugUtility.LogException(e);
                throw;
            }
        }

        public bool DataExists()
        {
            try
            {
                return File.Exists(GetFilePath());
            }
            catch (Exception e)
            {
                DebugUtility.LogException(e);
                throw;
            }
        }
    }

    public sealed class JSONSerializer<T> : ISerializer<T>
    {
        public string Serialize(T target)
        {
            try
            {
                string json = JsonUtility.ToJson(target);
                return json;
            }
            catch (Exception e)
            {
                DebugUtility.LogException(e);
                throw;
            }
        }

        public T Deserialize(string serializedData)
        {
            try
            {
                T data = JsonUtility.FromJson<T>(serializedData);
                return data;
            }
            catch (Exception e)
            {
                DebugUtility.LogException(e);
                throw;
            }
        }
    }
}