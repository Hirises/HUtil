using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using UnityEngine.Events;

namespace HUtil.UI
{
    public interface IBindingPort
    {
        public string Path { get; }
        public BindingMode Direction { get; }
    }
}