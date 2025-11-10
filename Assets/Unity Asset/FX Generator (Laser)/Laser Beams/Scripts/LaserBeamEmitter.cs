using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LaserBeams
{
    public class LaserBeamEmitter : MonoBehaviour
    {
        public BeamType beamType = BeamType.Straight; // ��¶�� beamType ����
        public GameObject emitterPoint;
        public Transform target; // Ŀ�����������
        public Vector3 startPos, targetPosition;
        public GameObject laserBeamPrefab; // LaserBeam Ԥ����

        public bool autoPlay = true;
        private LaserBeam laserBeamInstance; // LaserBeam ʵ��

        private Vector3 previousStartPos, previousTargetPosition; // ��¼��һ�ε�����Ŀ��λ��
        private BeamType previousBeamType; // ��¼��һ�ε� beamType

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

        // 레이저빔을 생성함, 이미 생성되었다면 시작, 도착위치만 재설정함
                public void InstantiateLaserBeam(Vector3 _startPos, Vector3 _endPosition, BeamType _beamType)
                {
                    startPos = _startPos;
                    targetPosition = _endPosition;
                    beamType = _beamType;
                    if (laserBeamInstance == null)
                    {
                        if (laserBeamPrefab != null)
                        {
                            GameObject laserBeamObj = Instantiate(laserBeamPrefab, startPos, Quaternion.identity);
                            laserBeamInstance = laserBeamObj.GetComponent<LaserBeam>();
                            if (laserBeamInstance != null)
                            {
                                laserBeamInstance.Init(startPos, targetPosition, beamType);
                            }
                        }
                    }
                    else
                    {
                        laserBeamInstance.gameObject.SetActive(true);
                        laserBeamInstance.transform.position = startPos;
                        laserBeamInstance.UpdateBeam(startPos, targetPosition, beamType);
                    }
                
                    previousStartPos = startPos;
                    previousTargetPosition = targetPosition;
                    previousBeamType = beamType;
                }
        // ���ټ�������Ч�ķ���
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