using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
Dialogue system needs:
Walk up to object and will start speaking
Should speak after a trigger
Branching dialogue
"Default" for first?
*/
public class Dialogue : MonoBehaviour
{
    NPCTextbox _textBox;

    [System.Serializable]
    public struct Message
    {
        public string Name;
        [TextArea] public string Text;
        [Space(10)]
        public float Speed;        
        public float PunctuationMult;
        public string AudioClip;

        public float GetSpeed => Speed == 0 ? DEFAULT_SPEED : Speed;
        public float GetPuncMult => PunctuationMult == 0 ? DEFAULT_PUNC_MULT : PunctuationMult;
        public string GetAudioClip => string.IsNullOrWhiteSpace(AudioClip) ? DEFAULT_AUDIO : AudioClip;
    }

    public List<Message> Messages = new List<Message>();

    const float DEFAULT_SPEED = 0.05f;
    const float DEFAULT_PUNC_MULT = 10;
    const string DEFAULT_AUDIO = "";

    [HideInInspector] public bool ReadDefault;

    Coroutine _currentTyping;

    private void Start()
    {
        _textBox = GetComponentInChildren<NPCTextbox>();
    }

    Message FindMessage(string name)
    {
        foreach (Message m in Messages)
        {
            if (m.Name == name)
                return m;
        }

        throw new System.Exception("Message not found: " + name);
    }

    public void PlayDialogue(string name)
    {
        if (name == "default")
        {
            if (ReadDefault)
                return;

            ReadDefault = true;
        }

        Message message = FindMessage(name);

        _textBox.StartDialogue();

        if (_currentTyping != null)
            StopCoroutine(_currentTyping);

        _currentTyping = StartCoroutine(C_TypeSentence(message));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayDialogue("default");
    }

    IEnumerator C_TypeSentence(Message message)
    {
        float elapsed = 0;
        int i = 0;

        while (_textBox.transform.localScale.x < 1)
            yield return null;

        while (i < message.Text.Length)
        {
            float dur = message.GetSpeed;
            if (i >= 1 && message.Text[i - 1].Is('.', '?', '!', ','))
                dur *= message.PunctuationMult;

            // First i characters of the string
            _textBox.SetText(message.Text.Substring(0, i));

            if (elapsed >= dur)
            {                
                i++;
                // Play SFX
                elapsed = 0;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        _textBox.SetText(message.Text);
        
        // If player steps away, close dialogue box
    }
}
