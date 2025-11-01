using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PathToTunnel
{
    public class BaseFunction : MonoBehaviour
    {
        public static List<Vector3> GetSubVectors(List<Vector3> v3s, float ratio)
        {
            ratio = ratio > 1 ? 1 : ratio;
            float totalDistance = GetLength(v3s);

            float currentDistance = 0;
            float targetDistance = totalDistance * ratio;

            int currentIndex = 0;
            List<Vector3> v3s1 = new List<Vector3>();

            int c = v3s.Count;
            for (int i = 0; currentDistance < targetDistance  && i < c - 1; i++)
            {
                currentIndex = i;
                v3s1.Add(v3s[i]);
                currentDistance += Vector3.Distance(v3s[i], v3s[i + 1]);
            }

            float lastDistance = targetDistance - (currentDistance - Vector3.Distance(v3s[currentIndex], v3s[currentIndex+1]));

            if (lastDistance > 0)
            {
                v3s1.Add(v3s[currentIndex] +(v3s[currentIndex + 1] - v3s[currentIndex]).normalized * lastDistance);
            }

            return v3s1;
        }
        public static List<Vector3> GetTrailVectorsInvertion(List<Vector3> v3s, float ratio, float trailingLength)

        {
            List<Vector3> v3s1 = GetTrailVectors(v3s, ratio, trailingLength);
            List<Vector3> v3s2 = new List<Vector3>();
            int c = v3s1.Count;
            for (int i = c - 1; i > -1; i--)
            {
                v3s2.Add(v3s1[i]);
            }
            return v3s2;
        }

        public static float GetLength(List<Vector3> v3s)
        {
            float totalDistance = 0;
            int c = v3s.Count;
            for (int i = 0; i < c - 1; i++)
            {
                totalDistance += Vector3.Distance(v3s[i], v3s[i + 1]);
            }
            return totalDistance;
        }
        public static List<Vector3> GetTrailVectors(List<Vector3> v3s, float ratio, float trailingLength)
        {
            if (ratio < 0 || ratio > 1) return new List<Vector3>() ;
            int trailPointsCount = 8;

            float totalLength = GetLength(v3s);
            float perRatio = trailingLength / totalLength / trailPointsCount;
            float perLenght = totalLength * perRatio;


            List<Vector3> v3s1 = new List<Vector3>();
            for (int i = 0; i < trailPointsCount; i++)
            {
                float r = ratio - i * perRatio;
                if (r >= 0)
                {
                    float totalDistance = GetLength(v3s);

                    float currentDistance = 0;
                    float targetDistance = totalDistance * r;

                    int currentIndex = 0;

                    int c = v3s.Count;
                    for (int j = 0; currentDistance <= targetDistance && j < c - 1; j++)
                    {
                        currentIndex = j;
                        currentDistance += Vector3.Distance(v3s[j], v3s[j + 1]);
                    }

                    float lastDistance = targetDistance - (currentDistance - Vector3.Distance(v3s[currentIndex], v3s[currentIndex + 1]));

                    if (lastDistance > 0)
                    {
                        v3s1.Add(v3s[currentIndex] + (v3s[currentIndex + 1] - v3s[currentIndex]).normalized * lastDistance);
                        if (lastDistance < perLenght)
                        {
                            v3s1.Add(v3s[currentIndex]);
                        }
                    }
                    else
                    {
                        v3s1.Add(v3s[currentIndex]);
                    }
                }                  
                else 
                    v3s1.Add(v3s[0] + ( v3s[0] - v3s[1]).normalized * Mathf.Abs(r * perLenght));
            }
            return v3s1;
        }

        public static int GetTrailCounts(List<Vector3> v3s, int subIndex, float trailingLength)
        {


            float distance = 0;
            int count = 1;
            if (subIndex >= v3s.Count) return 0;
            for (int i = subIndex; i > 0; i--)
            {
                count++;
                distance += Vector3.Distance(v3s[i - 1], v3s[i]);
                if (distance > trailingLength)
                    return count;
            }
            return count;
        }



        public static Vector3 GetPosWithRatio(List<Vector3> v3s, float ratio)
        {
            float totalDistance = GetLength(v3s);

            float currentDistance = 0;
            float targetDistance = totalDistance * ratio;

            int currentIndex = 0;
           // List<Vector3> v3s1 = new List<Vector3>();

            int c = v3s.Count;
            for (int i = 0; currentDistance <= targetDistance && i < c - 1; i++)
            {
                currentIndex = i;
             //   v3s1.Add(v3s[i]);
                currentDistance += Vector3.Distance(v3s[i], v3s[i + 1]);
            }

            float lastDistance = targetDistance - (currentDistance - Vector3.Distance(v3s[currentIndex], v3s[currentIndex + 1]));

            if (lastDistance > 0)
            {
                 return v3s[currentIndex] + (v3s[currentIndex + 1] - v3s[currentIndex]).normalized * lastDistance;
            }
            return v3s[currentIndex];
        }
        public static Vector3 GetDirection(List<Vector3> v3s, float ratio)
        {
            float totalDistance = GetLength(v3s);

            float currentDistance = 0;
            float targetDistance = totalDistance * ratio;

            int currentIndex = 0;
            int c = v3s.Count;
            for (int i = 0; currentDistance < targetDistance && i < c - 1; i++)
            {
                currentIndex = i;
                currentDistance += Vector3.Distance(v3s[i], v3s[i + 1]);
            }
            return  (v3s[currentIndex + 1]- v3s[currentIndex]).normalized;

        }
    }
}