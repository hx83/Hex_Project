/// <summary>
/// ACE
/// </summary>
public interface IAppModule
{
    void Show(object obj = null);
    void Hide();
    void Destroy();

    void SetSiblingIndex(int value);
}
