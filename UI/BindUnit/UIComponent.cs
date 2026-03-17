using UnityEngine;

using HUtil.UI.Binder;
using System.Collections.Generic;
using Unity.Collections;
using System;
using HUtil.Runtime.Observable;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;

namespace HUtil.UI
{
    /// <summary>
    /// 재사용 가능한 UI 오브젝트의 단위
    /// </summary>
    public class UIComponent : MonoBinder
    {
        public override bool IsRootBinder => true;

        [InfoBox("ExternalBindingInfos는 외부에서 제공하는 바인딩 정보입니다. 외부를 알지 못하는 상황에서 참조를 선언하기 위해 사용합니다")]
        [SerializeField, ListDrawerSettings(ShowItemCount = true, CustomAddFunction = nameof(AddBindingInfo))]
        public List<BindingInfo> ExternalBindingInfos = new List<BindingInfo>();

        [InfoBox("InternalBindingInfos는 컴포넌트 내부에서 처리되는 바인딩 정보입니다. 자동으로 프로퍼티를 생성하여 할당합니다")]
        [SerializeField, ListDrawerSettings(ShowItemCount = true, CustomAddFunction = nameof(AddBindingInfo))]
        public List<BindingInfo> InternalBindingInfos = new List<BindingInfo>();

        private Dictionary<string, IViewModelProperty> _properties = new Dictionary<string, IViewModelProperty>();

        private BindingInfo AddBindingInfo(){
            return new BindingInfo(string.Empty, BindingType.Invalid, BindingDirectionFlags.Both);;
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            //pass
        }

        protected override void BeforePropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            _properties.Clear();
            foreach(var info in InternalBindingInfos){
                if(bindMap.TryGetValue(info.PropertyPath, out var property)){
                    _properties[info.PropertyPath] = property;
                }
                //collection처리도 GetObservableProperty()에서 해줌
                switch(info.Type.BaseType){
                    case BindingBaseType.Int:
                        bindMap[info.PropertyPath] = new StaticProperty<int>(info.Type.GetObservableProperty());
                        break;
                    case BindingBaseType.Float:
                        bindMap[info.PropertyPath] = new StaticProperty<float>(info.Type.GetObservableProperty());
                        break;
                    case BindingBaseType.String:
                        bindMap[info.PropertyPath] = new StaticProperty<string>(info.Type.GetObservableProperty());
                        break;
                    case BindingBaseType.Bool:
                        bindMap[info.PropertyPath] = new StaticProperty<bool>(info.Type.GetObservableProperty());
                        break;
                    case BindingBaseType.Enum:
                        bindMap[info.PropertyPath] = new StaticProperty<Enum>(info.Type.GetObservableProperty());
                        break;
                    case BindingBaseType.Color:
                        bindMap[info.PropertyPath] = new StaticProperty<Color>(info.Type.GetObservableProperty());
                        break;
                    case BindingBaseType.Sprite:
                        bindMap[info.PropertyPath] = new StaticProperty<Sprite>(info.Type.GetObservableProperty());
                        break;
                    case BindingBaseType.ViewModel:
                        bindMap[info.PropertyPath] = new StaticProperty<IViewModel>(info.Type.GetObservableProperty());
                        break;
                    case BindingBaseType.Command:
                    default:
                        break;
                }
            }
        }

        protected override void AfterPropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            foreach(var property in _properties){
                bindMap[property.Key] = property.Value;
            }
        }

        internal override Dictionary<string, BindingInfo> GetAllProvidingBindingInfosEditor()
        {
            var infos = base.GetAllProvidingBindingInfosEditor();
            foreach(var info in InternalBindingInfos)
            {
                infos[info.PropertyPath] = info;
            }
            foreach(var info in ExternalBindingInfos)
            {
                infos[info.PropertyPath] = info;
            }
            return infos;
        }
    }
}
