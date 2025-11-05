using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LaserBeams
{
    public class LaserBeamEmitter : MonoBehaviour
    {
        public BeamType beamType = BeamType.Straight; // 暴露的 beamType 属性
        public GameObject emitterPoint;
        public Transform target; // 目标物体的引用
        public Vector3 startPos, targetPosition;
        public GameObject laserBeamPrefab; // LaserBeam 预制体

        public bool autoPlay = true;
        private LaserBeam laserBeamInstance; // LaserBeam 实例

        private Vector3 previousStartPos, previousTargetPosition; // 记录上一次的起点和目标位置
        private BeamType previousBeamType; // 记录上一次的 beamType

        private void Awake()
        {
            if (emitterPoint != null && target != null)
            {
                startPos = emitterPoint.transform.position;
                targetPosition = target.position;
            }

            if (autoPlay)
            {
                InstantiateLaserBeam(startPos, targetPosition, beamType);
            }
        }

        void Update()
        {
            if (emitterPoint != null && target != null)
            {
                startPos = emitterPoint.transform.position;
                targetPosition = target.position;
            }

            if (laserBeamInstance != null &&
                (startPos != previousStartPos || targetPosition != previousTargetPosition || beamType != previousBeamType
                || beamType == BeamType.Random))
            {
                laserBeamInstance.UpdateBeam(startPos, targetPosition, beamType);
                previousStartPos = startPos;
                previousTargetPosition = targetPosition;
                previousBeamType = beamType;
            }
        }

        public void InstantiateLaserBeam(BeamType _beamType, GameObject _laserBeamPrefab)
        {
            DestroyLaserBeam();
            if (emitterPoint != null && target != null)
            {
                startPos = emitterPoint.transform.position;
                targetPosition = target.position;
            }
            if (_laserBeamPrefab)
                laserBeamPrefab = _laserBeamPrefab;
            InstantiateLaserBeam(startPos, targetPosition, beamType);
        }

        // 实例化激光束特效的方法
        void InstantiateLaserBeam(Vector3 _startPos, Vector3 _endPosition, BeamType _beamType)
        {
            startPos = _startPos;
            targetPosition = _endPosition;
            beamType = _beamType;
            if (laserBeamInstance == null && laserBeamPrefab != null)
            {
                GameObject laserBeamObj = Instantiate(laserBeamPrefab, startPos, Quaternion.identity);
                laserBeamInstance = laserBeamObj.GetComponent<LaserBeam>();
                if (laserBeamInstance != null)
                {
                    laserBeamInstance.Init(startPos, targetPosition, beamType);
                    previousStartPos = startPos;
                    previousTargetPosition = targetPosition;
                    previousBeamType = beamType;
                }
            }
        }

        // 销毁激光束特效的方法
        void DestroyLaserBeam()
        {
            if (laserBeamInstance != null)
            {
                Destroy(laserBeamInstance.gameObject);
                laserBeamInstance = null;
            }
        }
    }
}