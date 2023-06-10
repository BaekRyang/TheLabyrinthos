using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public struct PopUpInfo
    {
        public RectTransform box;

        public TMP_Text   title;
        public GameObject titleObject;

        public TMP_Text   desc;
        public GameObject descObject;

        public TMP_Text   inspect;
        public GameObject inspectObject;
    }

    public static PopUpManager Instance;

    public PopUpInfo     infoBoxP;
    public RectTransform infoBox;
    public TMP_Text      infoBoxTitle;
    public TMP_Text      infoBoxDesc;
    public TMP_Text      infoBoxInspect;

    public PopUpInfo     infoBoxBattleP;
    public RectTransform infoBoxBattle;
    public TMP_Text      infoBoxBattleTitle;
    public TMP_Text      infoBoxBattleDesc;
    public TMP_Text      infoBoxBattleInspect;

    public PopUpInfo     descBoxP;
    public RectTransform descBox;
    public TMP_Text      descBoxTitle;
    public TMP_Text      descBoxDesc;

    public ItemActionHandler itemActionHandler;
    public ItemActionHandler itemActionHandlerBattle;

    private void Awake()
    {
        infoBoxP =
            new PopUpInfo()
            {
                box           = infoBox,
                title         = infoBoxTitle,
                titleObject   = infoBoxTitle.transform.parent.gameObject,
                desc          = infoBoxDesc,
                descObject    = infoBoxDesc.transform.parent.gameObject,
                inspect       = infoBoxInspect,
                inspectObject = infoBoxInspect.transform.parent.parent.gameObject
            };

        infoBoxBattleP =
            new PopUpInfo()
            {
                box           = infoBoxBattle,
                title         = infoBoxBattleTitle,
                titleObject   = infoBoxBattleTitle.transform.parent.gameObject,
                desc          = infoBoxBattleDesc,
                descObject    = infoBoxBattleDesc.transform.parent.gameObject,
                inspect       = infoBoxBattleInspect,
                inspectObject = infoBoxBattleInspect.transform.parent.parent.gameObject
            };

        descBoxP =
            new PopUpInfo()
            {
                box         = descBox,
                title       = descBoxTitle,
                titleObject = descBoxTitle.transform.parent.gameObject,
                desc        = descBoxDesc,
                descObject  = descBoxDesc.transform.parent.gameObject
            };

        infoBox.gameObject.SetActive(false);
        infoBoxInspect.transform.parent.parent.gameObject.SetActive(false);
        
        infoBoxBattle.gameObject.SetActive(false);
        infoBoxBattleInspect.transform.parent.parent.gameObject.SetActive(false);
        
        descBox.gameObject.SetActive(false);
    }
    
    public IEnumerator LoadSetting()
    {
        Instance = this;
        yield return null;
    }

    public static IEnumerator UpdateUI(PopUpInfo pack)
    {
        pack.box.GetComponent<CanvasGroup>().alpha = 0;
        yield return null;
        RectTransform infoBoxTitleRect   = pack.title.transform.parent.GetComponent<RectTransform>();
        RectTransform infoBoxDescRect    = pack.desc.transform.parent.GetComponent<RectTransform>();
        RectTransform infoBoxInspectRect = null;

        var descSize = infoBoxDescRect.sizeDelta;

        
        if (pack.inspect != null)
        {
            infoBoxInspectRect = pack.inspect.transform.parent.parent.GetComponent<RectTransform>();
            var infoBoxInspectRect2 = pack.inspect.GetComponent<RectTransform>();
            if (infoBoxInspectRect.gameObject.activeSelf)
            {
                infoBoxInspectRect2.sizeDelta = new Vector2(descSize.x - 10, pack.inspect.preferredHeight);
            }
        }

        if (pack.box.gameObject.activeSelf)
        {
            infoBoxTitleRect.sizeDelta =
                new Vector2(descSize.x, 30);

            if (infoBoxInspectRect != null)
                infoBoxInspectRect.localPosition = new Vector2(0, -(descSize.y + 30));
        }

        yield return null;
        pack.box.GetComponent<CanvasGroup>().alpha = 1;
    }
}