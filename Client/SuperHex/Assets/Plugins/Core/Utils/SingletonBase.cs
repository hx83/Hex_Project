public class SingletonBase<T>:EventDispatcher where T : new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
	/// <summary>
	/// 新写法
	/// </summary>
	/// <value>The ins.</value>
	public static T Ins
	{
		get
		{
			if (_instance == null)
			{
				_instance = new T();
			}
			return _instance;
		}
	}
}

