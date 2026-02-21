using UnityEngine;

namespace HUtil.Runtime
{
    /// <summary>
    /// Vector~류 확장 메서드
    /// </summary>
    public static class VectorExtension
    {
        #region With

        #region Vector3
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
        #endregion

        #region Vector3Int
        /// <summary>
        /// X요소가 대체된 새로운 Vector3Int을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3Int</param>
        /// <param name="x">대체될 X요소</param>
        /// <returns>새로운 Vector3Int</returns>
        public static Vector3Int WithX(this Vector3Int v, int x)
        {
            return new Vector3Int(x, v.y, v.z);
        }

        /// <summary>
        /// Y요소가 대체된 새로운 Vector3Int을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3Int</param>
        /// <param name="y">대체될 Y요소</param>
        /// <returns>새로운 Vector3Int</returns>
        public static Vector3Int WithY(this Vector3Int v, int y)
        {
            return new Vector3Int(v.x, y, v.z);
        }

        /// <summary>
        /// Z요소가 대체된 새로운 Vector3Int을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3Int</param>
        /// <param name="z">대체될 Z요소</param>
        /// <returns>새로운 Vector3Int</returns>
        public static Vector3Int WithZ(this Vector3Int v, int z)
        {
            return new Vector3Int(v.x, v.y, z);
        }

        /// <summary>
        /// X와 Y요소가 대체된 새로운 Vector3Int을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3Int</param>
        /// <param name="x">대체될 X요소</param>
        /// <param name="y">대체될 Y요소</param>
        /// <returns>새로운 Vector3Int</returns>
        public static Vector3Int WithXY(this Vector3Int v, int x, int y)
        {
            return new Vector3Int(x, y, v.z);
        }

        /// <summary>
        /// X와 Z요소가 대체된 새로운 Vector3Int을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3Int</param>
        /// <param name="x">대체될 X요소</param>
        /// <param name="z">대체될 Z요소</param>
        /// <returns>새로운 Vector3Int</returns>
        public static Vector3Int WithXZ(this Vector3Int v, int x, int z)
        {
            return new Vector3Int(x, v.y, z);
        }

        /// <summary>
        /// Y와 Z요소가 대체된 새로운 Vector3Int을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector3Int</param>
        /// <param name="y">대체될 Y요소</param>
        /// <param name="z">대체될 Z요소</param>
        /// <returns>새로운 Vector3Int</returns>
        public static Vector3Int WithYZ(this Vector3Int v, int y, int z)
        {
            return new Vector3Int(v.x, y, z);
        }
        #endregion

        #region Vector2
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
        #endregion

        #region Vector2Int
        /// <summary>
        /// X요소가 대체된 새로운 Vector2Int을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector2Int</param>
        /// <param name="x">대체될 X요소</param>
        /// <returns>새로운 Vector2Int</returns>
        public static Vector2Int WithX(this Vector2Int v, int x){
            return new Vector2Int(x, v.y);
        }

        /// <summary>
        /// Y요소가 대체된 새로운 Vector2Int을 반환합니다.
        /// </summary>
        /// <param name="v">기존 Vector2Int</param>
        /// <param name="y">대체될 Y요소</param>
        /// <returns>새로운 Vector2Int</returns>
        public static Vector2Int WithY(this Vector2Int v, int y){
            return new Vector2Int(v.x, y);
        }
        #endregion

        #endregion


        #region Scremble

        #region Vector3
        public static Vector3 XZY(this Vector3 v){
            return new Vector3(v.z, v.y, v.x);
        }

        public static Vector3 YXZ(this Vector3 v){
            return new Vector3(v.y, v.x, v.z);
        }

        public static Vector3 YZX(this Vector3 v){
            return new Vector3(v.y, v.z, v.x);
        }

        public static Vector3 ZYX(this Vector3 v){
            return new Vector3(v.z, v.y, v.x);
        }

        public static Vector3 ZXY(this Vector3 v){
            return new Vector3(v.z, v.x, v.y);
        }

        public static Vector2 XY(this Vector3 v){
            return new Vector2(v.x, v.y);
        }

        public static Vector2 YZ(this Vector3 v){
            return new Vector2(v.y, v.z);
        }

        public static Vector2 ZX(this Vector3 v){
            return new Vector2(v.z, v.x);
        }
        #endregion

        #region Vector3Int
        public static Vector3Int XZY(this Vector3Int v){
            return new Vector3Int(v.z, v.y, v.x);
        }

        public static Vector3Int YXZ(this Vector3Int v){
            return new Vector3Int(v.y, v.x, v.z);
        }

        public static Vector3Int YZX(this Vector3Int v){
            return new Vector3Int(v.y, v.z, v.x);
        }

        public static Vector3Int ZYX(this Vector3Int v){
            return new Vector3Int(v.z, v.y, v.x);
        }

        public static Vector3Int ZXY(this Vector3Int v){
            return new Vector3Int(v.z, v.x, v.y);
        }

        public static Vector2Int XY(this Vector3Int v){
            return new Vector2Int(v.x, v.y);
        }

        public static Vector2Int YZ(this Vector3Int v){
            return new Vector2Int(v.y, v.z);
        }

        public static Vector2Int ZX(this Vector3Int v){
            return new Vector2Int(v.z, v.x);
        }
        #endregion

        public static Vector2 YX(this Vector2 v){
            return new Vector2(v.y, v.x);
        }

        public static Vector2Int YX(this Vector2Int v){
            return new Vector2Int(v.y, v.x);
        }

        #endregion
    }
}
