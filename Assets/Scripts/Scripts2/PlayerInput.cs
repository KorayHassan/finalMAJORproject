using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    public event Action<string> OnButtonPressed;

    void Update()
    {
        // Numpad grid
        CheckKey(KeyCode.Keypad9, "9");
        CheckKey(KeyCode.Keypad8, "8");
        CheckKey(KeyCode.Keypad7, "7");

        CheckKey(KeyCode.Keypad6, "6");
        CheckKey(KeyCode.Keypad5, "5");
        CheckKey(KeyCode.Keypad4, "4");

        CheckKey(KeyCode.Keypad3, "3");
        CheckKey(KeyCode.Keypad2, "2");
        CheckKey(KeyCode.Keypad1, "1");

        // Spacebar (controller / universal input)
        CheckKey(KeyCode.Space, "Space");
    }

    void CheckKey(KeyCode key, string button)
    {
        if (Input.GetKeyDown(key))
        {
            OnButtonPressed?.Invoke(button);
        }
    }
}