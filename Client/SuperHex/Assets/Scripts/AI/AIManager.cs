using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using Behavior3CSharp;
using Behavior3CSharp.Core;
using AI;

public class AIManager 
{
    private static JsonData json;
	public static void Setup()
    {
        B3Config.Setup();

        string aipath = Application.streamingAssetsPath + "/ai.json";


        FileStream file = new FileStream(aipath, FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(file);
        string str = sr.ReadToEnd();
        //Debug.WriteLine(str);
        json = B3Config.Parse(str);
        


        //tree = new BehaviorTree();
        //tree.Load(json);
    }

    public static RobotBTree GetTree(Player p)
    {
        PlayerAIExt ai = new PlayerAIExt(p);
        RobotBTree tree = new RobotBTree(ai);
        tree.Load(json);

        return tree;
    }
}
