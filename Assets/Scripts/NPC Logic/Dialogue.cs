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
    M_Dialogue _dialogueManager;
    M_Time _time;

    [System.Serializable]
    public struct Message
    {
        public string Name;
        [TextArea] public string Text;
        [Space(10)]
        public float Speed;        
        public float PunctuationMult;
        public string AudioClip;
        public float TimeToClose;
        public int CharPerTime;
        public bool Repeating;

        public float GetSpeed => Speed == 0 ? DEFAULT_SPEED : Speed;
        public float GetPuncMult => PunctuationMult == 0 ? DEFAULT_PUNC_MULT : PunctuationMult;
        public float GetTimeToClose => TimeToClose == 0 ? DEFAULT_TIMETOCLOSE : TimeToClose;
        public int GetCharPerTime => CharPerTime == 0 ? DEFAULT_CHARPERTIME : CharPerTime;
        public string GetAudioClip => string.IsNullOrWhiteSpace(AudioClip) ? DEFAULT_AUDIO : AudioClip;
    }

    public List<Message> Messages = new List<Message>();

    const float DEFAULT_SPEED = 0.05f;
    const float DEFAULT_PUNC_MULT = 10;
    const float DEFAULT_TIMETOCLOSE = 5;
    const int DEFAULT_CHARPERTIME = 1;
    const string DEFAULT_AUDIO = "";

    [HideInInspector] public bool ReadDefault;
    Message _speakMessage;

    string _lastMessage;

    Coroutine _currentTyping;

    private void Start()
    {
        _textBox = GetComponentInChildren<NPCTextbox>();
        _dialogueManager = Singleton.Get<M_Dialogue>();
        _time = Singleton.Get<M_Time>();

        TryFindMessage("default", out _speakMessage);
    }

    bool TryFindMessage(string name, out Message message)
    {
        foreach (Message m in Messages)
        {
            if (m.Name == name)
            {
                message = m;
                return true;
            }
        }

        message = new Message();
        return false;
    }

    public bool TryPlayDialogue(string name)
    {
        if (_time.InHalfTime && TryFindMessage(name + "_HT", out Message htMessage))
        {
            PlayMessage(htMessage);
            return true;
        }

        if (!TryFindMessage(name, out Message message))
            return false;

        PlayMessage(message);
        return true;
    }

    public void PlayMessage(Message message)
    {
        if (message.Name == _lastMessage && !message.Repeating)
            return;

        _lastMessage = message.Name;

        if (message.Name == "default")
            ReadDefault = true;

        _textBox.StartDialogue();

        if (_currentTyping != null)
            StopCoroutine(_currentTyping);

        _currentTyping = StartCoroutine(C_TypeSentence(message));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryPlayDialogue(_speakMessage.Name);
    }

    public void ChangeSpokenMessage(string name) => TryFindMessage(name, out _speakMessage);

    IEnumerator C_TypeSentence(Message message)
    {
        float elapsed = 0;
        int i = 0;

        string fixedText = _dialogueManager.ReplaceWithCorrectButtons(message.Text);

        while (_textBox.transform.localScale.x < 1)
            yield return null;

        while (i < fixedText.Length)
        {
            float dur = message.GetSpeed;
            if (i >= 1 && fixedText[i - 1].Is('.', '?', '!', ','))
                dur *= message.GetPuncMult;

            // First i characters of the string
            _textBox.SetText(fixedText.Substring(0, i));

            if (elapsed >= dur)
            {
                i += message.GetCharPerTime;
                // Play SFX
                elapsed = 0;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        _textBox.SetText(fixedText);

        yield return new WaitForSecondsRealtime(message.GetTimeToClose);

        if (TryPlayDialogue(message.Name + "+"))
            yield break;

        if (message.Repeating)
        {
            PlayMessage(message);
            yield break;
        }

        _textBox.EndDialogue();
    }
}
