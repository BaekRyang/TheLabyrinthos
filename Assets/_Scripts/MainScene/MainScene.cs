using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class MainScene : MonoBehaviour
{
    [SerializeField] Image I_curtain;
    [SerializeField] AudioSource AS_mainLoop;
    [SerializeField] GameObject GO_mainUI;

    [SerializeField] GameObject[] GO_lights;
    [SerializeField] Light[] LIT_lights;

    bool b_loaded = false;

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        StartCoroutine(Open());

        LIT_lights = new Light[GO_lights.Length];

        for (int i = 0; i < GO_lights.Length; i++)
        {
            GO_lights[i] = GO_lights[i].transform.GetChild(0).gameObject;
            LIT_lights[i] = GO_lights[i].GetComponent<Light>();
        }
    }

    void Update()
    {

    }

    public void ButtonAction(string button)
    {
        if (button == "ClickToStart")
        {
            if (b_loaded)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                CanvasGroup tmpCG = GO_mainUI.GetComponent<CanvasGroup>();
                GO_mainUI.SetActive(true);
                StartCoroutine(LerpValue(alpha => tmpCG.alpha = alpha, 0, 1, 1));
                StartCoroutine(LerpValue((intense) => {
                    for (int i = 0; i < 12; i++)
                        LIT_lights[i].intensity = intense;
                    return;
                }, 0, 5, 3));
            }
        }
        else if (button == "StartGame")
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        StartCoroutine(LerpValue(volume => AS_mainLoop.volume = volume, 1, 0, 2));
        StartCoroutine(CurtainModify(false, 3));
    }
    
    private IEnumerator Open()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(LerpValue(volume => AS_mainLoop.volume = volume, 0, 1, 4));
        yield return new WaitForSeconds(1);
        StartCoroutine(CurtainModify(true, 3));
        yield return new WaitForSeconds(3);
        b_loaded = true;
    }


    public IEnumerator CurtainModify(bool open, float delay) //화면 암전 풀거나 걸기
    {
        float elapsedTime = 0f;

        Color startColor = I_curtain.color;
        Color endColor = I_curtain.color;
        if (open)
        {
            startColor.a = 1f;
            endColor.a = 0f;
        }
        else
        {
            startColor.a = 0f;
            endColor.a = 1f;
        }


        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / delay;
            I_curtain.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        I_curtain.color = endColor;

        if (!open)
        {
            SceneManager.LoadScene("Levels");   
        }
    }

    private IEnumerator LerpValue(Action<float> valueSetter, float from, float to, float duration)
                                 //람다식을 매개변수로 가져와서 사용한다.
    {
        float elapsedTime = 0.0f;
        
        while (elapsedTime < duration)
        {
            //LerpValue(volume => AS_mainLoop.volume = volume, 0, 1, 4)
            //          volume                                          => LerpValue에서 Mathf.Lerp를 해서 나온 값을 volume으로 전달한다.
            //                    AS_mainLoop.volume = volume =>        => param volume을 AS_mainLoop.volume 에 넣어준다.                     
            valueSetter(Mathf.Lerp(from, to, (elapsedTime / duration)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        valueSetter(to);
    }
}
