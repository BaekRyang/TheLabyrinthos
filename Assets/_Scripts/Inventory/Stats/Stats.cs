using Tayx.Graphy.Utils.NumString;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    private Slider   hp;
    private Slider   exp;
    private Slider   speed;
    private Slider   defence;
    private TMP_Text level;

    private void Awake()
    {
        hp = transform.Find("HP").GetChild(0).GetComponent<Slider>();
        exp = transform.Find("EXP").GetChild(0).GetComponent<Slider>();
        speed = transform.Find("Speed").GetChild(0).GetComponent<Slider>();
        defence = transform.Find("Defence").GetChild(0).GetComponent<Slider>();
        level = transform.Find("Level").GetChild(1).GetComponent<TMP_Text>();
    }
    

    public void UpdateUI()
    {
        var player = Player.Instance;
        
        hp.value      = player.PS_playerStats.Health;
        hp.maxValue   = player.PS_playerStats.MaxHealth;
        exp.value     = player.PS_playerStats.Exp;
        speed.value   = (player.PS_playerStats.Speed * player.WP_weapon.f_speedMult * 100).ToInt();
        defence.value = player.PS_playerStats.Defense;
        level.text    = player.PS_playerStats.Level.ToString();
    }
}
