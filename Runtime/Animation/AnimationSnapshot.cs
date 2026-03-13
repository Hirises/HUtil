using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HUtil.Runtime.Extension;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HUtil.Runtime.Animation
{
    /// <summary>
    /// 필드 변경사항을 애니메이션 스냅샷으로 캡처하는 클래스
    /// </summary>
    public class AnimationSnapshot
    {
        private struct PoseData{
            public Type componentType;
            #if UNITY_EDITOR
            public SerializedPropertyType propertyType;
            #endif
            public string propertyPath;
            public object value;
        }

        /// <summary>
        /// 스냅샷을 캡쳐하는 옵션
        /// </summary>
        public struct CaptureOption{
            public Type[] ignoreComponentTypes;
            public string[] ignorePropertyPaths;

            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="ignoreComponentTypes">무시할 컴포넌트 타입(상속은 무시)</param>
            /// <param name="includeComponentTypesDerivedFrom">무시할 컴포넌트 타입(이 타입의 서브클래스도 무시함)</param>
            /// <param name="ignorePropertyPaths">무시할 프로퍼티 경로</param>
            public CaptureOption(Type[] ignoreComponentTypes, Type[] includeComponentTypesDerivedFrom, string[] ignorePropertyPaths){
                List<Type> excludeComponentTypes = new List<Type>();
                excludeComponentTypes.AddRange(ignoreComponentTypes);
                foreach(var type in includeComponentTypesDerivedFrom){
                    excludeComponentTypes.AddRange(RuntimeReflectionHelper.GetAllConcreteTypesDerivedFrom(type));
                }
                this.ignoreComponentTypes = excludeComponentTypes.ToArray();
                this.ignorePropertyPaths = ignorePropertyPaths;
            }
        }
        
        private GameObject targetObject;
        private Dictionary<string, PoseData> poseA = new Dictionary<string, PoseData>();
        private Dictionary<string, PoseData> poseB = new Dictionary<string, PoseData>();
        private bool _isCapturedA = false;
        private bool _isCapturedB = false;
        private CaptureOption? _captureOption;

        /// <summary>
        /// A 포즈가 캡처되었는지 여부
        /// </summary>
        public bool IsCapturedA => _isCapturedA;
        /// <summary>
        /// B 포즈가 캡처되었는지 여부
        /// </summary>
        public bool IsCapturedB => _isCapturedB;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="targetObject">대상 오브젝트</param>
        public AnimationSnapshot(GameObject targetObject, CaptureOption? captureOption = null)
        {
            this.targetObject = targetObject;
            _isCapturedA = false;
            _isCapturedB = false;
            _captureOption = captureOption;
        }
        
        #region public Methods
        /// <summary>
        /// A 포즈를 캡처합니다.
        /// </summary>
        /// <param name="poseDict">캡처된 필드 변경사항</param>
        public void CapturePoseA()
        {
            CapturePose(poseA, _captureOption);
            _isCapturedA = true;
        }

        /// <summary>
        /// B 포즈를 캡처합니다.
        /// </summary>
        /// <param name="poseDict">캡처된 필드 변경사항</param>
        public void CapturePoseB()
        {
            CapturePose(poseB, _captureOption);
            _isCapturedB = true;
        }

        /// <summary>
        /// 모든 포즈를 초기화합니다.
        /// </summary>
        public void ClearAll(){
            ClearPoseA();
            ClearPoseB();
        }

        /// <summary>
        /// A 포즈를 초기화합니다.
        /// </summary>
        public void ClearPoseA(){
            poseA.Clear();
            _isCapturedA = false;
        }

        /// <summary>
        /// B 포즈를 초기화합니다.
        /// </summary>
        public void ClearPoseB(){
            poseB.Clear();
            _isCapturedB = false;
        }
        #endregion

        #region [Internal] Capture Logics
        private void CapturePose(Dictionary<string, PoseData> poseDict, CaptureOption? captureOption = null)
        {
            #if UNITY_EDITOR
            if (targetObject == null) return;
            poseDict.Clear();

            foreach (var comp in targetObject.GetComponents<Component>())
            {
                if(captureOption.HasValue && captureOption.Value.ignoreComponentTypes.Contains(comp.GetType())) continue;

                SerializedObject so = new SerializedObject(comp);
                SerializedProperty prop = so.GetIterator();

                // enterChildren을 true로 두면 모든 계층을 하나씩 다 방문함
                while (prop.NextVisible(true))
                {
                    if(captureOption.HasValue && captureOption.Value.ignorePropertyPaths.Contains(prop.propertyPath)) continue;

                    // 자식이 없는 '말단(Leaf)' 노드이면서 지원하는 타입일 때만 기록
                    // Color 전체는 hasChildren이 true라 무시되고, 
                    // 그 안의 r, g, b, a는 hasChildren이 false라 기록됨!
                    if (!prop.hasChildren && IsSupportedType(prop))
                    {
                        AddPoseToList(poseDict, comp, prop);
                        continue;
                    }
                    if(prop.propertyType == SerializedPropertyType.Color){
                        AddPoseToList(poseDict, comp, prop.FindPropertyRelative("r"));
                        AddPoseToList(poseDict, comp, prop.FindPropertyRelative("g"));
                        AddPoseToList(poseDict, comp, prop.FindPropertyRelative("b"));
                        AddPoseToList(poseDict, comp, prop.FindPropertyRelative("a"));
                        continue;
                    }
                    if(prop.propertyType == SerializedPropertyType.ObjectReference){
                        AddPoseToList(poseDict, comp, prop);
                        continue;
                    }
                    Debug.Log($"Skipped: {prop.propertyPath} {prop.propertyType}");
                }
            }
            #endif
        }

        #if UNITY_EDITOR
        private bool IsSupportedType(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Float:
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.ObjectReference:
                    return true;
                default:
                    return false;
            }
        }

        // 헬퍼: 딕셔너리에 데이터 추가
        private void AddPoseToList(Dictionary<string, PoseData> poseDict, Component comp, SerializedProperty prop)
        {
            PoseData pose = new PoseData();
            pose.componentType = comp.GetType();
            pose.propertyPath = prop.propertyPath;
            pose.propertyType = prop.propertyType;
            pose.value = GetPropertyValue(prop);
            poseDict.Add($"{comp.GetType().FullName}|{prop.propertyPath}", pose);
            Debug.Log($"Captured: {prop.propertyPath} = {pose.value}");
        }

        private object GetPropertyValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Float: return prop.floatValue;
                case SerializedPropertyType.Integer: return prop.intValue;
                case SerializedPropertyType.Boolean: return prop.boolValue ? 1f : 0f;
                case SerializedPropertyType.Enum: return prop.enumValueIndex;
                case SerializedPropertyType.ObjectReference: return prop.objectReferenceValue;
                default: return 0f;
            }
        }
        #endif
        #endregion

        #region Create Animation Clip
        /// <summary>
        /// A 포즈를 애니메이션 클립으로 생성합니다.
        /// </summary>
        /// <param name="clipName">클립 이름</param>
        /// <param name="assetPath">클립 저장 경로</param>
        /// <returns>생성된 애니메이션 클립</returns>
        public AnimationClip CreateAnimationClipA(string clipName, string assetPath = "Assets/CapturedAnimations/")
        {
            return CreateAnimationClip(poseA, clipName, assetPath);
        }

        /// <summary>
        /// B 포즈를 애니메이션 클립으로 생성합니다.
        /// </summary>
        /// <param name="clipName">클립 이름</param>
        /// <param name="assetPath">클립 저장 경로</param>
        /// <returns>생성된 애니메이션 클립</returns>
        public AnimationClip CreateAnimationClipB(string clipName, string assetPath = "Assets/CapturedAnimations/")
        {
            return CreateAnimationClip(poseB, clipName, assetPath);
        }

        /// <summary>
        /// A->B에서 달라진 필드만만 애니메이션 클립으로 생성합니다.
        /// </summary>
        /// <param name="clipName">클립 이름</param>
        /// <param name="assetPath">클립 저장 경로</param>
        /// <returns>생성된 애니메이션 클립</returns>
        public AnimationClip CreateAnimationClipAB(string clipName, string assetPath = "Assets/CapturedAnimations/")
        {
            Dictionary<string, PoseData> poseDict = new Dictionary<string, PoseData>();
            foreach(var poseKey in poseB.Keys){
                if(poseA.ContainsKey(poseKey) && !IsPoseEquals(poseA[poseKey], poseB[poseKey])){
                    poseDict.Add(poseKey, poseB[poseKey]);
                }
            }
            return CreateAnimationClip(poseDict, clipName, assetPath);
        }

        /// <summary>
        /// B->A에서 달라진 필드만만 애니메이션 클립으로 생성합니다.
        /// </summary>
        /// <param name="clipName">클립 이름</param>
        /// <param name="assetPath">클립 저장 경로</param>
        /// <returns>생성된 애니메이션 클립</returns>
        public AnimationClip CreateAnimationClipBA(string clipName, string assetPath = "Assets/CapturedAnimations/")
        {
            Dictionary<string, PoseData> poseDict = new Dictionary<string, PoseData>();
            foreach(var poseKey in poseA.Keys){
                if(poseB.ContainsKey(poseKey) && !IsPoseEquals(poseA[poseKey], poseB[poseKey])){
                    poseDict.Add(poseKey, poseA[poseKey]);
                }
            }
            return CreateAnimationClip(poseDict, clipName, assetPath);
        }

        private bool IsPoseEquals(PoseData poseA, PoseData poseB)
        {
            #if UNITY_EDITOR
            if(poseA.propertyType != poseB.propertyType) return false;
            switch(poseA.propertyType){
                case SerializedPropertyType.Float:
                    return Mathf.Approximately((float)poseA.value, (float)poseB.value);
                case SerializedPropertyType.Integer:
                    return (int)poseA.value == (int)poseB.value;
                case SerializedPropertyType.Boolean:
                    return (bool)poseA.value == (bool)poseB.value;
                case SerializedPropertyType.Enum:
                    return (int)poseA.value == (int)poseB.value;
                case SerializedPropertyType.ObjectReference:
                    return poseA.value == poseB.value;
            }
            #endif
            return false;
        }

        private AnimationClip CreateAnimationClip(Dictionary<string, PoseData> poseDict, string clipName, string assetPath = "Assets/CapturedAnimations/")
        {
            #if UNITY_EDITOR
            AnimationClip clip = new AnimationClip();
            int curvesAdded = 0;

            foreach (var poseKey in poseDict.Keys)
            {
                var pose = poseDict[poseKey];
                AnimationCurve curve = null;
                switch (pose.propertyType)
                {
                    case SerializedPropertyType.Float:
                    case SerializedPropertyType.Boolean:
                        float valFloat = (float)pose.value;
                        curve = AnimationCurve.Linear(0, valFloat, 1f, valFloat);
                        clip.SetCurve("", pose.componentType, pose.propertyPath, curve);
                        break;
                    case SerializedPropertyType.Enum:
                    case SerializedPropertyType.Integer:
                            int valInt = (int)pose.value;
                            Debug.Log($"valInt {pose.propertyPath}: {valInt} {BitConverter.ToSingle(BitConverter.GetBytes(valInt), 0)}");
                            curve = AnimationCurve.Constant(0, 1f, BitConverter.ToSingle(BitConverter.GetBytes(valInt), 0));
                            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.DiscreteCurve("", pose.componentType, pose.propertyPath), curve);
                        break;
                    case SerializedPropertyType.ObjectReference:
                        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[2];
                        keyframes[0] = new ObjectReferenceKeyframe();
                        keyframes[0].time = 0;
                        keyframes[0].value = (UnityEngine.Object)pose.value;
                        keyframes[1] = new ObjectReferenceKeyframe();
                        keyframes[1].time = 1f;
                        keyframes[1].value = (UnityEngine.Object)pose.value;
                        AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.DiscreteCurve("", pose.componentType, pose.propertyPath), keyframes);
                        break;
                    default:
                        continue;
                }
                curvesAdded++;
            }

            if (curvesAdded > 0)
            {
                CreateSubDirectoryRecursive(assetPath);
                AssetDatabase.CreateAsset(clip, $"{assetPath}{clipName}.anim");
                AssetDatabase.SaveAssets();
                Debug.Log($"{curvesAdded}개의 필드 변화를 포함한 클립 생성 성공!");
                return clip;
            }
            Debug.Log("클립 생성 실패! 변경된 필드가 없습니다.");
            return null;
            #else
            Debug.Log("AnimationSnapshot is not supported in runtime.");
            return null;
            #endif
        }

        private void CreateSubDirectoryRecursive(string assetPath){
            #if UNITY_EDITOR
            var folders = assetPath.Split('/');
            var currentPath = "";
            foreach(var folder in folders){
                if(string.IsNullOrEmpty(folder)) continue;
                if(string.IsNullOrEmpty(currentPath)){
                    currentPath = folder;
                    continue;
                }
                if(!AssetDatabase.IsValidFolder($"{currentPath}/{folder}")){
                    AssetDatabase.CreateFolder(currentPath, folder);
                }
                currentPath = $"{currentPath}/{folder}";
            }
            #endif
        }
        #endregion
    }
}