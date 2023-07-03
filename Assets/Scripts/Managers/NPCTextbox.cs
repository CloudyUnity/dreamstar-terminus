using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#pragma warning disable CS0162 // Unreachable code detected
public class NPCTextbox : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    [SerializeField] GameObject _crnr1, _crnr2, _crnr3, _crnr4, _top, _left, _bottom, _right, _bg;
    [SerializeField] float _growSpeed;

    const bool DEBUG_MODE = false;

    private void Start()
    {
        transform.localScale = Vector3.zero;

        if (DEBUG_MODE)
            StartDialogue();
    }

    private void Update()
    {
        if (DEBUG_MODE)
            SetText(_text.text);
    }
#pragma warning restore CS0162 // Unreachable code detected

    public void SetText(string str)
    {
        if (transform.localScale.x < 1)
            return;

        _text.text = str;

        int size = str.Length;

        //Debug.Log(_text.renderedHeight + " " + _text.renderedHeight / 0.15f + " " + Mathf.RoundToInt(_text.renderedHeight / 0.15f));

        int newLineBonus = Mathf.Clamp(Mathf.RoundToInt(_text.renderedHeight / 0.15f), 0, 99) - 1;

        int clamped = newLineBonus > 0 ? 20 : size;

        float growAmount = clamped * 0.071f;

        float height = 0.169f * newLineBonus;

        _crnr1.transform.LerpTo(new Vector3(-0.1325f - growAmount, _crnr1.transform.localPosition.y), _growSpeed, local: true);
        _crnr2.transform.LerpTo(new Vector3(0.1665f + growAmount, _crnr2.transform.localPosition.y), _growSpeed, local: true);
        _crnr3.transform.LerpTo(new Vector3(0.1665f + growAmount, 0.687f + height), _growSpeed, local: true);
        _crnr4.transform.LerpTo(new Vector3(-0.1325f - growAmount, 0.687f + height), _growSpeed, local: true);

        float width = clamped * 0.13f;
        _top.transform.LerpTo(new Vector3(Mathf.Lerp(0.0134f, 0, clamped / 20f), 0.65587f + height), _growSpeed, local: true);
        _top.transform.LerpToScale(new Vector3(_top.transform.localScale.x, 0.267f + width), _growSpeed);

        _bottom.transform.LerpToScale(new Vector3(_bottom.transform.localScale.x, 0.267f + width), _growSpeed);

        float mid = Mathf.Clamp(Mathf.Lerp(_crnr1.transform.localPosition.y, _crnr4.transform.localPosition.y, 0.5f), 0.1f, 99f);

        _left.transform.LerpTo(new Vector3(-0.101373f - growAmount, mid), _growSpeed, local: true);
        _left.transform.LerpToScale(new Vector3(_left.transform.localScale.x, 0.1f + 0.16f * newLineBonus), _growSpeed);

        _right.transform.LerpTo(new Vector3(0.13207f + growAmount, mid), _growSpeed, local: true);
        _right.transform.LerpToScale(new Vector3(_right.transform.localScale.x, 0.1f + 0.16f * newLineBonus), _growSpeed);

        Vector3 averagePoint = M_Extensions.AveragePoint(_crnr1, _crnr2, _crnr3, _crnr4);
        _bg.transform.LerpTo(averagePoint, _growSpeed);

        float disX = Mathf.Clamp(Vector2.Distance(_crnr1.transform.position, _crnr2.transform.position), 0, 99);
        float disY = Mathf.Clamp(Vector2.Distance(_crnr1.transform.position, _crnr4.transform.position), 0, 99);
        _bg.transform.LerpToScale(new Vector2(disX, disY), _growSpeed);

        _text.alignment = newLineBonus > 0 ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.Bottom;
    }

    public void EndDialogue()
    {
        StartCoroutine(C_ChangeSize(Vector2.zero));
    }

    public void StartDialogue()
    {
        StartCoroutine(C_ChangeSize(Vector2.one));
    }

    IEnumerator C_ChangeSize(Vector2 target)
    {
        Vector2 start = transform.localScale;

        float elapsed = 0;
        float dur = 0.3f;

        while (elapsed < dur && transform.localScale.x < 1)
        {
            float curved = M_Extensions.CosCurve(elapsed / dur);
            transform.localScale = Vector2.Lerp(start, target, curved);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = target;
    }
}
