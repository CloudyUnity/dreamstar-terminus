using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class M_Rebinding : Singleton
{
    [SerializeField] bool _customRebinds = true;

    [System.Serializable]
    public struct TextIARPair
    {
        public TMP_Text Text;
        public InputActionReference IAR;
    }

    [SerializeField] List<TextIARPair> _textIARPairList = new List<TextIARPair>();

    InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
    InputControls _controls;

    const bool DEBUG_MODE = false;

    private void Start()
    {
        _controls = Get<PlayerInput>().Controls;

        string rebinds = PlayerPrefs.GetString("rebinds");

        if ((!string.IsNullOrEmpty(rebinds) && _customRebinds) || DEBUG_MODE)
        {
            _controls.LoadBindingOverridesFromJson(rebinds);
        }

        SetText();
    }

    void SetText()
    {
        foreach (var pair in _textIARPairList)
        {
            InputAction action = _controls.FindAction(pair.IAR.action.id.ToString());
            pair.Text.text = KeybindToText(action);
        }
    }

    public void Save()
    {
        string rebinds = _controls.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("rebinds", rebinds);

        Debug.Log("New bindings saved");
    }

    public void StartRebinding(InputActionReference iar)
    {
        InputAction action = _controls.FindAction(iar.action.id.ToString());

        TextIARPair pair = FindPair(iar);

        GameObject button = FindButton(pair);

        if (!button.activeSelf)
            return;

        button.SetActive(false);

        _controls.Disable();

        _rebindingOperation = action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(x => RebindComplete(pair, action))
            .Start();

        #region ADD BINDINGS ZOMBIE
        //InputAction anyInputAction = new InputAction("AnyInputAction");
        //anyInputAction.performed += ctx =>
        //{
        //    var binding = anyInputAction.bindings[0].effectivePath;
        //    iar.action.AddBinding(binding);
        //    RebindComplete(pair);
        //    Debug.Log("Binding Added");
        //};
        //anyInputAction.Enable();
        #endregion
    }

    private void RebindComplete(TextIARPair pair, InputAction action)
    {
        string txt = KeybindToText(action);
        pair.Text.text = txt;

        Debug.Log("Keybind changed to: " + txt);

        _rebindingOperation.Dispose();

        _controls.Enable();

        FindButton(pair).SetActive(true);

        Save();
    }

    public void ClearBindings(InputActionReference iar)
    {
        InputAction action = _controls.FindAction(iar.action.id.ToString());

        action.RemoveAllBindingOverrides();

        TextIARPair pair = FindPair(iar);
        pair.Text.text = KeybindToText(action);

        Save();
    }

    string KeybindToText(InputAction action)
    {
        int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);

        string path = action.bindings[bindingIndex].effectivePath;

        return InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.UseShortNames);
    }

    TextIARPair FindPair(InputActionReference iar)
    {
        foreach (var pair in _textIARPairList)
        {
            if (pair.IAR == iar)
                return pair;
        }

        throw new System.Exception("TEXT PAIR NOT FOUND");
    }

    GameObject FindButton(TextIARPair pair) => pair.Text.transform.parent.gameObject;
}
