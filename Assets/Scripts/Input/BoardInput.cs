using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardInput : MonoBehaviour
{
    public static Action<Vector2> OnInput;

    private void Update()
    {
        Pointer pointer;
#if UNITY_EDITOR
        pointer = Mouse.current;
#else
        pointer = Touchscreen.current;
#endif
        if (pointer.press.wasPressedThisFrame)
        {
            Ray r = Camera.main.ScreenPointToRay(pointer.position.value);
            if (new Plane(Vector3.back, Vector3.zero).Raycast(r, out float hit))
            {
                Vector3 hitPoint = r.GetPoint(hit);
                OnInput?.Invoke(hitPoint);
            }
        }
    }
}
