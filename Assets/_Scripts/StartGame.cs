using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StartGame : MonoBehaviour
{
    [SerializeField] CanvasGroup warningCG;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject  skip;

    private IEnumerator Start()
    {
        yield return StartCoroutine(Warning());

        videoPlayer.Play();
        skip.SetActive(true);

        //Video가 끝날때까지 대기
        while (videoPlayer.frame < (long)videoPlayer.frameCount - 1)
        {
            if (Input.GetMouseButton(0))
                break;
            yield return null;
        }

        SceneManager.LoadScene("Lobby");
    }

    private IEnumerator Warning()
    {
        yield return StartCoroutine(Lerp.LerpValue(value => warningCG.alpha = value, 0, 1f, 1, Mathf.Lerp));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(Lerp.LerpValue(value => warningCG.alpha = value, 1, 0f, 1, Mathf.Lerp));
        DestroyImmediate(warningCG.gameObject);
    }
}