using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI.Binder
{
    public class CollectionBinder : MonoBinder
    {
        protected override bool IsPropagateBindMap => true;

        [SerializeField] private Transform _root;
        [SerializeField] private CollectionBindingInfo _list_prop = new CollectionBindingInfo(BindingDirectionFlags.ToUI);
        [SerializeField] private UIComponent _itemPrefab;
        [SerializeField] private bool _autoDestroyItem = true;

        private List<UIComponent> _items = new List<UIComponent>();

        protected override void Reset()
        {
            base.Reset();
            _root = transform;
        }

        protected void OnValidate()
        {
            _list_prop.Validate();
        }

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            if(_autoDestroyItem){
                for (int i = 0; i < _root.childCount; i++)
                {
                    var child = _root.GetChild(i);
                    if(child.gameObject == _itemPrefab.gameObject){
                        child.gameObject.SetActive(false);
                        continue;
                    }
                    Destroy(child.gameObject);  //즉시 삭제가 아니라 예약 동작이라 CollectionModifiedException에서 자유로움
                }
            }

            _list_prop.Bind(bindMap, disposable, SetList);
        }

        public override void Unbind()
        {
            foreach (var item in _items)
            {
                item.Unbind();
                Destroy(item.gameObject);
            }
            _items.Clear();
            base.Unbind();
        }

        private void SetList(ListChangeEvent<IViewModel> @event)
        {
            switch (@event.Action)
            {
                case ListChangeAction.Add:
                {
                    var item = Instantiate(_itemPrefab, _root);
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