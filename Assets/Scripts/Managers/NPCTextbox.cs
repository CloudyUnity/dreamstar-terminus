using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCTextbox : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    [SerializeField] GameObject _crnr1, _crnr2, _crnr3, _crnr4, _top, _left, _bottom, _right, _bg;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        //SetText(_text.text);
    }

    public void SetText(string str)
    {
        _text.text = str;

        int size = str.Length;

        if (transform.localScale.x == 0)
        {
            StartCoroutine(C_ChangeSize(Vector2.one));
            return;
        }

        int newLineBonus = Mathf.FloorToInt(_text.renderedHeight / 0.15f);

        int clamped = newLineBonus > 0 ? 20 : size;

        float growAmount = clamped * 0.071f;

        float height = 0.169f * newLineBonus;

        _crnr1.transform.localPosition = new Vector3(-0.1325f - growAmount, _crnr1.transform.localPosition.y);
        _crnr2.transform.localPosition = new Vector3(0.1665f + growAmount, _crnr2.transform.localPosition.y);
        _crnr3.transform.localPosition = new Vector3(0.1665f + growAmount, 0.687f + height);
        _crnr4.transform.localPosition = new Vector3(-0.1325f - growAmount, 0.687f + height);

        float width = clamped * 0.13f;
        _top.transform.localPosition = new Vector3(Mathf.Lerp(0.0134f, 0, clamped / 20f), 0.65587f + height);
        _top.transform.localScale = new Vector3(_top.transform.localScale.x, 0.267f + width);

        _bottom.transform.localScale = new Vector3(_bottom.transform.localScale.x, 0.267f + width);

        float mid = Mathf.Lerp(_crnr1.transform.localPosition.y, _crnr4.transform.localPosition.y, 0.5f);
        _left.transform.localPosition = new Vector3(-0.101373f - growAmount, mid);
        _left.transform.localScale = new Vector3(_left.transform.localScale.x, 0.1f + 0.16f * newLineBonus);

        _right.transform.localPosition = new Vector3(0.13207f + growAmount, mid);
        _right.transform.localScale = new Vector3(_right.transform.localScale.x, 0.1f + 0.16f * newLineBonus);

        _bg.transform.position = M_Extensions.AveragePoint(_crnr1, _crnr2, _crnr3, _crnr4);
        float disX = Vector2.Distance(_crnr1.transform.position, _crnr2.transform.position);
        float disY = Vector2.Distance(_crnr1.transform.position, _crnr4.transform.position);
        _bg.transform.localScale = new Vector2(disX, disY);

        _text.alignment = newLineBonus > 0 ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.Bottom;
    }

    public void EndDialogue()
    {
        StartCoroutine(C_ChangeSize(Vector2.zero));
    }

    IEnumerator C_ChangeSize(Vector2 target)
    {
        Vector2 start = transform.localScale;

        float elapsed = 0;
        float dur = 0.3f;

        while (elapsed < dur)
        {
            float curved = M_Extensions.CosCurve(elapsed / dur);
            transform.localScale = Vector2.Lerp(start, target, curved);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = target;
    }
}
