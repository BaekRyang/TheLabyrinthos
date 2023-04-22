using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAnimate : MonoBehaviour
{
    RectTransform RT_rect;
    public bool b_isPlayer = false;
    void Start()
    {
        RT_rect = GetComponent<RectTransform>();
    }

    public IEnumerator Run(bool isPlayer, float duration)
    {
        int loc = 1536;
        if (isPlayer)
        {
            // isPlayer가 true인 경우 아무것도 하지 않습니다.
        }
        else
        {
            transform.localPosition = new Vector2(loc, 0); // localPosition 값을 초기화합니다.

            // duration 동안 localPosition 값을 0으로 Interpolate 합니다.
            float time = 0f;
            Vector2 startPos = transform.localPosition;
            Vector2 endPos = Vector2.zero;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                transform.localPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }
            transform.localPosition = endPos; // localPosition 값을 최종값으로 설정합니다.
        }
    }
}
