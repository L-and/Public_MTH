using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParticlePathSystem;
using PathToTunnel;


namespace LaserBeams
{
    public class LaserBeam : MonoBehaviour
    { 
        // Public Variables
        public GameObject launchEffectPrefab; // 발사 효과 프리팹
        public float launchEffectDuration = 1f; // 발사 효과 지속 시간
        public float beamGrowthTime = 1f;
        public float Width;
        public float particleSpeedScale = 1f;
        public int beamSegments = 20;
        public BeamType beamType = BeamType.Straight;
        public bool renderPipe = false;
        public bool enableRaycast = true; // 레이캐스트 활성화 여부
        public bool enableCollisionDetection = true; // 충돌 감지 활성화 여부
        public LayerMask raycastLayerMask; // 레이캐스트 레이어 마스크

        public GameObject hitPrefab; // 피격 효과 프리팹
        public float detectionInterval = 0.1f; // 감지 주기
        
        public float hitPrefabDestroyTime = 1f;
        [Tooltip("레이저 지속시간")]
        public float beamDuration = 1f;
        [Tooltip("지속시간이 끝나면 파괴할지 여부")]
        public bool isBeamDestory = false;

        // Private Variables
        private GameObject currentLaunchEffect; // Added for launch effect reuse
        private BeamPositionsGenerator beamGenerator;
        private List<Vector3> beamPoints;
        private List<Vector3> currentBeamPoints; // ????????????beamPoints
        private List<LineRenderer> lineRenderers = new List<LineRenderer>();
        private List<ParticlePath> particlePaths = new List<ParticlePath>();
        private MeshCollider beamCollider;
        private Vector3 startPos, endPosition;
        private float ratio = 0;
        private float hitRatio = 1f; // ??????????????????
        private float previousRatio = -1f; // ??¼??????ratio?
        private float beamLength = 0;
        private bool beamLaunched = false;
        private bool isUpdateBeamPositions = false;
        private float detectionTimer = 0f; // ??????????

        // Initialization
        void Start()
        {
            FindComponentsInChildren();
        }

        public void Init(Vector3 _startPos, Vector3 _endPosition, BeamType _beamType)
        {
            beamGenerator = new BeamPositionsGenerator();
            beamCollider = gameObject.AddComponent<MeshCollider>();
            beamCollider.convex = false;
            UpdateBeam(_startPos, _endPosition, _beamType);
        }

        public void UpdateBeam(Vector3 _startPos, Vector3 _endPosition, BeamType _beamType)
        {
            StopAllCoroutines();

            startPos = _startPos;
            endPosition = _endPosition;
            beamType = _beamType;
            beamPoints = beamGenerator.GenerateBeam(startPos, endPosition, beamType, 1.0f, beamSegments);
            isUpdateBeamPositions = true;

            StartCoroutine(LaunchBeamWithDelay());
        }

        IEnumerator LaunchBeamWithDelay()
        {
            // 빔 발사 시작
            if (currentLaunchEffect != null)
            {
                currentLaunchEffect.SetActive(false);
            }

            if (launchEffectPrefab != null && beamPoints.Count > 1)
            {
                Vector3 direction = beamPoints[1] - beamPoints[0];
                Quaternion rotation = Quaternion.LookRotation(direction);
                Vector3 effectPosition = startPos; 

                if (currentLaunchEffect == null)
                {
                    currentLaunchEffect = Instantiate(launchEffectPrefab, effectPosition, rotation);
                    currentLaunchEffect.transform.SetParent(transform); // Parent to LaserBeam GameObject
                }
                else
                {
                    currentLaunchEffect.transform.position = effectPosition;
                    currentLaunchEffect.transform.rotation = rotation;
                    currentLaunchEffect.SetActive(true);
                }
            }

            beamLaunched = true;
            ratio = 0;
            previousRatio = -1f;

            yield return new WaitForSeconds(beamGrowthTime + beamDuration);

            if (isBeamDestory)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
                
        void Update()
        {
            if (beamLaunched)
            {
                ratio += Time.deltaTime / beamGrowthTime;
                if (ratio > hitRatio) ratio = hitRatio; // 비율이 hitRatio를 초과하지 않도록
                if (Mathf.Abs(ratio - previousRatio) > Mathf.Epsilon || isUpdateBeamPositions) // 비율이 변경되었거나 업데이트가 필요한 경우
                {
                    isUpdateBeamPositions = false;
                    currentBeamPoints = beamGenerator.GetSubVectors(beamPoints, ratio); // GetSubVectors를 사용하여 currentBeamPoints 계산
                    beamLength = beamGenerator.GetLength(beamPoints);
                    UpdateBeamEffect();
                    UpdateBeamCollider();
                    previousRatio = ratio; // 이전 비율 업데이트
                }

                detectionTimer += Time.deltaTime;
                if (detectionTimer >= detectionInterval)
                {
                    detectionTimer = 0f;

                    if (enableRaycast)
                    {
                        PerformMultiSegmentRaycast();
                    }

                    if (enableCollisionDetection)
                    {
                        PerformCollisionDetection();
                    }
                }
            }
        }

        // 자식 오브젝트에서 LineRenderer와 ParticlePath 컴포넌트 찾기
        private void FindComponentsInChildren()
        {
            lineRenderers.Clear();
            particlePaths.Clear();

            lineRenderers.AddRange(GetComponentsInChildren<LineRenderer>());
            particlePaths.AddRange(GetComponentsInChildren<ParticlePath>());
        }

        // ????Beam???
        private void UpdateBeamEffect()
        {
            List<Vector3> relativePoints = new List<Vector3>();
            foreach (var point in currentBeamPoints)
            {
                relativePoints.Add(point - startPos);
            }

            foreach (var lr in lineRenderers)
            {
                lr.positionCount = relativePoints.Count;
                lr.SetPositions(relativePoints.ToArray());
            }

            foreach (var ps in particlePaths)
            {
                ps.UpdatePath(relativePoints, beamLength / beamGrowthTime * particleSpeedScale);
            }
        }

        // ????Beam?????
        private void UpdateBeamCollider()
        {
            List<Vector3> relativePoints = new List<Vector3>();
            foreach (var point in currentBeamPoints)
            {
                relativePoints.Add(point - startPos);
            }

            if (relativePoints == null || relativePoints.Count < 2) return;

            // ????beam???????????
            Mesh beamMesh = PipeGenerator.CreatePipeMesh(relativePoints.ToArray(), Width, 10);

            beamCollider.sharedMesh = beamMesh;
            if (renderPipe)
            {
                beamCollider.gameObject.GetComponent<MeshFilter>().mesh = beamMesh;
                beamCollider.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                beamCollider.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        // ???????????
        private void PerformMultiSegmentRaycast()
        {
            bool hitDetected = false;
            float distanceCovered = 0f;

            for (int i = 0; i < beamPoints.Count - 1; i++)
            {
                Vector3 start = beamPoints[i];
                Vector3 end = beamPoints[i + 1];
                RaycastHit hit;

                distanceCovered += Vector3.Distance(start, end);

                if (Physics.Raycast(start, end - start, out hit, Vector3.Distance(start, end), raycastLayerMask))
                {
                    float hitDistance = distanceCovered - Vector3.Distance(end, hit.point);
                    hitRatio = hitDistance / beamLength;
                    hitDetected = true;
                    TriggerHitEffects(hit.point);
                    break;
                }
            }

            if (!hitDetected)
            {
                hitRatio = 1f; // ?????????????hitRatio?????1
                TriggerHitEffects(endPosition); // ??endPosition?????????
            }
        }

        // ?????????
        private void PerformCollisionDetection()
        {
            RaycastHit hit;
            if (Physics.Raycast(startPos, endPosition - startPos, out hit, Vector3.Distance(startPos, endPosition), raycastLayerMask))
            {
                float hitDistance = Vector3.Distance(startPos, hit.point);
                hitRatio = hitDistance / beamLength;
                TriggerHitEffects(hit.point);
            }
            else
            {
                hitRatio = 1f; // ?????????????hitRatio?????1
                TriggerHitEffects(endPosition); // ??endPosition?????????
            }
        }

        // ??????????
        private void TriggerHitEffects(Vector3 hitPoint)
        {
            if (hitPrefab != null)
            {
                Destroy(Instantiate(hitPrefab, hitPoint, Quaternion.identity), hitPrefabDestroyTime);
            }
            //Debug.Log("Hit detected at: " + hitPoint);
        }

    }
}