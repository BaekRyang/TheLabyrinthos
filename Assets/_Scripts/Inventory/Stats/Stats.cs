using Tayx.Graphy.Utils.NumString;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    private Slider hp;
    private Slider exp;
    private Slider speed;
    private Slider defence;

    private void Awake()
    {
        hp = transform.Find("HP").GetChild(0).GetComponent<Slider>();
        exp = transform.Find("EXP").GetChild(0).GetComponent<Slider>();
        speed = transform.Find("Speed").GetChild(0).GetComponent<Slider>();
        defence = transform.Find("Defence").GetChild(0).GetComponent<Slider>();
    }
    

    public void UpdateUI()
    {
        var player = Player.Instance;
        
        hp.value      = player.PS_playerStats.health;
        hp.maxValue   = player.PS_playerStats.maxHealth;
        exp.value     = player.PS_playerStats.exp;
        speed.value   = (player.PS_playerStats.speed * player.WP_weapon.f_speedMult*100).ToInt();
        defence.value = player.PS_playerStats.defense;
    }
}
