using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] Image I_curtain;
    [SerializeField] AudioSource AS_mainLoop;

    bool b_loaded = false;
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        StartCoroutine(Open());
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        if (b_loaded)
        {
            StartCoroutine(Lerp(AS_mainLoop, 1, 0, 4));
            StartCoroutine(CurtainModify(false, 3));
        }
    }
    
    private IEnumerator Open()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(Lerp(AS_mainLoop, 0, 1, 4));
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

    private IEnumerator Lerp(AudioSource target, float from, float to, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            target.volume = Mathf.Lerp(from, to, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.volume = to;
    }
}
