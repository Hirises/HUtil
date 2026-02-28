using System;

using UnityEngine;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// Property를 Component에 할당해주는 최종 주체
    /// </summary>
    public abstract class MonoBinder : MonoBehaviour
    {
        public abstract void Bind(object data);
    }
}
