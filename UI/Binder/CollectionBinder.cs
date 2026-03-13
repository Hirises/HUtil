using System;
using System.Collections.Generic;
using System.ComponentModel;

using HUtil.Runtime.Observable;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI.Binder
{
    public class CollectionBinder : MonoBinder
    {
        public override bool IsRootBinder => true;
        protected override bool PropagateBinding => false;

        [SerializeField] private CollectionBindingPort _list_prop = new CollectionBindingPort(BindingBaseType.ViewModel, BindingDirectionFlags.ToUI);
        [SerializeField] private MonoResolver _itemPrefab;
        [SerializeField] private bool _autoDestroyItem = true;

        private List<MonoResolver> _items = new List<MonoResolver>();

        protected void OnValidate()
        {
            _list_prop.Validate(this);
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            if(_autoDestroyItem){
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    if(child.gameObject == _itemPrefab.gameObject){
                        child.gameObject.SetActive(false);
                        continue;
                    }
                    Destroy(child.gameObject);  //즉시 삭제가 아니라 예약 동작이라 CollectionModifiedException에서 자유로움
                }
            }

            _list_prop.Bind(bindMap, disposable, SetList);
        }

        protected override void UnbindInternal()
        {
            foreach (var item in _items)
            {
                item.Unbind();
                Destroy(item.gameObject);
            }
            _items.Clear();
            base.UnbindInternal();
        }

        private void SetList(ListChangeEvent<IViewModel> @event)
        {
            switch (@event.Action)
            {
                case ListChangeAction.Add:
                {
                    var item = Instantiate(_itemPrefab, transform);
                    item.ManualBind(@event.Item);
                    item.gameObject.SetActive(true);
                    _items.Insert(@event.Index, item);
                    break;
                }
                case ListChangeAction.Remove:
                {
                    var item = _items[@event.Index];
                    item.Unbind();
                    Destroy(item.gameObject);
                    _items.RemoveAt(@event.Index);
                    break;
                }
                case ListChangeAction.Replace:
                {
                    var item = _items[@event.Index];
                    item.Unbind();
                    item.ManualBind(@event.Item);
                    break;
                }
                case ListChangeAction.Clear:
                {
                    foreach (var item in _items)
                    {
                        item.Unbind();
                        Destroy(item.gameObject);
                    }
                    _items.Clear();
                    break;
                }
            }
        }
    }
}