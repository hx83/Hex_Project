using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 角色管理器
/// </summary>
public class PlayerManager 
{
    private static EventDispatcher _dispatcher;

    private static uint playerCount;
    private static Dictionary<uint, Player> dict;
    private static List<Player> list;

    private static uint updateCount;
    public static void Setup()
    {
        _dispatcher = new EventDispatcher();

        updateCount = 0;
        playerCount = 0;
        dict = new Dictionary<uint, Player>();
        list = new List<Player>();
    }
    public static void AddEventListener(string type, System.Action<BaseEvent> listener)
    {
        _dispatcher.AddEventListener(type, listener);
    }

    public static void RemoveEventListener(string type, System.Action<BaseEvent> listener)
    {
        _dispatcher.RemoveEventListener(type, listener);
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
        //
        updateCount++;
        if(updateCount%20 == 0)
        {
            SortScore();
        }
    }
    /// <summary>
    /// 给玩家分数排序
    /// </summary>
    private static void SortScore()
    {
        list.Sort(SortList);

        _dispatcher.DispatchEvent(PlayerEvent.SORT_SCORE, list);
    }
    private static int SortList(Player a, Player b) //a b表示列表中的元素
    {

        if (a.Score > b.Score)
        {
            return 1;

        }

        else if (a.Score < b.Score)
        {

            return -1;

        }

        return 0;

    }

    //
    //

    private static void AddPlayer(Player player)
    {
        dict.Add(player.ID, player);
        list.Add(player);
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
