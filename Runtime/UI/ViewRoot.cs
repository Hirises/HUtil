using UnityEngine;

using HUtil.Runtime.UI.Binder;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// 재사용 가능한 UI 오브젝트의 단위
    /// </summary>
    public class ViewRoot : MonoBehaviour
    {
        [SerializeField] private MonoBinder[] binders;

        private void Reset()
        {
            binders = GetComponentsInChildren<MonoBinder>();
        }

        public void Bind(object data)
        {
            foreach (var binder in binders)
            {
                binder.Bind(data);
            }
        }
    }
}
