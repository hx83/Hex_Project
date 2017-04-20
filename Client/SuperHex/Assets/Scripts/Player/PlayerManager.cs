using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 角色管理器
/// </summary>
public class PlayerManager 
{
    private static uint playerCount;
    private static Dictionary<uint,Player> dict;
    public static void Setup()
    {
        playerCount = 0;
        dict = new Dictionary<uint, Player>();
    }
    public static void Clear()
    {
        playerCount = 0;
        dict.Clear();
    }
    public static void Update()
    {
        UpdateAllPlayer();
    }
    public static HeroPlayer CreateHero()
    {
        playerCount++;
        HeroPlayer p = new HeroPlayer();
        p.ID = playerCount;
        AddPlayer(p);
        return p;
    }
    public static Player CreatePlayer()
    {
        playerCount++;
        Player p = new Player();
        p.ID = playerCount;
        AddPlayer(p);
        return p;
    }

    public static Player GetPlayer(uint id)
    {
        if(dict.ContainsKey(id) == false)
        {
            return null;
        }
        else
        {
            return dict[id];
        }
    }
    private static void UpdateAllPlayer()
    {
        foreach (var item in dict)
        {
            item.Value.Update();
        }
    }
    private static void AddPlayer(Player player)
    {
        dict.Add(player.ID, player);
    }

    public static void HitPlayer(Player Attacker,Player Defender)
    {
        Debug.Log("hit player");
        if(Attacker == Defender)
        {
            Attacker.Die();
        }
        else
        {
            Defender.Die();
        }
    }
}
