using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour
{
    SpriteRenderer sr;
    bool isOdd;
    bool clickable = true;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Initialize(bool odd, float fadeTime)
    {
        isOdd = odd;

        if (!isOdd)
        {
            StartCoroutine(FadeOut(fadeTime));
        }
    }

    IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        Color c = sr.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            sr.color = c;
            yield return null;
        }
    }

    void OnMouseDown()
    {
        if (!clickable) return;

        clickable = false;
        GameManager.Instance.OnTileClicked(isOdd);
    }
}
