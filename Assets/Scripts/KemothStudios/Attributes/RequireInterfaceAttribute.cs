using System;
using UnityEngine;

namespace KemothStudios.Utility.Attributes
{

    /// <summary>
    /// This attribute allows object references to be applied on to the target field if that reference is implementing the given type of interface
    /// </summary>
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; private set; }

        public RequireInterfaceAttribute(Type interfaceType) => InterfaceType = interfaceType;
    }
}