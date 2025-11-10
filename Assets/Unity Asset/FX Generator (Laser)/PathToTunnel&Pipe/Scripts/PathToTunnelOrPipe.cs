using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PathToTunnel
{
    public class PathToTunnelOrPipe : MonoBehaviour
    {
        [Tooltip("Each time a path is generated, a material  will be randomly selected from this list.")]
        public List<Material> materials = new List<Material>();
        public List<Vector3> pathPositions = new List<Vector3>();
        public Transform positionListParent;
        public int sides = 10;
        public float radius = 1;
        public GameObject pipeObject;
        int matIndex = 0,positionIndex = 0;
        private void Start()
        {
            PlayNextPositions();
        }

        public void ChangeMaterail(Material mat)
        {
            pipeObject.GetComponent<MeshRenderer>().material = mat;
        }
        public void UpdateTunnel()
        {
            pipeObject.GetComponent<MeshFilter>().mesh = PipeGenerator.CreatePipeMesh(pathPositions.ToArray(), radius, sides, true);
        }
        public void SetNextMaterial()
        {
            matIndex++;
            if (matIndex >= materials.Count) matIndex = 0;
            ChangeMaterail(materials[matIndex]);
        }

        public void PlayNextPositions()
        {
            pathPositions = positionListParent.GetChild(positionIndex).GetComponent<PathPositions>().positions; 
            UpdateTunnel();
            positionIndex++;
            if (positionIndex >= positionListParent.childCount) positionIndex = 0;
        }

        # region The methods that may be used in the path are not currently used in this case
        // 这个方法根据给定的ratio裁剪路径
        public List<Vector3> TrimPath(List<Vector3> originalPath, float ratio)
        {
            float totalDistance = GetLength(originalPath);
            float targetDistance = totalDistance * ratio;

            List<Vector3> trimmedPath = new List<Vector3>();
            float currentDistance = 0f;

            for (int i = 0; i < originalPath.Count - 1; i++)
            {
                if (currentDistance >= targetDistance)
                {
                    // 从当前点开始，添加剩余的所有点到新列表
                    trimmedPath.Add(originalPath[i]);  // 确保包含第一个大于ratio对应点的坐标
                    trimmedPath.AddRange(originalPath.GetRange(i + 1, originalPath.Count - i - 1));
                    break;
                }
                // 更新当前累积距离
                currentDistance += Vector3.Distance(originalPath[i], originalPath[i + 1]);
            }

            return trimmedPath;
        }

        public Vector3 GetPosWithRatio(List<Vector3> v3s, float ratio)
        {
            float totalDistance = GetLength(v3s);
            float currentDistance = 0;
            float targetDistance = totalDistance * ratio;
            int currentIndex = 0;

            for (int i = 0; currentDistance <= targetDistance && i < v3s.Count - 1; i++)
            {
                currentIndex = i;
                currentDistance += Vector3.Distance(v3s[i], v3s[i + 1]);
            }

            float lastDistance = targetDistance - (currentDistance - Vector3.Distance(v3s[currentIndex], v3s[currentIndex + 1]));

            if (lastDistance > 0)
            {
                return v3s[currentIndex] + (v3s[currentIndex + 1] - v3s[currentIndex]).normalized * lastDistance;
            }
            return v3s[currentIndex];
        }

        public Vector3 GetDirection(List<Vector3> v3s, float ratio)
        {
            float totalDistance = GetLength(v3s);
            float currentDistance = 0;
            int currentIndex = 0;

            for (int i = 0; currentDistance < totalDistance * ratio && i < v3s.Count - 1; i++)
            {
                currentIndex = i;
                currentDistance += Vector3.Distance(v3s[i], v3s[i + 1]);
            }

            return (v3s[currentIndex + 1] - v3s[currentIndex]).normalized;
        }

        public float GetLength(List<Vector3> v3s)
        {
            float length = 0.0f;
            for (int i = 0; i < v3s.Count - 1; i++)
            {
                length += Vector3.Distance(v3s[i], v3s[i + 1]);
            }
            return length;
        }
        #endregion
    }
}