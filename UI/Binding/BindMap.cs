using System;
using System.Collections.Generic;

namespace HUtil.UI
{
    public struct BindMap
    {
        public Dictionary<string, IViewModelProperty> Properties { get; private set; }
    }
}