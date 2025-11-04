using UnityEngine;
using System.Collections.Generic;


namespace LaserBeams
{
    public enum BeamType
    {
        Straight,
        Curved,
        ZigZag,
        Wave,
        SineWave,
        Random,
        Bezier,
        Bezier2,
        Bezier3,
        Bezier4,
        Bezier5,
        Bezier6
    }

    public class BeamPositionsGenerator
    {
        public List<Vector3> GenerateBeam(Vector3 start, Vector3 end, BeamType type, float ratio, int segments = 10)
        {
            List<Vector3> points = new List<Vector3>();

            switch (type)
            {
                case BeamType.Straight:
                    points = GenerateStraightBeam(start, end, segments);
                    break;
                case BeamType.Curved:
                    points = GenerateCurvedBeam(start, end, segments);
                    break;
                case BeamType.ZigZag:
                    points = GenerateZigZagBeam(start, end, segments);
                    break;
                case BeamType.Wave:
                    points = GenerateWaveBeam(start, end, segments);
                    break;
                case BeamType.Random:
                    points = GenerateRandomBeam(start, end, segments);
                    break;
                case BeamType.Bezier:
                    points = GenerateBezierBeam(start, end, segments);
                    break;
                case BeamType.Bezier2:
                    points = GenerateBezier2Beam(start, end, segments);
                    break;
                case BeamType.Bezier3:
                    points = GenerateBezier3Beam(start, end, segments);
                    break;
                case BeamType.Bezier4:
                    points = GenerateBezier4Beam(start, end, segments);
                    break;
                case BeamType.Bezier5:
                    points = GenerateBezier5Beam(start, end, segments);
                    break;
                case BeamType.Bezier6:
                    points = GenerateBezier6Beam(start, end, segments);
                    break;
                case BeamType.SineWave:
                    points = GenerateSineWaveBeam(start, end, segments);
                    break;
            }

            return GetSubVectors(points, ratio);
        }

        private List<Vector3> GenerateStraightBeam(Vector3 start, Vector3 end, int segments)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                points.Add(Vector3.Lerp(start, end, t));
            }
            return points;
        }

        private List<Vector3> GenerateCurvedBeam(Vector3 start, Vector3 end, int segments)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 controlPoint = (start + end) / 2 + Vector3.up * Vector3.Distance(start, end) / 2;

            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector3 point = Mathf.Pow(1 - t, 2) * start + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * end;
                points.Add(point);
            }
            return points;
        }

        private List<Vector3> GenerateZigZagBeam(Vector3 start, Vector3 end, int segments)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 direction = (end - start).normalized;
            float length = Vector3.Distance(start, end);
            float step = length / segments;
            float amplitude = step / 4;

            for (int i = 0; i <= segments; i++)
            {
                Vector3 point = start + direction * step * i;
                if (i % 2 == 0)
                {
                    point += Vector3.up * amplitude;
                }
                else
                {
                    point -= Vector3.up * amplitude;
                }
                points.Add(point);
            }

            // 修正起点
            points[0] = start;
            // 修正终点
            points[segments] = end;

            return points;
        }


        private List<Vector3> GenerateWaveBeam(Vector3 start, Vector3 end, int segments)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 direction = (end - start).normalized;
            float length = Vector3.Distance(start, end);
            float amplitude = length / 50;
            float waveLength = length / segments;
            segments *= 2; 
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector3 point = Vector3.Lerp(start, end, t);
                point += Vector3.up * Mathf.Sin(t * Mathf.PI * 2 / waveLength) * amplitude;
                points.Add(point);
            }
            return points;
        }

        private List<Vector3> GenerateRandomBeam(Vector3 start, Vector3 end, int segments)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(start);
            for (int i = 1; i < segments; i++)
            {
                float t = (float)i / segments;
                Vector3 point = Vector3.Lerp(start, end, t);
                point += new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ) * (Vector3.Distance(start, end) / segments / 3);
                points.Add(point);
            }
            points.Add(end);
            return points;
        }
        private List<Vector3> GenerateBezierBeam(Vector3 start, Vector3 end, int segments)
        {
            Bezier bezier = new Bezier(
                start,
               GetFixedAngleVector(end - start, 45) * (Vector3.Distance(start, end) / 2),
               GetFixedAngleVector(start - end, 45) * (Vector3.Distance(start, end) / 2),
                end
            );

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                points.Add(bezier.GetPointAtTime(t));
            }
            return points;
        }

        private List<Vector3> GenerateBezier2Beam(Vector3 start, Vector3 end, int segments)
        {
            Bezier bezier = new Bezier(
                start,

               GetFixedAngleVector(end - start, 30) * (Vector3.Distance(start, end) / 3),
               GetFixedAngleVector(start - end, 30) * (Vector3.Distance(start, end) / 3),
                end
            );

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                points.Add(bezier.GetPointAtTime(t));
            }
            return points;
        }

        private List<Vector3> GenerateBezier3Beam(Vector3 start, Vector3 end, int segments)
        {
            Bezier bezier = new Bezier(
                start,
               GetFixedAngleVector(end - start, 10) * (Vector3.Distance(start, end) / 3),
               GetFixedAngleVector(start - end, 20) * (Vector3.Distance(start, end)),
                end
            );

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                points.Add(bezier.GetPointAtTime(t));
            }
            return points;
        }

        private List<Vector3> GenerateBezier4Beam(Vector3 start, Vector3 end, int segments)
        {
            Bezier bezier = new Bezier(
                start,
                GetFixedAngleVector(end - start, 30) * (Vector3.Distance(start, end) / 6),
                GetFixedAngleVector(start - end, 60) * (Vector3.Distance(start, end) / 3),
                end
            );

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                points.Add(bezier.GetPointAtTime(t));
            }
            return points;
        }

        private List<Vector3> GenerateBezier5Beam(Vector3 start, Vector3 end, int segments)
        {
            Bezier bezier = new Bezier(
               start,
               GetFixedAngleVector(end - start, 10) * (Vector3.Distance(start, end) / 2),
               GetFixedAngleVector(start - end, 20) * (Vector3.Distance(start, end) / 3),
               end
           );

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                points.Add(bezier.GetPointAtTime(t));
            }
            return points;
        }

        private List<Vector3> GenerateBezier6Beam(Vector3 start, Vector3 end, int segments)
        {
            Bezier bezier = new Bezier(
                start,
                GetFixedAngleVector(end - start, 30) * (Vector3.Distance(start, end) / 3),
                GetFixedAngleVector(start - end, 10) * (Vector3.Distance(start, end) / 5),
                end
            );

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                points.Add(bezier.GetPointAtTime(t));
            }
            return points;
        }



        private List<Vector3> GenerateSineWaveBeam(Vector3 start, Vector3 end, int segments)
        {
            List<Vector3> points = new List<Vector3>();
            Vector3 direction = (end - start).normalized;
            float length = Vector3.Distance(start, end);
            float amplitude = length / 50; // 调整振幅
            float waveLength = length;
            segments *= 2;
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector3 point = Vector3.Lerp(start, end, t);
                point += Vector3.up * Mathf.Sin(t * Mathf.PI * 5) * amplitude;
                points.Add(point);
            }
            points.Add(end);
            return points;
        }

        public List<Vector3> GetSubVectors(List<Vector3> v3s, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            float totalDistance = GetLength(v3s);

            float currentDistance = 0;
            float targetDistance = totalDistance * ratio;

            int currentIndex = 0;
            List<Vector3> v3s1 = new List<Vector3>();

            int c = v3s.Count;
            for (int i = 0; currentDistance < targetDistance && i < c - 1; i++)
            {
                currentIndex = i;
                v3s1.Add(v3s[i]);
                currentDistance += Vector3.Distance(v3s[i], v3s[i + 1]);
            }

            float lastDistance = targetDistance - (currentDistance - Vector3.Distance(v3s[currentIndex], v3s[currentIndex + 1]));

            if (lastDistance > 0)
            {
                v3s1.Add(v3s[currentIndex] + (v3s[currentIndex + 1] - v3s[currentIndex]).normalized * lastDistance);
            }

            return v3s1;
        }

        public float GetLength(List<Vector3> v3s)
        {
            float length = 0;
            for (int i = 0; i < v3s.Count - 1; i++)
            {
                length += Vector3.Distance(v3s[i], v3s[i + 1]);
            }
            return length;
        }
        Vector3 GetFixedAngleVector(Vector3 vector, float angleInDegrees)
        {
            // 将角度转换为弧度
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // 选择一个任意的非平行向量
            Vector3 arbitraryVector = new Vector3(1, 0, 0);
            if (Vector3.Dot(vector.normalized, arbitraryVector.normalized) > 0.99f)
            {
                arbitraryVector = new Vector3(0, 1, 0);
            }

            // 计算向量的叉积以获得一个垂直向量
            Vector3 orthogonalVector = Vector3.Cross(vector, arbitraryVector).normalized;

            // 在垂直方向上旋转这个向量
            Vector3 rotatedOrthogonalVector = Quaternion.AngleAxis(angleInDegrees, vector) * orthogonalVector;

            // 确保最终的向量不会向下
            Vector3 result = vector.normalized * Mathf.Cos(angleInRadians) + rotatedOrthogonalVector * Mathf.Sin(angleInRadians);
            if (result.y < 0)
            {
                result = vector.normalized * Mathf.Cos(angleInRadians) - rotatedOrthogonalVector * Mathf.Sin(angleInRadians);
            }
            result.Normalize();  // 规范化向量

            return result;
        }
    }
}