using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoPrinter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] string s_name;
    [SerializeField] string s_desc;
    bool b_hover;
    RectTransform infoBox;

    void Start()
    {
        infoBox = InventoryManager.Instance.RT_descBox;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        b_hover = true;
        if (name == "ItemCell(Clone)") return;

        infoBox.gameObject.SetActive(true);

        infoBox.position = Input.mousePosition;

        infoBox.localScale = Vector3.one;
        infoBox.GetChild(0).GetChild(0).GetComponentInChildren<TMP_Text>().text = s_name;
        infoBox.GetChild(0).GetChild(1).GetComponentInChildren<TMP_Text>().text = s_desc.Replace("\\n", "\n");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        b_hover = false;
        infoBox.localScale = Vector3.zero;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (b_hover)
        {
            infoBox.position = Input.mousePosition + new Vector3(10, -20, 0);
        }
    }
}
