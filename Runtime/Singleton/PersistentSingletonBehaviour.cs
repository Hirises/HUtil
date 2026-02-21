using UnityEngine;

namespace HUtil.Runtime.Singleton
{
    /// <summary>
    /// 런타임에 동적으로 생성되는 싱글톤 DontDestroyOnLoad 오브젝트
    /// </summary>
    /// <typeparam name="T">싱글톤 클래스</typeparam>
    public class PersistentSingletonBehaviour<T> : MonoBehaviour where T : PersistentSingletonBehaviour<T>
    {
        private static T _instance;
        /// <summary>
        /// 싱글톤 인스턴스<br>
        /// 최초 get 시점에 씬에 자동으로 추가됩니다
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }
    }
}
