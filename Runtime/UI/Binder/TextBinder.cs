using UnityEngine;
using System;
using TMPro;

namespace HUtil.UI.Binder
{
    public class TextBinder : MonoBinder
    {
        [SerializeField] private TMP_Text target;

        private void Reset()
        {
            target = GetComponent<TMP_Text>();
        }

        public override void Bind(object data)
        {
            target.text = data as string;
        }
    }
}
