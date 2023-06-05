using System;
using System.Collections;
using System.Collections.Generic;
using TypeDefs;
using Unity.VisualScripting;
using UnityEngine;


public class CreatureManager : MonoBehaviour
{
    //변화값
    const float ATTACK_MULT  = 1.25f;
    const float DEFENCE_MULT = 1.2f;
    const float HP_MULT      = 1.25f;
    const float SPEED_MULT   = 1.0f;

    //조정값
    const float ATTACK_ADJ  = -0.02f;
    const float DEFENCE_ADJ = -0.01f;
    const float HP_ADJ      = -0.01f;
    const float SPEED_ADJ   = 0.0f;

    public static CreatureManager Instance;

    [SerializeField] private CreatureSpritePack[]                   sprites;
    [DoNotSerialize] public  Dictionary<string, CreatureSpritePack> spritesDictionary = new Dictionary<string, CreatureSpritePack>();

    [SerializeField] private List<Creature> creatures = new List<Creature>();

    private void Awake()
    {
        Instance ??= this;
    }

    public IEnumerator LoadSettings()
    {
        foreach (var sprite in sprites)
        {
            spritesDictionary.Add(sprite.creatureName, sprite);
        }

        creatures.Add(new Creature(8, 3, 50.0f, 0.8f, 0, GetSpritePack(1, 1))); //기본 크리쳐

        //초기값을 통하여 스텟 변화계산후 적용
        for (int i = 1; i < 8; i++)
        {
            int   atk = (int)MathF.Round((creatures[i - 1].damage  * (ATTACK_MULT  + ATTACK_ADJ  * i)), 0);
            int   def = (int)MathF.Round((creatures[i - 1].defense * (DEFENCE_MULT + DEFENCE_ADJ * i)), 0);
            float hp  = (float)Math.Round(creatures[i - 1].health * (HP_MULT + HP_ADJ * i), 1);
            float spd = creatures[i - 1].speed * (SPEED_MULT + SPEED_ADJ * i);
            creatures.Add(new Creature(atk, def, hp, spd, 0, GetSpritePack(1, 1)));
        }

        yield return null;
    }

    public CreatureSpritePack GetSpritePack(int level, int type = -1)
    {
        if (type == -1) //타입을 입력하지 않았으면
            type = UnityEngine.Random.Range(1, 4); //1~3 사이에서 무작위 크리쳐 이미지를 준다.

        return spritesDictionary["L" + level + "_Creature" + type];
    }

    public CreatureSpritePack GetSpritePack(string creatureName)
    {
        return spritesDictionary[creatureName];
    }

    public Creature GetCreatureStats(int level)
    {
        if (level > creatures.Count) //인수가 크리쳐 리스트의 크기보다 크면 마지막 크리쳐를 반환
            return new Creature(creatures[^1]);

        if (level < 1) //인수가 1보다 작으면 첫번째 크리쳐를 반환
            return new Creature(creatures[0]);

        return new Creature(creatures[level - 1]);
    }

    public Creature GetRandomCreatureByLevel(int level)
    {
        var creature = GetCreatureStats(level);

        creature.spritePack = GetSpritePack(level);

        return creature;
    }
}