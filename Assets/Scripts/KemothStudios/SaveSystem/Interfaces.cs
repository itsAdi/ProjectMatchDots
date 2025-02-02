using UnityEngine;

namespace KemothStudios.Utility.SaveSystem
{
    /// <summary>
    /// Serializer for data
    /// </summary>
    public interface ISerializer<TDataType>
    {
        string Serialize(TDataType target);
        TDataType Deserialize(string serializedData);
    }

    /// <summary>
    /// Service to process data
    /// </summary>
    public interface IDataService<TDataType>
    {
        void Save(bool overwrite = false);
        TDataType Load();
        bool DataExists();
    }
}