using UnityEngine;

namespace HUtil.Runtime
{
    public static class VectorUtil
    {
        /// <summary>
        /// X요소가 대체된 새로운 Vector3을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3</param>
        /// <param name="x">대체될 X요소</param>
        /// <returns>새로운 Vector3</returns>
        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        /// <summary>
        /// Y요소가 대체된 새로운 Vector3을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3</param>
        /// <param name="y">대체될 Y요소</param>
        /// <returns>새로운 Vector3</returns>
        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        /// <summary>
        /// Z요소가 대체된 새로운 Vector3을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3</param>
        /// <param name="z">대체될 Z요소</param>
        /// <returns>새로운 Vector3</returns>
        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        /// <summary>
        /// X와 Y요소가 대체된 새로운 Vector3을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3</param>
        /// <param name="x">대체될 X요소</param>
        /// <param name="y">대체될 Y요소</param>
        /// <returns>새로운 Vector3</returns>
        public static Vector3 WithXY(this Vector3 v, float x, float y)
        {
            return new Vector3(x, y, v.z);
        }

        /// <summary>
        /// X와 Z요소가 대체된 새로운 Vector3을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3</param>
        /// <param name="x">대체될 X요소</param>
        /// <param name="z">대체될 Z요소</param>
        /// <returns>새로운 Vector3</returns>
        public static Vector3 WithXZ(this Vector3 v, float x, float z)
        {
            return new Vector3(x, v.y, z);
        }

        /// <summary>
        /// Y와 Z요소가 대체된 새로운 Vector3을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3</param>
        /// <param name="y">대체될 Y요소</param>
        /// <param name="z">대체될 Z요소</param>
        /// <returns>새로운 Vector3</returns>
        public static Vector3 WithYZ(this Vector3 v, float y, float z)
        {
            return new Vector3(v.x, y, z);
        }



        /// <summary>
        /// X요소가 대체된 새로운 Vector2을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector2</param>
        /// <param name="x">대체될 X요소</param>
        /// <returns>새로운 Vector2</returns>
        public static Vector2 WithX(this Vector2 v, float x){
            return new Vector2(x, v.y);
        }

        /// <summary>
        /// Y요소가 대체된 새로운 Vector2을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector2</param>
        /// <param name="y">대체될 Y요소</param>
        /// <returns>새로운 Vector2</returns>
        public static Vector2 WithY(this Vector2 v, float y){
            return new Vector2(v.x, y);
        }
    }
}
