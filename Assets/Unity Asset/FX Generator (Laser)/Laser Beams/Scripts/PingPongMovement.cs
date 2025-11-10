using UnityEngine;


namespace LaserBeams
{
    public class PingPongMovement : MonoBehaviour
    {
        public Vector3 pointA; // 第一个位置
        public Vector3 pointB; // 第二个位置
        public float speed = 1.0f; // 移动速度

        void Update()
        {
            // 使用PingPong函数来计算t值，它在0和1之间来回循环
            float t = Mathf.PingPong(Time.time * speed / Vector3.Distance(pointA, pointB), 1.0f);

            // 在两个点之间插值
            transform.position = Vector3.Lerp(pointA, pointB, t);
        }
    }
}