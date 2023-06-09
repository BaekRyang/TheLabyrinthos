using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainScene : MonoBehaviour
{
    [SerializeField] DataCarrier dataCarrier;

    [SerializeField] Image       I_curtain;
    [SerializeField] Image       I_curtainOverVideo;
    [SerializeField] AudioSource AS_mainLoop;
    [SerializeField] GameObject  GO_mainUI;

    [SerializeField] GameObject[] GO_lights;
    [SerializeField] Light[]      LIT_lights;
    [SerializeField] Volume       VOL_volume;

    [SerializeField] GameObject GO_targetPosition;
    [SerializeField] GameObject GO_defaultPosition;

    [SerializeField] GameObject GO_whiteBoard;
    private          Material   MAT_logos;

    [SerializeField] GameObject[] GO_cameras;

    [SerializeField] VideoPlayer videoPlayer;

    bool b_loaded;

    void Start()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
        //Screen.SetResolution(2560, 1440, true);
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
            StartCoroutine(Lerp.LerpValue(alpha => tmpCG.alpha = alpha, 0, 1f, 1, Mathf.Lerp));
            StartCoroutine(Lerp.LerpValue(intense =>
            {
                for (int i = 0; i < 12; i++)
                    LIT_lights[i].intensity = intense;
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
            StartGame();
        else if (button == "Exit")
            StartCoroutine(CurtainModify(false, 1, false, true));
    }

    public void StartGame()
    {
        StartCoroutine(Lerp.LerpValue(volume => AS_mainLoop.volume = volume, 1, 0f, 2, Mathf.Lerp));
        StartCoroutine(CurtainModify(false, 2, true));
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
        yield return StartCoroutine(Lerp.LerpValue(value => I_curtainOverVideo.color = value, new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), 1, Color.Lerp));

        while (true)
        {
            //videoPlayer가 실행된지 1초 이상이면서 비디오가 재생중이 아니라면
            if (videoPlayer.time > 1 && !videoPlayer.isPlaying || Input.GetMouseButtonDown(0))
            {
                videoPlayer.Stop();
                break;
            }
            yield return null;
        }

        yield return StartCoroutine(Lerp.LerpValue(value => I_curtainOverVideo.color = value, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), 1, Color.Lerp));
        
        DestroyImmediate(videoPlayer.gameObject);
        DestroyImmediate(I_curtainOverVideo.gameObject);

        yield return new WaitForSeconds(1);
        StartCoroutine(Lerp.LerpValue<float>(volume => AS_mainLoop.volume = volume, 0, 1, 4, Mathf.Lerp));
        yield return new WaitForSeconds(1);
        StartCoroutine(CurtainModify(true, 2));
        yield return new WaitForSeconds(3);
        b_loaded = true;
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