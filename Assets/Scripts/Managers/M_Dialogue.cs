using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class M_Dialogue : Singleton
{
    InputControls _controls;

    PlayerInput _input;

    bool _usingGamePad;

    private void Start()
    {
        _input = Get<PlayerInput>();
        _controls = _input.Controls;

        _usingGamePad = Gamepad.current != null;
    }

    #region USING GAMEPAD
    private void Update()
    {
        _usingGamePad = (_usingGamePad && !KeyBoardAnyKey()) || GamePadAnyKey();
    }

    bool GamePadAnyKey()
    {
        if (Gamepad.current == null)
            return false;

        bool gamePadAnyKeyPressed = Gamepad.current.allControls.Any(x => x is ButtonControl button && button.isPressed && !button.synthetic);
        return gamePadAnyKeyPressed;            
    }

    bool KeyBoardAnyKey()
    {
        if (Keyboard.current == null)
            return false;

        return Keyboard.current.anyKey.isPressed;
    }
    #endregion

    public string ReplaceWithCorrectButtons(string text)
    {
        string newText = text;

        while (newText.Contains("{"))
        {
            int startIndex = newText.LastIndexOf("{");
            int endIndex = newText.LastIndexOf("}");

            string subString = newText.Substring(startIndex + 1, endIndex - startIndex - 1);
            string button = KeyBindToButton(subString);

            newText = newText.Remove(startIndex, endIndex - startIndex + 1)
                .Insert(startIndex, button);
        }

        return newText;
    }

    public string KeyBindToButton(string action)
    {
        InputAction inputAction = _controls.FindAction(action);

        if (inputAction == null)
            throw new System.Exception("Action not found: " + action);

        string binding = GetCorrectBinding(inputAction.bindings).effectivePath;

        binding = binding.Split('/')[1];

        binding = char.ToUpper(binding[0]) + binding.Substring(1);

        return binding;
    }

    InputBinding GetCorrectBinding(UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputBinding> bindingArray)
    {
        foreach (InputBinding binding in bindingArray)
        {
            bool gamepad = binding.effectivePath.Contains("<Gamepad>") && _usingGamePad;
            bool keyboard = binding.effectivePath.Contains("<Keyboard>") && !_usingGamePad;

            if (gamepad || keyboard)
                return binding;
        }

        throw new System.Exception("Keybind not found in array");
    }
}
