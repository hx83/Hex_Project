using System.Collections.Generic;
using Umeng;

public class GameAnalysisMgr:SingletonBase<GameAnalysisMgr>
{
    private Dictionary<string, bool> _dictHasData;

    public void SetUp()
    {
        _dictHasData = new Dictionary<string, bool>();
    }


    public void AddAnalysis(string level)
    {
        if (_dictHasData.ContainsKey(level))
            return;
        GA.StartLevel(level);
        GA.FinishLevel(level);
        _dictHasData.Add(level, true);
        string evtID = level;
        GA.Event(evtID);
        //Debuger.LogWarning("[GameAnalysisMgr.AddAnalysis() => level:" + level + "]");
    }
}