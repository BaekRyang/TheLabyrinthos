using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SystemAlerts : MonoBehaviour
{
    public MMF_Player systemAlert;
    public TMP_Text   systemText;
    
    public TMP_Text   inspectorText;
    
    public void Alert(string text)
    {
        systemText.text = text;
        if(systemAlert.IsPlaying)
        {
            systemAlert.StopFeedbacks();
            systemAlert.RestoreInitialValues();
        }
        systemAlert.PlayFeedbacks();
    }

    public void ShowInspect(string text)
    {
        inspectorText.text    = text;
        inspectorText.enabled = true;
    }
    
    public void HideInspect()
    {
        inspectorText.enabled = false;
    }
}
