public class AtlasType
{
    public static readonly string BATTLE_MODEL = "BattleModel";
    public static readonly string BATTLE_UI = "Battle";
    public static readonly string COMMON = "Common";
    public static readonly string ICON = "Icon";

    public static string GetAtlasABName(string type)
    {
        return "atlas_" + type.ToLower();
    }
}
