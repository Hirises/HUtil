using System;

using HUtil.UI.Binder;

using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ActionResolvers;

using UnityEditor;

using UnityEngine;

namespace HUtil.UI.Editor
{
    [DrawerPriority(-1, 0, 0)]
    public class ParentContextResolverAttributeDrawer : OdinAttributeDrawer<ParentContextResolverAttribute>
    {
        private object _parent;
        private ActionResolver _resolver;

        protected override void Initialize()
        {
            _parent = Property.Parent.ValueEntry.WeakSmartValue;
            var args = new NamedValue[] { new NamedValue("parent", typeof(object), _parent) };
            _resolver = ActionResolver.Get(Property.Parent, Attribute.Setter, args);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if(_resolver.HasError)
            {
                _resolver.DrawError();
                CallNextDrawer(label);
                return;
            }
            _resolver.DoActionForAllSelectionIndices();

            CallNextDrawer(label);
        }
    }
}