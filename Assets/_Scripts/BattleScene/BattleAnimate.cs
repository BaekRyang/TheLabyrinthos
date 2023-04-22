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
            // isPlayer�� true�� ��� �ƹ��͵� ���� �ʽ��ϴ�.
        }
        else
        {
            transform.localPosition = new Vector2(loc, 0); // localPosition ���� �ʱ�ȭ�մϴ�.

            // duration ���� localPosition ���� 0���� Interpolate �մϴ�.
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
            transform.localPosition = endPos; // localPosition ���� ���������� �����մϴ�.
        }
    }
}
