namespace HUtil.Runtime.Singleton
{
    /// <summary>
    /// 런타임에 동적으로 생성되는 싱글톤 클래스
    /// </summary>
    /// <typeparam name="T">싱글톤 클래스</typeparam>
    public abstract class SingletonStaticClass<T> where T : SingletonStaticClass<T>, new()
    {
        private static T _instance;
        /// <summary>
        /// 싱글톤 인스턴스<br>
        /// 최초 get 시점에 최초 생성됩니다
        /// </summary>
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
    }
}
