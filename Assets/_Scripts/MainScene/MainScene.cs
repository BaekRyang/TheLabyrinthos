using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] DataCarrier dataCarrier;

    [SerializeField] Image I_curtain;
    [SerializeField] AudioSource AS_mainLoop;
    [SerializeField] GameObject GO_mainUI;

    [SerializeField] GameObject[] GO_lights;
    [SerializeField] Light[] LIT_lights;
    [SerializeField] Volume VOL_volume;

    [SerializeField] GameObject GO_targetPosition;
    [SerializeField] GameObject GO_defaultPosition;

    [SerializeField] GameObject GO_whiteBoard;
    Material[] MAT_logos = new Material[2];

    bool b_loaded = false;

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        StartCoroutine(Open());
        dataCarrier = GameObject.Find("DataPacker").GetComponent<DataCarrier>();
        DontDestroyOnLoad(dataCarrier);
        LIT_lights = new Light[GO_lights.Length];

        for (int i = 0; i < GO_lights.Length; i++)
        {
            GO_lights[i] = GO_lights[i].transform.GetChild(0).gameObject;
            LIT_lights[i] = GO_lights[i].GetComponent<Light>();
        }

        MAT_logos[0] = GO_whiteBoard.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material;
        MAT_logos[1] = GO_whiteBoard.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material;
    }

    public void CheckSeed(GameObject sender)
    {
        if (sender.GetComponent<TMP_InputField>().text.Length != 6)
        {
            sender.GetComponent<TMP_InputField>().text = "";
            sender.transform.GetChild(0).Find("Placeholder").GetComponent<TMP_Text>().text = "시드는 <b>6자리 HEX</b>이여야 합니다.";
        }
        else
        {
            GameObject.Find("DataPacker").GetComponent<DataCarrier>().seed = sender.GetComponent<TMP_InputField>().text;
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
                StartCoroutine(LerpValue<float>(alpha => tmpCG.alpha = alpha, 0, 1, 1, Mathf.Lerp));
                StartCoroutine(LerpValue<float>((intense) => {
                    for (int i = 0; i < 12; i++)
                        LIT_lights[i].intensity = intense;
                    return;
                }, 0, 5, 3, Mathf.Lerp));
            }
        }
        else if (button == "StartGame")
        {
            StartCoroutine(LerpValue(value => Camera.main.transform.position = value, GO_defaultPosition.transform.position, GO_targetPosition.transform.position, 1, Vector3.Lerp, EaseOutSine));
            StartCoroutine(LerpValue(value => Camera.main.transform.rotation = value, GO_defaultPosition.transform.rotation, GO_targetPosition.transform.rotation, 1, Quaternion.Lerp, EaseOutSine));
            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(LerpValue(value => dof.focusDistance.value = value, 5, 0.2f, 1, Mathf.Lerp));
            StartCoroutine(LerpValue(value => dof.aperture.value = value, 15, 32f, 1, Mathf.Lerp));

            StartCoroutine(LerpValue(value => MAT_logos[0].color = value, Color.white, new Color(1, 1, 1, 0), 0.5f, Color.Lerp));
            StartCoroutine(LerpValue(value => MAT_logos[1].color = value, Color.white, new Color(1, 1, 1, 0), 0.5f, Color.Lerp));

            GO_mainUI.transform.Find("NewGameSetting").gameObject.SetActive(true);
            var tmpCG = GO_mainUI.transform.Find("NewGameSetting").GetComponent<CanvasGroup>();
            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(LerpValue(value => tmpCG.alpha = value, 0, 1f, 1f, Mathf.Lerp, EaseOutSine));
            StartCoroutine(LerpValue(value => tmpCG2.alpha = value, 1, 0f, 1f, Mathf.Lerp, EaseOutSine));
            GO_mainUI.transform.Find("MainTabs").gameObject.SetActive(false);
        } else if (button == "LoadLevel")
            StartCoroutine(CurtainModify(false, 3));
        else if (button == "BacktoMenu")
        {
            StartCoroutine(LerpValue(value => Camera.main.transform.position = value, GO_targetPosition.transform.position, GO_defaultPosition.transform.position, 1, Vector3.Lerp, EaseOutSine));
            StartCoroutine(LerpValue(value => Camera.main.transform.rotation = value, GO_targetPosition.transform.rotation, GO_defaultPosition.transform.rotation, 1, Quaternion.Lerp, EaseOutSine));
            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(LerpValue(value => dof.focusDistance.value = value, 0.2f, 5, 1, Mathf.Lerp));
            StartCoroutine(LerpValue(value => dof.aperture.value = value, 32, 15f, 1, Mathf.Lerp));

            StartCoroutine(LerpValue(value => MAT_logos[0].color = value, new Color(1, 1, 1, 0), Color.white, 0.5f, Color.Lerp));
            StartCoroutine(LerpValue(value => MAT_logos[1].color = value, new Color(1, 1, 1, 0), Color.white, 0.5f, Color.Lerp));

            GO_mainUI.transform.Find("MainTabs").gameObject.SetActive(true);
            var tmpCG = GO_mainUI.transform.Find("NewGameSetting").GetComponent<CanvasGroup>();
            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(LerpValue(value => tmpCG.alpha = value, 1, 0f, 0.5f, Mathf.Lerp));
            StartCoroutine(LerpValue(value => tmpCG2.alpha = value, 0, 1f, 0.5f, Mathf.Lerp));
            GO_mainUI.transform.Find("NewGameSetting").gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        StartCoroutine(LerpValue<float>(volume => AS_mainLoop.volume = volume, 1, 0, 2, Mathf.Lerp));
        StartCoroutine(CurtainModify(false, 3));
    }

    
    private IEnumerator Open()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(LerpValue<float>(volume => AS_mainLoop.volume = volume, 0, 1, 4, Mathf.Lerp));
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

    private IEnumerator LerpValue<T>(
        Action<T> valueSetter,                      //람다 함수
        T from,                                     //from
        T to,                                       //to
        float duration,                             //보간 시간
        Func<T, T, float, T> lerpFunction,   //보간 함수
        Func<float, float> easingFunction = null)   //이징 함수
    {

        if (easingFunction == null) //없으면 선형보간
            easingFunction = Linear;

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float easedT = easingFunction(t);
            valueSetter(lerpFunction(from, to, easedT));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        valueSetter(to);
    }

    private float Linear(float t) //선형보간
    {
        return t;
    }

    private float EaseOutSine(float t)
    {
        return Mathf.Sin(Mathf.Pow(t, 0.5f) * Mathf.PI / 2);
    }   

    private float EaseInSine(float t)
    {
        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }
}
