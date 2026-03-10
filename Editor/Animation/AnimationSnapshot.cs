using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace HUtil.Editor.Animation
{
    /// <summary>
    /// 필드 변경사항을 애니메이션 스냅샷으로 캡처하는 클래스
    /// </summary>
    public class AnimationSnapshot
    {
        private GameObject targetObject;
        private Dictionary<string, float> poseA = new Dictionary<string, float>();
        private Dictionary<string, float> poseB = new Dictionary<string, float>();
        private bool _isCapturedA = false;
        private bool _isCapturedB = false;

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
        public AnimationSnapshot(GameObject targetObject)
        {
            this.targetObject = targetObject;
            _isCapturedA = false;
            _isCapturedB = false;
        }
        
        #region public Methods
        /// <summary>
        /// A 포즈를 캡처합니다.
        /// </summary>
        /// <param name="poseDict">캡처된 필드 변경사항</param>
        public void CapturePoseA()
        {
            CapturePose(poseA);
            _isCapturedA = true;
        }

        /// <summary>
        /// B 포즈를 캡처합니다.
        /// </summary>
        /// <param name="poseDict">캡처된 필드 변경사항</param>
        public void CapturePoseB()
        {
            CapturePose(poseB);
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
        private void CapturePose(Dictionary<string, float> poseDict)
        {
            if (targetObject == null) return;
            poseDict.Clear();

            foreach (var comp in targetObject.GetComponents<Component>())
            {
                if (comp == null) continue;
                SerializedObject so = new SerializedObject(comp);
                SerializedProperty prop = so.GetIterator();

                while (prop.NextVisible(true))
                {
                    // 핵심: property.numericValue를 쓰지 않고, 
                    // 수치로 변환 가능한 모든 하위 필드(x, y, r, g, b 등)를 낱개로 캡처
                    if (prop.hasChildren) continue; // 부모 속성은 건너뛰고 실제 값 필드만 캡처

                    if (IsSupportedType(prop))
                    {
                        string key = $"{comp.GetType().AssemblyQualifiedName}|{prop.propertyPath}";
                        poseDict[key] = GetPropertyValueAsFloat(prop);
                    }
                }
            }
            Debug.Log($"포즈 캡처 완료! ({poseDict.Count}개 데이터)");
        }

        private bool IsSupportedType(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Float:
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.Enum:
                    return true;
                default:
                    return false;
            }
        }

        private float GetPropertyValueAsFloat(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Float: return prop.floatValue;
                case SerializedPropertyType.Integer: return prop.intValue;
                case SerializedPropertyType.Boolean: return prop.boolValue ? 1f : 0f;
                case SerializedPropertyType.Enum: return prop.enumValueIndex;
                default: return 0f;
            }
        }
        #endregion

        #region Create Animation Clip
        /// <summary>
        /// A와 B 캡쳐본을 바탕으로 변경된 사항만 애니메이션 클립으로 생성합니다.
        /// </summary>
        /// <param name="clipName">클립 이름</param>
        public void CreateAnimationClipAB(string clipName, string assetPath = "Assets/CapturedAnimations/")
        {
            if (!IsCapturedA || !IsCapturedB) return;

            AnimationClip clip = new AnimationClip();
            int curvesAdded = 0;

            foreach (var key in poseA.Keys)
            {
                if (!poseB.ContainsKey(key)) continue;

                float valA = poseA[key];
                float valB = poseB[key];

                if (!Mathf.Approximately(valA, valB))
                {
                    string[] parts = key.Split('|');
                    string typeName = parts[0];
                    string propertyPath = parts[1];

                    System.Type compType = System.Type.GetType(typeName);
                    AnimationCurve curve = AnimationCurve.Linear(0, valA, 1f, valA);

                    clip.SetCurve("", compType, propertyPath, curve);
                    curvesAdded++;
                }
            }

            if (curvesAdded > 0)
            {
                AssetDatabase.CreateAsset(clip, $"{assetPath}{clipName}.anim");
                AssetDatabase.SaveAssets();
                Debug.Log($"{curvesAdded}개의 필드 변화를 포함한 클립 생성 성공!");
            }
        }

        /// <summary>
        /// A와 B로 변화하는 애니메이션 클립을 생성합니다.
        /// </summary>
        /// <param name="clipName">클립 이름</param>
        public void CreateAnimationClipDelta(string clipName, float duration = 1f, bool isEaseInOut = true, string assetPath = "Assets/CapturedAnimations/")
        {
            if (!IsCapturedA || !IsCapturedB) return;

            AnimationClip clip = new AnimationClip();
            int curvesAdded = 0;

            foreach (var key in poseA.Keys)
            {
                if (!poseB.ContainsKey(key)) continue;

                float valA = poseA[key];
                float valB = poseB[key];

                if (!Mathf.Approximately(valA, valB))
                {
                    string[] parts = key.Split('|');
                    string typeName = parts[0];
                    string propertyPath = parts[1];

                    System.Type compType = System.Type.GetType(typeName);
                    AnimationCurve curve = null;
                    if (isEaseInOut)
                    {
                        curve = AnimationCurve.EaseInOut(0, valA, duration, valB);
                    }
                    else
                    {
                        curve = AnimationCurve.Linear(0, valA, duration, valB);
                    }

                    clip.SetCurve("", compType, propertyPath, curve);
                    curvesAdded++;
                }
            }

            if (curvesAdded > 0)
            {
                AssetDatabase.CreateAsset(clip, $"{assetPath}{clipName}.anim");
                AssetDatabase.SaveAssets();
                Debug.Log($"{curvesAdded}개의 필드 변화를 포함한 클립 생성 성공!");
            }
        }
        #endregion
    }
}