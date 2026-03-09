using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Editor;

namespace HUtil.UI.Editor
{
    public static class UIReflectionHelper
    {
        public static List<Type> GetAllViewModelTypes(UIComponent uiComponent)
        {
            var typeNameSet = uiComponent.ViewModelResolvers.Select(vmr => vmr.ViewModelType).ToHashSet();
            return InspectorHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel)).Where(typeStr => typeNameSet.Contains(typeStr.FullName)).ToList();
        }

        public static List<string> GetAllBindablePropertyNames(UIComponent uiComponent, BindingType receivingType, BindingMode bindingMode)
        {
            List<string> output = new();
            foreach(Type t in GetAllViewModelTypes(uiComponent))
            {
                BinderReflectionHelper.GetAllBindablePropertyNames(t, receivingType, bindingMode);
            }
            return output;
        }
    }
}
