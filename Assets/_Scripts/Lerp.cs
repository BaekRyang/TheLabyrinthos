using System;
using System.Collections;
using UnityEngine;

public static class Lerp
{
    public static IEnumerator LerpValue<T>(
        Action<T> valueSetter,                      //람다 함수
        T from,                                     //from
        T to,                                       //to
        float duration,                             //보간 시간
        Func<T, T, float, T> lerpFunction,          //보간 함수
        Func<float, float> easingFunction = null)   //이징 함수
    {
        if (easingFunction == null) //없으면 선형보간
            easingFunction = Linear;

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float easedT = easingFunction(t);            //이징 함수를 사용하여 t의 변화값을 사용한다.
            valueSetter(lerpFunction(from, to, easedT)); //LerpFunction을 한 결과를 valueSetter의 param으로 준다.
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        valueSetter(to);
    }

    public static float Linear(float t) //선형보간
    {
        return t;
    }

    public static float EaseOutSine(float t)
    {
        return Mathf.Sin(Mathf.Pow(t, 0.5f) * Mathf.PI / 2);
    }

    public static float EaseInSine(float t)
    {
        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }
}
