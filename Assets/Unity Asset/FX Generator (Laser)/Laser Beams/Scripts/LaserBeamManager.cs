using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LaserBeams
{
    public class LaserBeamManager : MonoBehaviour
    {
        public List<GameObject> laserBeamPrefabs; // 存储所有可能的激光预制体
        public bool autoGenerate = true; // 是否自动生成
        public List<LaserBeamEmitter> laserBeamEmitters = new List<LaserBeamEmitter>(); // 存储所有 LaserBeamEmitter 实例

        void Awake()
        {
            FindAllLaserBeamEmitters();
            if (autoGenerate)
            {
                GenerateRandomBeamTypes();
                GenerateRandomLaserBeamPrefabs();
            }
        }

        // 查找所有子物体中的 LaserBeamEmitter 组件
        private void FindAllLaserBeamEmitters()
        {
            laserBeamEmitters.Clear();
            laserBeamEmitters.AddRange(GetComponentsInChildren<LaserBeamEmitter>());
        }

        // 生成随机 BeamType 的所有 LaserBeamEmitter
        public void GenerateRandomBeamTypes()
        {
            foreach (var emitter in laserBeamEmitters)
            {
                BeamType randomType = GetRandomBeamType();
                emitter.beamType = randomType;
                emitter.InstantiateLaserBeam(randomType, null);
            }
        }

        // 生成随机 laserBeamPrefab 的所有 LaserBeamEmitter
        public void GenerateRandomLaserBeamPrefabs()
        {
            foreach (var emitter in laserBeamEmitters)
            {
                GameObject randomPrefab = GetRandomLaserBeamPrefab();
                emitter.InstantiateLaserBeam(emitter.beamType, randomPrefab);
            }
        }

        // 获取随机 BeamType
        private BeamType GetRandomBeamType()
        {
            BeamType[] beamTypes = (BeamType[])System.Enum.GetValues(typeof(BeamType));
            return beamTypes[Random.Range(0, beamTypes.Length)];
        }

        // 获取随机 laserBeamPrefab
        private GameObject GetRandomLaserBeamPrefab()
        {
            if (laserBeamPrefabs.Count == 0) return null;
            return laserBeamPrefabs[Random.Range(0, laserBeamPrefabs.Count)];
        }
    }
}