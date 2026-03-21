using Unity.Properties;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using HUtil.Runtime.Observable;
using HUtil.UI.Binder;
using System;

namespace HUtil.UI
{
    public class UIPropertyInspector : MonoBinder
    {
        public override bool IsRootBinder => true;
        protected override bool PropagateBinding => true;

        [SerializeField, OnValueChanged(nameof(OnUIComponentChanged))] private UIComponent _selectedComponent;
        [ListDrawerSettings(DefaultExpandedState = true, HideAddButton = true, HideRemoveButton = true)]
        [SerializeField] private List<PropertyInspector> _properties = new List<PropertyInspector>();

        private Dictionary<string, IViewModelProperty> prev = new Dictionary<string, IViewModelProperty>();

        protected override void Reset()
        {
            base.Reset();
            _selectedComponent = GetComponent<UIComponent>();
            OnUIComponentChanged(_selectedComponent);
        }

        private void Start()
        {
            Bind(new Dictionary<string, IViewModelProperty>());
        }

        private void OnUIComponentChanged(UIComponent component)
        {
            _properties.Clear();
            if(component != null)
            {
                foreach(var info in component.ExternalBindingInfos)
                {
                    _properties.Add(new PropertyInspector{
                        Name = info.PropertyPath,
                        Type = info.Type,
                        Property = info.Type.GetObservableProperty() as IObservableProperty
                    });
                }
            }
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {

        }

        protected override void BeforePropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            prev.Clear();
            foreach(var propertyInsp in _properties){
                if(bindMap.TryGetValue(propertyInsp.Name, out var property)){
                    prev[propertyInsp.Name] = property;
                }
                //collection처리도 GetObservableProperty()에서 해줌
                if(propertyInsp.Type.IsCollection){
                    switch(propertyInsp.Type.BaseType){
                        case BindingBaseType.Int:
                            bindMap[propertyInsp.Name] = new StaticProperty<int>(propertyInsp.Property as ObservableList<int>);
                            break;
                        case BindingBaseType.Float:
                            bindMap[propertyInsp.Name] = new StaticProperty<float>(propertyInsp.Property as ObservableList<float>);
                            break;
                        case BindingBaseType.String:
                            bindMap[propertyInsp.Name] = new StaticProperty<string>(propertyInsp.Property as ObservableList<string>);
                            break;
                        case BindingBaseType.Bool:
                            bindMap[propertyInsp.Name] = new StaticProperty<bool>(propertyInsp.Property as ObservableList<bool>);
                            break;
                        case BindingBaseType.Enum:
                            bindMap[propertyInsp.Name] = new StaticProperty<Enum>(propertyInsp.Property as ObservableList<Enum>);
                            break;
                        case BindingBaseType.Color:
                            bindMap[propertyInsp.Name] = new StaticProperty<Color>(propertyInsp.Property as ObservableList<Color>);
                            break;
                        case BindingBaseType.Sprite:
                            bindMap[propertyInsp.Name] = new StaticProperty<Sprite>(propertyInsp.Property as ObservableList<Sprite>);
                            break;
                        case BindingBaseType.ViewModel:
                            bindMap[propertyInsp.Name] = new StaticProperty<IViewModel>(propertyInsp.Property as ObservableList<IViewModel>);
                            break;
                        case BindingBaseType.Command:
                        default:
                            break;
                    }
                }else{
                    switch(propertyInsp.Type.BaseType){
                        case BindingBaseType.Int:
                            bindMap[propertyInsp.Name] = new StaticProperty<int>(propertyInsp.Property as ObservableProperty<int>);
                            break;
                        case BindingBaseType.Float:
                            bindMap[propertyInsp.Name] = new StaticProperty<float>(propertyInsp.Property as ObservableProperty<float>);
                            break;
                        case BindingBaseType.String:
                            bindMap[propertyInsp.Name] = new StaticProperty<string>(propertyInsp.Property as ObservableProperty<string>);
                            break;
                        case BindingBaseType.Bool:
                            bindMap[propertyInsp.Name] = new StaticProperty<bool>(propertyInsp.Property as ObservableProperty<bool>);
                            break;
                        case BindingBaseType.Enum:
                            bindMap[propertyInsp.Name] = new StaticProperty<Enum>(propertyInsp.Property as ObservableProperty<Enum>);
                            break;
                        case BindingBaseType.Color:
                            bindMap[propertyInsp.Name] = new StaticProperty<Color>(propertyInsp.Property as ObservableProperty<Color>);
                            break;
                        case BindingBaseType.Sprite:
                            bindMap[propertyInsp.Name] = new StaticProperty<Sprite>(propertyInsp.Property as ObservableProperty<Sprite>);
                            break;
                        case BindingBaseType.ViewModel:
                            bindMap[propertyInsp.Name] = new StaticProperty<IViewModel>(propertyInsp.Property as ObservableProperty<IViewModel>);
                            break;
                        case BindingBaseType.Command:
                        default:
                            break;
                    }
                }
            }
        }

        protected override void AfterPropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            foreach(var property in prev){
                bindMap[property.Key] = property.Value;
            }
        }

        [Serializable]
        private class PropertyInspector {
            [SerializeField, DisplayAsString, HorizontalGroup, HideLabel] public string Name;
            [SerializeField, HorizontalGroup, InlineProperty, ReadOnly, HideLabel] public BindingType Type;
            [SerializeField, HorizontalGroup, InlineProperty, SerializeReference, HideReferenceObjectPicker, HideLabel]
            public IObservableProperty Property;
        }
    }
}