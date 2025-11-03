using UnityEngine;
using System.Collections.Generic;

namespace ParticlePathSystem
{
    public class PPS_Example : MonoBehaviour
    {
        public List<ParticlePath> particleSystems; // 存储多个 ParticlePathSystem // Store multiple ParticlePathSystem instances
        public Transform pathPositionsParent;
        private int index = -1;
        private int currentParticleSystemIndex = 0;

        void Start()
        {
            if (particleSystems == null || particleSystems.Count == 0)
            {
                Debug.LogError("No Particle Systems assigned."); // 没有分配粒子系统
                return;
            }

            //foreach (var ps in particleSystems)
            //{
            //    ps.gameObject.SetActive(false); // 隐藏所有粒子系统的GameObject // Hide all particle systems' GameObjects
            //}

            LoadNextPath();
        }

        public void LoadNextPath()
        {
            // 隐藏所有子物体 // Hide all child objects
            for (int i = 0; i < pathPositionsParent.childCount; i++)
            {
                pathPositionsParent.GetChild(i).gameObject.SetActive(false);
            }

            // 更新索引 // Update index
            index++;
            if (index >= pathPositionsParent.childCount) index = 0;

            // 显示当前子物体 // Show current child object
            if (pathPositionsParent.childCount > 0)
            {
                Transform currentChild = pathPositionsParent.GetChild(index);
                currentChild.gameObject.SetActive(true);

                PlayNewPath(currentChild.GetComponent<PPS_PathPositions>().GetPositions());
            }
        }

        public void LoadNextParticale()
        {
            // 隐藏所有粒子系统的GameObject // Hide all particle systems' GameObjects
            foreach (var ps in particleSystems)
            {
                ps.gameObject.SetActive(false);
            }

            // 切换到下一个粒子系统 // Switch to the next particle system
            currentParticleSystemIndex++;
            if (currentParticleSystemIndex >= particleSystems.Count)
            {
                currentParticleSystemIndex = 0;
            }

            // 显示当前粒子系统的GameObject // Show current particle system's GameObject
            particleSystems[currentParticleSystemIndex].gameObject.SetActive(true);

            // 显示当前子物体 // Show current child object
            if (pathPositionsParent.childCount > 0)
            {
                Transform currentChild = pathPositionsParent.GetChild(index);
                PlayNewPath(currentChild.GetComponent<PPS_PathPositions>().GetPositions());
            }
        }

        private void PlayNewPath(List<Vector3> positions)
        {
            particleSystems[currentParticleSystemIndex].InitializePath(positions, particleSystems[currentParticleSystemIndex].speed);
        }
    }
}
