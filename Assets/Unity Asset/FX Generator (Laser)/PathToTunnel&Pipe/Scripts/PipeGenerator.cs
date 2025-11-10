using System.Collections.Generic;
using UnityEngine;
namespace PathToTunnel
{
    public class PipeGenerator : MonoBehaviour
    {
        public static Mesh CreatePipeMesh(Vector3[] points, float radius, int sides ,bool invertNormals = false)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Pipe";

            if (points.Length < 2 || sides < 3)
            {
                Debug.LogError("点数或边数无效。");
                return mesh;
            }

            Vector3[] vertices = new Vector3[points.Length * (sides + 1)];
            int[] triangles = new int[(points.Length - 1) * sides * 6];
            Vector2[] uvs = new Vector2[vertices.Length];
            Vector3[] normals = new Vector3[vertices.Length];

            float[] segmentLengths = new float[points.Length - 1];
            float totalLength = 0;
            for (int i = 0; i < points.Length - 1; i++)
            {
                segmentLengths[i] = Vector3.Distance(points[i], points[i + 1]);
                totalLength += segmentLengths[i];
            }
            Vector3 pVector = Vector3.one.normalized;
            float accumulatedLength = 0;
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 direction = Vector3.forward;
               // int r = 1;
                if (i == 0)
                {
                    direction = (points[i + 1] - points[i]).normalized;
                }
                else if (i == points.Length - 1)
                {
                    direction = (points[i] - points[i - 1]).normalized;
                }
                else
                {
                    direction = ((points[i + 1] - points[i]).normalized + (points[i] - points[i - 1]).normalized).normalized;
                }

                Vector3 perpVector = ChoosePerpendicularVector(direction);
                if (perpVector == Vector3.zero)
                    perpVector = pVector;
                else
                    pVector = perpVector;
                Vector3 normal = Vector3.Cross(direction, perpVector).normalized;
                Vector3 binormal = Vector3.Cross(direction, normal).normalized;



                //Debug.DrawRay(points[i] + Vector3.one * .1f, perpVector, Color.blue);
                //Debug.DrawRay(points[i], normal, Color.red);
                //Debug.DrawRay(points[i], binormal, Color.green);
                //Debug.DrawRay(points[i], direction, Color.yellow);




                for (int j = 0; j <= sides; j++)
                {
                    float angle = j * 2 * Mathf.PI / sides;
                    float asinHalfAngle = 1;
                    if (i != 0 && i != points.Length - 1)
                    {
                        Vector3 dirForward = points[i + 1] - points[i];
                        Vector3 dirBackward = points[i - 1] - points[i];
                        float angleDegrees = Vector3.Angle(dirForward, dirBackward);
                        float halfAngleRadians = (angleDegrees / 2.0f) * Mathf.Deg2Rad;
                        float sinHalfAngle = Mathf.Sin(halfAngleRadians);
                        asinHalfAngle = sinHalfAngle == 0 ? 1 : 1 / sinHalfAngle; 
                    }
                    Vector3 radial = Mathf.Cos(angle) * normal * radius * asinHalfAngle + Mathf.Sin(angle) * binormal * radius;
                    int vertexIndex = i * (sides + 1) + j;
                    vertices[vertexIndex] = points[i] + radial;
                    uvs[vertexIndex] = new Vector2(j / (float)sides, accumulatedLength / totalLength);
                    normals[vertexIndex] = invertNormals ? -radial.normalized : radial.normalized;
                }

                if (i < points.Length - 1) accumulatedLength += segmentLengths[i];
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                for (int j = 0; j < sides; j++)
                {
                    int current = i * (sides + 1) + j;
                    int next = current + 1;
                    int currentNextLevel = current + (sides + 1);
                    if (invertNormals)
                    {
                        triangles[i * sides * 6 + j * 6 + 0] = currentNextLevel;
                        triangles[i * sides * 6 + j * 6 + 1] = next;
                        triangles[i * sides * 6 + j * 6 + 2] = current;
                        triangles[i * sides * 6 + j * 6 + 3] = next + (sides + 1);
                        triangles[i * sides * 6 + j * 6 + 4] = next;
                        triangles[i * sides * 6 + j * 6 + 5] = currentNextLevel;
                    }
                    else
                    {
                        triangles[i * sides * 6 + j * 6 + 0] = current;
                        triangles[i * sides * 6 + j * 6 + 1] = next;
                        triangles[i * sides * 6 + j * 6 + 2] = currentNextLevel;
                        triangles[i * sides * 6 + j * 6 + 3] = currentNextLevel;
                        triangles[i * sides * 6 + j * 6 + 4] = next;
                        triangles[i * sides * 6 + j * 6 + 5] = next + (sides + 1);
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }
        public static Vector3 GetPerpendicularVector(Vector3 direction)
        {
            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && Mathf.Abs(direction.x) < Mathf.Abs(direction.z))
                return Vector3.Cross(direction, Vector3.right).normalized;
            else if (Mathf.Abs(direction.y) < Mathf.Abs(direction.z))
                return Vector3.Cross(direction, Vector3.up).normalized;
            else
                return Vector3.Cross(direction, Vector3.forward).normalized;
        }
        // 判断向量 B 相对于向量 A 是在左边还是右边
        public static bool isRightSide(Vector3 A, Vector3 B,Vector3 panelDirction)
        {
            Vector3 crossProduct = Vector3.Cross(A, B);
            float dotProduct = Vector3.Dot(crossProduct, panelDirction);

            if (dotProduct > 0)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        public static Vector3 ChoosePerpendicularVector(Vector3 direction)
        {
            // 扩展备选向量列表，包括负方向
            Vector3[] possibleNormals = new Vector3[]
            {
        Vector3.up, Vector3.down, Vector3.forward, Vector3.back, Vector3.right, Vector3.left
            };

            Vector3 bestNormal = possibleNormals[0];
            float minDot = Mathf.Abs(Vector3.Dot(direction.normalized, bestNormal));

            for (int i = 1; i < possibleNormals.Length; i++)
            {
                float dot = Mathf.Abs(Vector3.Dot(direction.normalized, possibleNormals[i]));
                if (dot < minDot)
                {
                    minDot = dot;
                    bestNormal = possibleNormals[i];
                }
            }

            // 确保选出的向量确实垂直
            if (minDot < 0.01)  // 使用一个小的阈值而不是完全为零，因为浮点精度问题
            {
                return Vector3.zero;
            }

            return bestNormal;
        }

      
    }


}