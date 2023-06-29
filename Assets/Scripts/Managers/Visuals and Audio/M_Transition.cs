using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class M_Transition : Singleton
{
    [HideInInspector] public bool Transitioning;

    SpriteRenderer _transition;
    M_Camera _cam;

    List<SpriteRenderer> _boxes = new List<SpriteRenderer>();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        _transition = transform.GetChild(0).GetComponent<SpriteRenderer>();       
    }

    private void Update()
    {
        if (_cam == null)
        {
            _cam = Get<M_Camera>();
            return;
        }

        transform.position = (Vector2)_cam.transform.position;
    }

    public async Task TransitionAsync(bool inwards)
    {
        if (Transitioning)
        {
            Debug.Log("Transition not ready, do not call");
            return;
        }

        Transitioning = true;

        float elapsed = 0;
        float dur = 1.5f;
        float boxScale = 0.15f;
        float scrollSpeed = 0.75f;        

        if (_boxes.Count == 0)
        {
            for (float i = -7; i < 7; i += boxScale)
                for (float j = -4; j < 4; j += boxScale)
                {
                    SpriteRenderer rend = Instantiate(_transition, transform);
                    rend.transform.localScale = Vector3.one * boxScale;
                    rend.transform.localPosition = new Vector3(i, j);
                    _boxes.Add(rend);
                }
        }

        float pureSeed = Random.Range(0, 9999f);

        while (elapsed < dur)
        {
            for (int i = 0; i < _boxes.Count; i++)
            {
                if (_boxes[i] == null)
                    return;

                float seed = (Mathf.PerlinNoise(_boxes[i].transform.position.x + pureSeed + (elapsed * scrollSpeed), _boxes[i].transform.position.y + pureSeed + (elapsed * scrollSpeed)) - 0.5f) * 2;
                float curved = M_Extensions.ParametricVaryCurve(elapsed / dur, seed);

                if (!inwards)
                    curved = 1 - curved;

                _boxes[i].SetAlpha(curved);
            }

            elapsed += Time.deltaTime;
            await Task.Yield();
        }

        Transitioning = false;
    }
}
