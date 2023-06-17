using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class M_Extensions
{
    #region MATH
    public static float Pow(this float a, float pow) => Mathf.Pow(a, pow);
    public static float RoundTo(this ref float x, float point) => x = Mathf.Round(x * Mathf.Pow(10, point)) / Mathf.Pow(10, point);
    public static float ChangeDigit(float n, float c)
    {
        float length = Mathf.Floor(Mathf.Log10(c) + 1);
        float asTen = Mathf.Pow(10, length);

        float before = Mathf.Round(n / asTen) * asTen;
        float after = n % asTen;
        return before + c + after;
    }
    public static float Greater(float a, float b) => a > b ? a : b;
    public static float AsRange(this Vector2 range) => Random.Range(range.x, range.y);
    public static float AsRange(this float num) => Random.Range(-num, num);
    public static float RandomizeSign(this float x) => x * (Random.value > 0.5f ? 1 : -1);
    public static bool RandBool() => Random.Range(0, 2) == 0;
    public static bool IsEven(this int x) => x % 2 == 0;
    public static bool IsEven(this float x) => x % 2 == 0;
    public static bool IsBetween(this float a, float min, float max) => (a <= max && a >= min) || (a >= max && a <= min);
    public static float Sign(this float a) => Mathf.Sign(a);
    #endregion

    #region VECTORS
    public static Vector2 BezierCube(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float r = 1f - t;
        float f0 = r * r * r;
        float f1 = r * r * t * 3;
        float f2 = r * t * t * 3;
        float f3 = t * t * t;

        return new Vector2(
        f0 * p0.x + f1 * p1.x + f2 * p2.x + f3 * p3.x,
        f0 * p0.y + f1 * p1.y + f2 * p2.y + f3 * p3.y
        );
    }
    public static Vector2 ReverseMidpoint(Vector2 point, Vector2 mid)
    {
        Vector2 ans;
        ans.x = 2 * mid.x - point.x;
        ans.y = 2 * mid.y - point.y;
        return ans;
    }
    public static Vector2 InverseLerp(Vector2 v, Vector2 a, Vector2 b)
    {
        float x = (v.x - a.x) / (b.x - a.x);
        float y = (v.y - a.y) / (b.y - a.y);
        return new Vector2(x, y);
    }
    public static Vector2 LerpAxis(Vector2 a, Vector2 b, Vector2 t)
    {
        float x = Mathf.Lerp(a.x, b.x, t.x);
        float y = Mathf.Lerp(a.y, b.y, t.y);
        return new Vector2(x, y);
    }
    #endregion

    #region LISTS
    public static GameObject FindGameObjectWithPos(this List<GameObject> gos, Vector3 pos)
    {
        foreach (GameObject go in gos)
        {
            if (go.transform.position == pos)
                return go;
        }
        return null;
    }
    public static bool Is<T>(this T a, params T[] array)
    {
        foreach (T t in array)
        {
            if (t.Equals(a))
                return true;
        }
        return false;
    }
    public static T RandomItem<T>(this List<T> list) => list[Random.Range(0, list.Count)];
    public static T RandomItem<T>(this T[] array) => array[Random.Range(0, array.Length)];
    #endregion

    #region Curves
    public static float CosCurve(float t) => 0.5f - 0.5f * Mathf.Cos(Mathf.PI * t);
    public static float DecayCurve(float x, float p) => Mathf.Pow(1 - Mathf.Pow(x, p), 1 / p);
    public static float SlowingCurve(float x) => 1.3f - Mathf.Pow(0.16f / Mathf.Pow(x + 0.3f, 2), 0.5f);
    public static float HumpCurve(float t, float peak, float start) => (4 * start - 4 * peak) * Mathf.Pow(t - 0.5f, 2) + peak;
    public static float HumpCurveV2(float t, float a) => -4 * a * Mathf.Pow(t - 0.5f, 2) + a;
    public static float OverShootCurve(float t)
    {
        return 1 - (float)(Mathf.Sin(t * Mathf.PI * 2) / (Mathf.Exp(1) * t * 2.3));
    }
    public static float FlatCurve(float x)
    {
        float a = 0.15f;
        float c = 21f;

        System.Func<float, float> f = k => 0.5f / (1 + Mathf.Exp(-c * (x - k)));
        return f(a) + f(1 - a);
    }
    #endregion

    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t)
        {
            Object.Destroy(child.gameObject);
        }
    }    
    public static float RemoveNaN(this float x) => float.IsNaN(x) ? 0 : x;
    public static string ToTime(this float x)
    {
        float min = Mathf.Floor(x / 60);
        float sec = Mathf.Floor(x % 60);

        string secStr = sec < 10 ? "0" + sec.ToString() : sec.ToString();

        return min.ToString() + ":" + secStr;
    }    
    public static void SetAlpha(this UnityEngine.UI.Image img, float value)
    {
        Color newColor = img.color;
        newColor.a = value;
        img.color = newColor;
    }
    public static void SetAlpha(this SpriteRenderer rend, float value)
    {
        Color newColor = rend.color;
        newColor.a = value;
        rend.color = newColor;
    }

    public static RaycastHit2D Ray(Vector2 origin, Vector2 dir, float dis = 1, LayerMask layer = default(LayerMask))
    {
        if (Application.isEditor)
            Debug.DrawRay(origin, dir * dis);

        return Physics2D.Raycast(origin, dir, dis, layer);
    }

    public static bool CheckTag(this GameObject obj, string tag) => M_Tags.CheckTag(obj, tag);

    public static IEnumerable<string> SplitToLines(this string input)
    {
        if (input == null)
        {
            yield break;
        }

        using (System.IO.StringReader reader = new System.IO.StringReader(input))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}
