using System.Collections.Generic;
using UnityEngine;

namespace ParticlePathSystem
{
    public class PPSPath
    {
        public List<Vector3> points;
        public float length;
        private List<Vector3> directions;
        private List<float> cumulativeLengths;

        public PPSPath(List<Vector3> points)
        {
            this.points = points;
            CalculatePathData();
        }
        private void CalculatePathData()
        {
            length = 0;
            directions = new List<Vector3>();
            cumulativeLengths = new List<float>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                float segmentLength = Vector3.Distance(points[i], points[i + 1]);
                length += segmentLength;
                Vector3 direction = (points[i + 1] - points[i]).normalized;
                directions.Add(direction);
                cumulativeLengths.Add(length);
            }
        }


        public Vector3 GetDirectionAtDistance(float distance)
        {
            if (distance <= 0) return directions[0];
            if (distance >= length) return directions[directions.Count - 1];

            for (int i = 0; i < cumulativeLengths.Count; i++)
            {
                if (distance <= cumulativeLengths[i])
                {
                    return directions[i];
                }
            }

            return directions[directions.Count - 1];
        }
    }
}
