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

    public const float DURATION = 1.5f;
    public const float BOX_SCALE = 0.15f;
    public const float SCROLL_SPEED = 0.75f;

    protected override void Awake()
    {
        base.Awake();
       
        _transition = transform.GetChild(0).GetComponent<SpriteRenderer>();       
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
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

        if (_boxes.Count == 0)
        {
            for (float i = -7; i < 7; i += BOX_SCALE)
                for (float j = -4; j < 4; j += BOX_SCALE)
                {
                    SpriteRenderer rend = Instantiate(_transition, transform);
                    rend.transform.localScale = Vector3.one * BOX_SCALE;
                    rend.transform.localPosition = new Vector3(i, j);
                    _boxes.Add(rend);
                }
        }

        float pureSeed = Random.Range(0, 9999f);

        while (elapsed < DURATION)
        {
            for (int i = 0; i < _boxes.Count; i++)
            {
                if (_boxes[i] == null)
                    return;

                float perlinX = _boxes[i].transform.position.x + pureSeed + (elapsed * SCROLL_SPEED);
                float perlinY = _boxes[i].transform.position.y + pureSeed + (elapsed * SCROLL_SPEED);
                float seed = (Mathf.PerlinNoise(perlinX, perlinY) - 0.5f) * 2;

                float curved = M_Extensions.ParametricVaryCurve(elapsed / DURATION, seed);

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
