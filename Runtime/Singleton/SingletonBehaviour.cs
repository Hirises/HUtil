using UnityEngine;

namespace HUtil.Runtime.Singleton
{
    /// <summary>
    /// 런타임에 바인딩되는 싱글톤 오브젝트
    /// </summary>
    /// <typeparam name="T">싱글톤 클래스</typeparam>
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T _instance;
        /// <summary>
        /// 싱글톤 인스턴스
        /// </summary>
        public static T Instance => _instance;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
