using System;
using System.Collections.Generic;

using HUtil.UI.Binder;

using UnityEditor;

using UnityEngine;

namespace HUtil.UI.Editor
{
    [InitializeOnLoad]
    public static class BinderEditorEvent
    {
        static BinderEditorEvent()
        {
            ObjectChangeEvents.changesPublished += OnChangesPublished;
        }

        private static void OnChangesPublished(ref ObjectChangeEventStream stream)
        {
            for (int i = 0; i < stream.length; i++)
            {
                var type = stream.GetEventType(i);

                switch(type)
                {
                    case ObjectChangeKind.ChangeGameObjectStructure:    //컴포넌트 추가 & 제거 & 위치 이동
                    {
                        stream.GetChangeGameObjectStructureEvent(i, out var changeEvent);
                        UpdateInfluencedBinderList(changeEvent.instanceId);
                        break;
                    }
                    case ObjectChangeKind.ChangeChildrenOrder:     //오브젝트 순서 변경
                    {
                        stream.GetChangeChildrenOrderEvent(i, out var changeEvent);
                        UpdateInfluencedBinderList(changeEvent.instanceId);
                        break;
                    }
                    case ObjectChangeKind.ChangeRootOrder:     //오브젝트 순서 변경
                    {
                        stream.GetChangeRootOrderEvent(i, out var changeEvent);
                        UpdateInfluencedBinderList(changeEvent.instanceId);
                        break;
                    }
                    case ObjectChangeKind.ChangeGameObjectParent:     //오브젝트 부모 변경
                    {
                        stream.GetChangeGameObjectParentEvent(i, out var changeEvent);
                        UpdateInfluencedBinderList(changeEvent.previousParentInstanceId);
                        UpdateInfluencedBinderList(changeEvent.newParentInstanceId);
                        UpdateInfluencedBinderList(changeEvent.instanceId);
                        break;
                    }
                    case ObjectChangeKind.CreateGameObjectHierarchy:     //오브젝트 생성
                    {
                        stream.GetCreateGameObjectHierarchyEvent(i, out var changeEvent);
                        UpdateInfluencedBinderList(changeEvent.instanceId);
                        break;
                    }
                    case ObjectChangeKind.DestroyGameObjectHierarchy:     //오브젝트 삭제
                    {
                        stream.GetDestroyGameObjectHierarchyEvent(i, out var changeEvent);
                        UpdateInfluencedBinderList(changeEvent.parentInstanceId);
                        break;
                    }
                }
            }
        }

        private static void UpdateInfluencedBinderList(int instanceId)
        {
            var go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if(go == null){
                return;
            }

            //자기자신 업데이트
            var binders = go.GetComponents<MonoBinder>();
            foreach(var binder in binders)
            {
                binder.UpdateBinderList();
            }

            //자식들 업데이트
            var childBinders = FindChildBinders(go);
            foreach(var binder in childBinders)
            {
                binder.UpdateBinderList();
            }

            //부모 업데이트
            var parentBinder = FindParentBinder(go);
            if(parentBinder != null){
                parentBinder.UpdateBinderList();
            }
        }
        
        private static List<MonoBinder> FindChildBinders(GameObject go, List<MonoBinder> output = null)
        {
            output ??= new List<MonoBinder>();
            output.Clear();

            foreach(Transform child in go.transform){
                //자식들을 재귀적으로 검색
                FindChildBindersInternal(child, output);
            }

            return output;
        }
    
        private static void FindChildBindersInternal(Transform self, List<MonoBinder> childBinders)
        {
            for(int i = 0; i < self.gameObject.GetComponentCount(); i++){
                var comp = self.gameObject.GetComponentAtIndex(i);
                if(comp is MonoBinder binder){
                    childBinders.Add(binder);
                    if(binder.IsRootBinder){
                        //하위로 전파하는 객체면 여기까지
                        return;
                    }
                }
            }
            foreach(Transform child in self.transform){
                //자식들을 재귀적으로 검색
                FindChildBindersInternal(child, childBinders);
            }
        }
    
        /// <summary>
        /// 이 바인더를 관리할 책임이 있는 상위 바인더를 찾습니다
        /// </summary>
        /// <returns></returns>
        public static MonoBinder FindParentBinder(GameObject go)
        {
            foreach(var binder in EnumerateParentBinders(go)){
                if(binder.IsRootBinder){
                    return binder;
                }
            }
            return null;
        }
    
        /// <summary>
        /// 상위 바인더들을 순차적으로 검색합니다
        /// </summary>
        /// <returns>상위 바인더</returns>
        private static IEnumerable<MonoBinder> EnumerateParentBinders(GameObject go)
        {
            var curObject = go.transform.parent;
            while(curObject != null){
                for(int i = curObject.gameObject.GetComponentCount() - 1; i >= 0; i--){
                    var comp = curObject.gameObject.GetComponentAtIndex(i);
                    if(comp is MonoBinder binder){
                        yield return binder;
                    }
                }
                curObject = curObject.parent;
            }
        }
    }
}