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

    [SerializeField] GameObject[] GO_cameras;

    bool b_loaded = false;
     
    void Start()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
        //Screen.SetResolution(2560, 1440, true);
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
        if (button == "ClickToStart" && b_loaded)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            CanvasGroup tmpCG = GO_mainUI.GetComponent<CanvasGroup>();
            GO_mainUI.SetActive(true);
            StartCoroutine(LerpValue(alpha => tmpCG.alpha = alpha, 0, 1f, 1, Mathf.Lerp));
            StartCoroutine(LerpValue((intense) =>
            {
                for (int i = 0; i < 12; i++)
                    LIT_lights[i].intensity = intense;
                return;
            }, 0.1f, 5, 3, Mathf.Lerp));
        }
        else if (button == "StartGame")
            StartCoroutine(MoveMenu(0));
        else if (button == "BacktoMenu_NG")
            StartCoroutine(MoveMenu(1));
        else if (button == "Settings")
            StartCoroutine(MoveMenu(2));
        else if (button == "BacktoMenu_ST")
            StartCoroutine(MoveMenu(3));
        else if (button == "LoadLevel")
            StartCoroutine(CurtainModify(false, 2, true));
        else if (button == "Exit")
            StartCoroutine(CurtainModify(false, 1, false, true));
            
    }

    public void StartGame()
    {
        StartCoroutine(LerpValue(volume => AS_mainLoop.volume = volume, 1, 0f, 2, Mathf.Lerp));
        StartCoroutine(CurtainModify(false, 3));
    }

    private IEnumerator MoveMenu(int type)
    {
        if (type == 0)
        {
            GO_cameras[0].SetActive(false);
            GO_cameras[1].SetActive(true);

            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(LerpValue(value => dof.focusDistance.value = value, 6, 2.5f, 1, Mathf.Lerp));

            StartCoroutine(LerpValue(value => MAT_logos[0].color = value, Color.white, new Color(1, 1, 1, 0), 0.5f, Color.Lerp));
            yield return StartCoroutine(LerpValue(value => MAT_logos[1].color = value, Color.white, new Color(1, 1, 1, 0), 0.5f, Color.Lerp));

            GO_mainUI.transform.Find("NewGameSetting").gameObject.SetActive(true);
            var tmpCG = GO_mainUI.transform.Find("NewGameSetting").GetComponent<CanvasGroup>();
            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(LerpValue(value => tmpCG.alpha = value, 0, 1f, 1f, Mathf.Lerp, EaseOutSine));
            StartCoroutine(LerpValue(value => tmpCG2.alpha = value, 1, 0f, 1f, Mathf.Lerp, EaseOutSine));
            yield return null;
        } else if (type == 1)
        {
            GO_cameras[0].SetActive(true);
            GO_cameras[1].SetActive(false);
            var tmpCG = GO_mainUI.transform.Find("NewGameSetting").GetComponent<CanvasGroup>();
            yield return StartCoroutine(LerpValue(value => tmpCG.alpha = value, 1, 0f, 0.3f, Mathf.Lerp));
            tmpCG.gameObject.SetActive(false);

            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(LerpValue(value => tmpCG2.alpha = value, 0, 1f, 0.5f, Mathf.Lerp));
            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(LerpValue(value => dof.focusDistance.value = value, 2.5f, 6, 1, Mathf.Lerp));

            StartCoroutine(LerpValue(value => MAT_logos[0].color = value, new Color(1, 1, 1, 0), Color.white, 0.5f, Color.Lerp));
            StartCoroutine(LerpValue(value => MAT_logos[1].color = value, new Color(1, 1, 1, 0), Color.white, 0.5f, Color.Lerp));
            yield return null;
        } else if (type == 2)
        {
            GO_cameras[0].SetActive(false);
            GO_cameras[2].SetActive(true);

            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(LerpValue(value => dof.focusDistance.value = value, 6, 2.5f, 1, Mathf.Lerp));
            StartCoroutine(LerpValue(value => dof.focalLength.value = value, 300, 100f, 1, Mathf.Lerp));

            GO_mainUI.transform.Find("Settings").gameObject.SetActive(true);
            var tmpCG = GO_mainUI.transform.Find("Settings").GetComponent<CanvasGroup>();
            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(LerpValue(value => tmpCG.alpha = value, 0, 1f, 1f, Mathf.Lerp, EaseOutSine));
            StartCoroutine(LerpValue(value => tmpCG2.alpha = value, 1, 0f, 1f, Mathf.Lerp, EaseOutSine));
            yield return null;
        }
        else if (type == 3)
        {
            GO_cameras[0].SetActive(true);
            GO_cameras[2].SetActive(false);
            var tmpCG = GO_mainUI.transform.Find("Settings").GetComponent<CanvasGroup>();
            yield return StartCoroutine(LerpValue(value => tmpCG.alpha = value, 1, 0f, 0.3f, Mathf.Lerp));
            tmpCG.gameObject.SetActive(false);

            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(LerpValue(value => tmpCG2.alpha = value, 0, 1f, 0.5f, Mathf.Lerp));
            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(LerpValue(value => dof.focusDistance.value = value, 2.5f, 6, 1, Mathf.Lerp));
            StartCoroutine(LerpValue(value => dof.focalLength.value = value, 100, 300f, 1, Mathf.Lerp));
            yield return null;
        }
    }
    private IEnumerator Open()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(LerpValue<float>(volume => AS_mainLoop.volume = volume, 0, 1, 4, Mathf.Lerp));
        yield return new WaitForSeconds(1);
        StartCoroutine(CurtainModify(true, 2));
        yield return new WaitForSeconds(3);
        b_loaded = true;
    }


    public IEnumerator CurtainModify(bool open, float delay, bool loadScene = false, bool quit = false) //화면 암전 풀거나 걸기
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

        if (loadScene)
        {
            yield return StartCoroutine(LerpValue<float>(volume => AS_mainLoop.volume = volume, 1, 0, 1, Mathf.Lerp));
            SceneManager.LoadScene("Levels");
        }
            
        if (quit)
        {
            yield return StartCoroutine(LerpValue<float>(volume => AS_mainLoop.volume = volume, 1, 0, 1, Mathf.Lerp));
            Application.Quit();
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
