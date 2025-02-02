using System;
using UnityEngine;

namespace KemothStudios.Utility
{
    [Serializable]
    public class SerializableGUID : IEquatable<SerializableGUID>
    {
        [SerializeField, HideInInspector]private uint _part1, _part2, _part3, _part4;

        public SerializableGUID(uint part1, uint part2, uint part3, uint part4)
        {
            _part1 = part1;
            _part2 = part2;
            _part3 = part3;
            _part4 = part4;
        }

        public static SerializableGUID NewGUID()
        {
            byte[] guidBytes = Guid.NewGuid().ToByteArray();
            return new SerializableGUID(
                BitConverter.ToUInt32(guidBytes, 0),
                BitConverter.ToUInt32(guidBytes, 4),
                BitConverter.ToUInt32(guidBytes, 8),
                BitConverter.ToUInt32(guidBytes, 12));
        }
        
        // Equality Checks
        public override bool Equals(object obj) => obj is SerializableGUID guid && Equals(guid);
        public bool Equals(SerializableGUID other) => other._part1 == _part1 && other._part2 == _part2 && other._part3 == _part3 && other._part4 == _part4;
        public override int GetHashCode() => HashCode.Combine(_part1, _part2, _part3, _part4);
        public static bool operator ==(SerializableGUID left, SerializableGUID right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }
        public static bool operator !=(SerializableGUID left, SerializableGUID right) => !(left == right);
    }
}