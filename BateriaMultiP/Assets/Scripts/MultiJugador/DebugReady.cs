using UnityEngine;
using UnityEngine.InputSystem;

public class DebugReady : MonoBehaviour
{
    public MatchStarter starter;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            starter.PressReady();
    }
}