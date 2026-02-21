namespace HUtil.Runtime.Singleton 
{
    public class SingletonClass<T> where T : class, new()
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
            }
        }
    }
}