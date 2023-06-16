using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_Keybinds : Singleton
{
    [SerializeField] List<KeyBind> _defaults = new List<KeyBind>();
    public bool Waiting;

    [ButtonInvoke(nameof(ResetKeys))] public bool ResetTheKeys;

    [System.Serializable]
    public struct KeyBind
    {
        public string Name;
        public string Code;
    }

    protected override void Awake()
    {
        GetKeys();
        base.Awake();
    }

    void GetKeys()
    {
        foreach (var bind in _defaults)
        {
            if (!PlayerPrefs.HasKey(bind.Name))
                PlayerPrefs.SetString(bind.Name, bind.Code);
        }
    }

    public KeyCode GetKey(string name) => (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(name));

    public void AssignKey(int n) => StartCoroutine(C_AssignKey(n));

    IEnumerator C_AssignKey(int n)
    {
        Waiting = true;

        KeyCode newKey = KeyCode.None;
        while (newKey == KeyCode.None)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    newKey = vKey;
                    break;
                }
            }
            yield return null;
        }

        Waiting = false;
        PlayerPrefs.SetString(_defaults[n].Name, newKey.ToString());
        M_Events.IvkReassignKeyCodes();
    }

    public void ResetKeys()
    {
        foreach (var bind in _defaults)
            PlayerPrefs.SetString(bind.Name, bind.Code);

        GetKeys();
    }

    //private void Update()
    //{
    //    if (V_HUDManager.Instance != null && !V_HUDManager.Instance.IsPaused)
    //        return;

    //    for (int i = 0; i < _defaults.Length; i++)
    //    {
    //        KeyBind bind = _defaults[i];
    //        _defaults[i].Text.text = bind.Name + " : " + (bind.Waiting ? "" : _keyBinds[i].Code.ToString());

    //        bool covered = V_UIManager.Instance.IsHovered(bind.Text.gameObject);
    //        bind.Text.black.color = covered ? Color.yellow : Color.black;
    //        MoveOption(bind.Text.parentGameObject, covered);
    //        bind.Text.parentGameObject.transform.localScale = Vector2.one * (covered ? 1.1f : 1);
    //    }

    //    // Emergency Reset:
    //    if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.Keypad2) && Input.GetKeyDown(KeyCode.Space))
    //    {
    //        ResetKeys();
    //    }
    //}

    public void MoveOption(GameObject go, bool covered)
    {
        float x = covered ? 50 : 0;
        Vector2 newPos = go.transform.localPosition;
        newPos.x = Mathf.Lerp(newPos.x, x, Time.unscaledDeltaTime * 5);
        go.transform.localPosition = newPos;
    }
}

[System.Serializable]
public struct TextPair
{
    public TMP_Text black;
    public TMP_Text grey;

    public string text
    {
        get
        {
            return black.text;
        }
        set
        {
            black.text = value;
            grey.text = value;
        }
    }
    public Color color
    {
        set
        {
            black.color = new Color(value.r, value.g, value.b, black.color.a);
            grey.color = new Color(value.r, value.g, value.b, grey.color.a);
        }
    }

    public float fontSize
    {
        set
        {
            black.fontSize = value;
            grey.fontSize = value;
        }
    }

    public int maxChar
    {
        set
        {
            black.maxVisibleCharacters = value;
            grey.maxVisibleCharacters = value;
        }
    }

    public Transform transform => black.transform;
    public GameObject gameObject => black.transform.gameObject;
    public GameObject parentGameObject => black.transform.parent.gameObject;
    public Transform parent => black.transform.parent;
}
