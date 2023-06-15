using System.Collections;
using TMPro;
using TypeDefs;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Resolution = TypeDefs.Resolution;
using Volume = UnityEngine.Rendering.Volume;

public class MainScene : MonoBehaviour
{
    [SerializeField] DataCarrier dataCarrier;

    [SerializeField] Image       I_curtain;
    [SerializeField] Image       I_curtainOverVideo;
    [SerializeField] AudioSource AS_mainLoop;
    [SerializeField] GameObject  GO_mainUI;
    [SerializeField] GameObject  loadGame;

    [SerializeField] GameObject[] GO_lights;
    [SerializeField] Light[]      LIT_lights;
    [SerializeField] Volume       VOL_volume;

    [SerializeField] GameObject GO_targetPosition;
    [SerializeField] GameObject GO_defaultPosition;

    [SerializeField] GameObject GO_whiteBoard;
    private          Material   MAT_logos;

    [SerializeField] GameObject[] GO_cameras;

    private bool loaded = false;

    void Start()
    {
        SystemObject.Instance.LoadSetting();
        if (SystemObject.Instance.LoadData())
        {
            loadGame.GetComponent<Button>().interactable = true;
        }

        StartCoroutine(Open());
        dataCarrier = GameObject.Find("DataPacker").GetComponent<DataCarrier>();
        DontDestroyOnLoad(dataCarrier);
        LIT_lights = new Light[GO_lights.Length];

        for (int i = 0; i < GO_lights.Length; i++)
            LIT_lights[i] = GO_lights[i].GetComponent<Light>();

        MAT_logos = GO_whiteBoard.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material;
    }

    public void CheckSeed(GameObject sender)
    {
        if (sender.GetComponent<TMP_InputField>().text.Length != 6)
        {
            sender.GetComponent<TMP_InputField>().text                                     = "";
            sender.transform.GetChild(0).Find("Placeholder").GetComponent<TMP_Text>().text = "시드는 <color=white>6자리 HEX</color>이여야 합니다.";
        }
        else
        {
            GameObject.Find("DataPacker").GetComponent<DataCarrier>().seed = sender.GetComponent<TMP_InputField>().text;
        }
    }

    public void ButtonAction(string button)
    {
        switch (button)
        {
            case "ClickToStart" when loaded:
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(2).gameObject.SetActive(false);
                    CanvasGroup tmpCG = GO_mainUI.GetComponent<CanvasGroup>();
                    GO_mainUI.SetActive(true);
                    StartCoroutine(Lerp.LerpValue(alpha => tmpCG.alpha = alpha, 0, 1f, 1, Mathf.Lerp));
                    StartCoroutine(Lerp.LerpValue(intense =>
                    {
                        for (int i = 0; i < 12; i++)
                            LIT_lights[i].intensity = intense;
                    }, 0.1f, 5, 3, Mathf.Lerp));
                    break;
                }
            case "StartGame":
                StartCoroutine(MoveMenu(0));
                break;
            case "BacktoMenu_NG":
                StartCoroutine(MoveMenu(1));
                break;
            case "Settings":
                StartCoroutine(MoveMenu(2));
                break;
            case "BacktoMenu_ST":
                StartCoroutine(MoveMenu(3));
                break;
            case "LoadGame":
                SystemObject.Instance.useSave = true;
                StartGame();
                break;
            case "NewGame":
                StartGame();
                break;
            case "Exit":
                StartCoroutine(CurtainModify(false, 1, false, true));
                break;
        }
    }

    public void StartGame(bool useSave = false)
    {
        StartCoroutine(Lerp.LerpValue(volume => AS_mainLoop.volume = volume, 1, 0f, 2, Mathf.Lerp));
        StartCoroutine(CurtainModify(false, 2f, true));
    }

    private IEnumerator MoveMenu(int type)
    {
        if (type == 0)
        {
            GO_cameras[0].SetActive(false);
            GO_cameras[1].SetActive(true);

            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(Lerp.LerpValue(value => dof.focusDistance.value = value, 6, 2.5f, 1, Mathf.Lerp));

            yield return StartCoroutine(Lerp.LerpValue(value => MAT_logos.color = value, Color.white, new Color(1, 1, 1, 0), 0.5f, Color.Lerp));

            GO_mainUI.transform.Find("NewGameSetting").gameObject.SetActive(true);
            var tmpCG  = GO_mainUI.transform.Find("NewGameSetting").GetComponent<CanvasGroup>();
            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(Lerp.LerpValue(value => tmpCG.alpha  = value, 0, 1f, 1f, Mathf.Lerp, Lerp.EaseOut));
            StartCoroutine(Lerp.LerpValue(value => tmpCG2.alpha = value, 1, 0f, 1f, Mathf.Lerp, Lerp.EaseOut));
            yield return null;
        }
        else if (type == 1)
        {
            GO_cameras[0].SetActive(true);
            GO_cameras[1].SetActive(false);
            var tmpCG = GO_mainUI.transform.Find("NewGameSetting").GetComponent<CanvasGroup>();
            yield return StartCoroutine(Lerp.LerpValue(value => tmpCG.alpha = value, 1, 0f, 0.3f, Mathf.Lerp));
            tmpCG.gameObject.SetActive(false);

            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(Lerp.LerpValue(value => tmpCG2.alpha = value, 0, 1f, 0.5f, Mathf.Lerp));
            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(Lerp.LerpValue(value => dof.focusDistance.value = value, 1.5f, 6, 1, Mathf.Lerp));

            StartCoroutine(Lerp.LerpValue(value => MAT_logos.color = value, new Color(1, 1, 1, 0), Color.white, 0.5f, Color.Lerp));
            yield return null;
        }
        else if (type == 2)
        {
            GO_cameras[0].SetActive(false);
            GO_cameras[2].SetActive(true);

            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(Lerp.LerpValue(value => dof.focusDistance.value = value, 6,   1.5f, 1, Mathf.Lerp));
            StartCoroutine(Lerp.LerpValue(value => dof.focalLength.value   = value, 300, 100f, 1, Mathf.Lerp));

            GO_mainUI.transform.Find("Settings").gameObject.SetActive(true);

            var tmpCG  = GO_mainUI.transform.Find("Settings").GetComponent<CanvasGroup>();
            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(Lerp.LerpValue(value => tmpCG.alpha  = value, 0, 1f, 1f, Mathf.Lerp, Lerp.EaseOut));
            StartCoroutine(Lerp.LerpValue(value => tmpCG2.alpha = value, 1, 0f, 1f, Mathf.Lerp, Lerp.EaseOut));

            {
                var anchor = GO_mainUI.transform.Find("Settings").Find("SoundSettingElements");

                Dictionary<string, Slider> soundSliders = new Dictionary<string, Slider>();
                //각 자식의 이름으로 슬라이더를 등록해준다.
                foreach (Transform child in anchor)
                    soundSliders.Add(child.name, child.GetComponentInChildren<Slider>());

                //Settings.Instance.optionData.volume의 값을 슬라이더에 적용시킨다.
                soundSliders["Master"].value = SystemObject.Instance.optionData.volume.master;
                soundSliders["Music"].value  = SystemObject.Instance.optionData.volume.music;
                soundSliders["SFX"].value    = SystemObject.Instance.optionData.volume.sfx;
            }

            yield return null;
        }
        else if (type == 3)
        {
            GO_cameras[0].SetActive(true);
            GO_cameras[2].SetActive(false);
            var tmpCG = GO_mainUI.transform.Find("Settings").GetComponent<CanvasGroup>();
            yield return StartCoroutine(Lerp.LerpValue(value => tmpCG.alpha = value, 1, 0f, 0.3f, Mathf.Lerp));
            tmpCG.gameObject.SetActive(false);

            var tmpCG2 = GO_mainUI.transform.Find("MainTabs").GetComponent<CanvasGroup>();
            StartCoroutine(Lerp.LerpValue(value => tmpCG2.alpha = value, 0, 1f, 0.5f, Mathf.Lerp));
            DepthOfField dof;
            VOL_volume.profile.TryGet(out dof);

            StartCoroutine(Lerp.LerpValue(value => dof.focusDistance.value = value, 1.5f, 6,    1, Mathf.Lerp));
            StartCoroutine(Lerp.LerpValue(value => dof.focalLength.value   = value, 100,  300f, 1, Mathf.Lerp));

            yield return null;
        }
    }

    private IEnumerator Open()
    {
        yield return new WaitForSeconds(1f);

        StartCoroutine(Lerp.LerpValue<float>(volume => AS_mainLoop.volume = volume, 0, 1, 4, Mathf.Lerp));
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(CurtainModify(true, 2));
        loaded = true;
    }


    public IEnumerator CurtainModify(bool open, float delay, bool loadScene = false, bool quit = false) //화면 암전 풀거나 걸기
    {
        float elapsedTime = 0f;

        Color startColor = I_curtain.color;
        Color endColor   = I_curtain.color;
        if (open)
        {
            startColor.a = 1f;
            endColor.a   = 0f;
        }
        else
        {
            startColor.a = 0f;
            endColor.a   = 1f;
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
            yield return StartCoroutine(Lerp.LerpValue<float>(volume => AS_mainLoop.volume = volume, 1, 0, 1, Mathf.Lerp));
            SceneManager.LoadScene("Levels");
        }

        if (quit)
        {
            yield return StartCoroutine(Lerp.LerpValue<float>(volume => AS_mainLoop.volume = volume, 1, 0, 1, Mathf.Lerp));
            Application.Quit();
        }
    }
}