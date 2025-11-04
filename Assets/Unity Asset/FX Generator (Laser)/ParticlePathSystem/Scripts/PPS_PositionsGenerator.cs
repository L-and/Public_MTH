using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // 引入UnityEditor命名空间来使用Handles
#endif

namespace ParticlePathSystem
{
    public class PPS_PositionsGenerator : PPS_PathPositions
    {
        public enum DrawType
        {
              tornado, Parabola, helix,lightning
        }

        public DrawType type = DrawType.tornado;

        void OnEnable()
        {
            GetPositions();
        }

        public override List<Vector3> GetPositions()
        {
            positions.Clear(); // 确保 positions 列表是空的
            if (type == DrawType.helix)
            {
                float angle = 5;
                for (float i = 0; i < 20; i += 0.3f)
                {
                    float index = i * 10 * 2;
                    float x = Mathf.Sin(angle * index * Mathf.Deg2Rad) * 3;
                    float y = Mathf.Cos(angle * index * Mathf.Deg2Rad) * 3;
                    positions.Add(new Vector3(x, y, i));
                }
            }
            else if (type == DrawType.tornado)
            {
                float height = 20;
                float radius = 5;
                int segments = 100;
                float rotations = 8; // 增加旋转圈数
                for (int i = 0; i <= segments; i++)
                {
                    float t = (float)i / segments;
                    float angle = t * Mathf.PI * 2 * rotations; // 旋转圈数
                    float x = Mathf.Cos(angle) * radius * t; // 半径由小变大
                    float y = Mathf.Sin(angle) * radius * t; // 半径由小变大
                    float z = t * height;
                    positions.Add(new Vector3(x, y, z));
                }
            }
            else if (type == DrawType.Parabola)
            {
                float height = 6;
                float width = 20;
                int segments = 100;
                for (int i = 0; i <= segments; i++)
                {
                    float t = (float)i / segments;
                    float y = -4 * height * (t - 0.5f) * (t - 0.5f) + height;
                    float z = t * width;
                    positions.Add(new Vector3(0, y, z));
                }
            }

            else if (type == DrawType.lightning)
            {
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(3, 0, 10));
                positions.Add(new Vector3(-3, 0, 11));
                positions.Add(new Vector3(0, 0, 20));
            }


            return positions;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            DrawLine();
        }

        void DrawLine()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < positions.Count - 1; i++)
            {
                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }

            for (int i = 0; i < positions.Count; i++)
            {
                Gizmos.DrawSphere(positions[i], 0.1f);
                Handles.Label(positions[i], i.ToString());
            }
        }
#endif
    }
}
