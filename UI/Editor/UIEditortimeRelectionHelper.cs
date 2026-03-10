using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Editor;

namespace HUtil.UI.Editor
{
    public static class UIEditortimeReflectionHelper
    {
        public static List<BindingInfo> GetAllResolvedBindingInfos(ViewModelResolver viewModelResolver, List<BindingInfo> output = null)
        {
            Type viewModelType = InspectorHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel)).FirstOrDefault(type => type.FullName == viewModelResolver.ViewModelType);
            var bindingInfos = UIRuntimeReflectionHelper.GetAllBindingInfos(viewModelType);
            output ??= new();
            foreach(var bindingInfo in bindingInfos){
                var internalPropertyPath = viewModelResolver.ConvertPropertyPath(bindingInfo.PropertyPath);
                output.Add(new BindingInfo(internalPropertyPath, bindingInfo.Type, bindingInfo.AllowedDirection));
            }
            return output;
        }

        public static List<Type> GetAllViewModelTypes(UIComponent uiComponent)
        {
            var typeNameSet = uiComponent.ViewModelResolvers.Select(vmr => vmr.ViewModelType).ToHashSet();
            return InspectorHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel)).Where(typeStr => typeNameSet.Contains(typeStr.FullName)).ToList();
        }

        public static List<string> GetAllBindablePropertyNames(UIComponent uiComponent, BindingType receivingType, BindingMode bindingMode)
        {
            List<BindingInfo> output = new();
            foreach(var viewModelResolver in uiComponent.ViewModelResolvers){
                GetAllResolvedBindingInfos(viewModelResolver, output);
            }
            return output.Where(b => b.CanAccept(receivingType, bindingMode)).Select(b => b.PropertyPath).ToList();
        }
    }
}
