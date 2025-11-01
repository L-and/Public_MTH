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
        public GameObject launchEffectPrefab; // 发射特效的预制体
        public float launchEffectDuration = 1f; // 发射特效的加载时间
        public float beamGrowthTime = 1f;
        public float Width;
        public float particleSpeedScale = 1f;
        public int beamSegments = 20;
        public BeamType beamType = BeamType.Straight;
        public bool renderPipe = false;
        public bool enableRaycast = true; // 用于控制是否启用多段射线检测
        public bool enableCollisionDetection = true; // 用于控制是否启用碰撞检测
        public LayerMask raycastLayerMask; // 用于指定射线检测的图层

        public GameObject hitPrefab; // 碰撞特效的预制体
        public float detectionInterval = 0.1f; // 碰撞检测间隔
        public float hitPrefabDestroyTime = 1f;

        // Private Variables
        private BeamPositionsGenerator beamGenerator;
        private List<Vector3> beamPoints;
        private List<Vector3> currentBeamPoints; // 用于存储实际展示的beamPoints
        private List<LineRenderer> lineRenderers = new List<LineRenderer>();
        private List<ParticlePath> particlePaths = new List<ParticlePath>();
        private MeshCollider beamCollider;
        private Vector3 startPos, endPosition;
        private float ratio = 0;
        private float hitRatio = 1f; // 用于存储碰撞发生时的比例
        private float previousRatio = -1f; // 记录上一次的ratio值
        private float beamLength = 0;
        private bool beamLaunched = false;
        private bool isUpdateBeamPositions = false;
        private float detectionTimer = 0f; // 碰撞检测计时器

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
            StartCoroutine(LaunchBeamWithDelay());
        }

        public void UpdateBeam(Vector3 _startPos, Vector3 _endPosition, BeamType _beamType)
        {
            startPos = _startPos;
            endPosition = _endPosition;
            beamType = _beamType;
            beamPoints = beamGenerator.GenerateBeam(startPos, endPosition, beamType, 1.0f, beamSegments); // 计算一次beamPoints
            isUpdateBeamPositions = true;
        }

        IEnumerator LaunchBeamWithDelay()
        {
            // 生成发射特效
            if (launchEffectPrefab != null && beamPoints.Count > 1)
            {
                Vector3 direction = beamPoints[1] - beamPoints[0];
                Quaternion rotation = Quaternion.LookRotation(direction);
                GameObject launchEffect = Instantiate(launchEffectPrefab, startPos, rotation);
                launchEffect.transform.SetParent(transform);
            }

            // 等待发射特效的持续时间
            yield return new WaitForSeconds(launchEffectDuration);

            // 开始beam的生长
            beamLaunched = true;
            ratio = 0;
            previousRatio = -1f; // 确保第一次更新时调用UpdateBeamCollider
        }

        void Update()
        {
            if (beamLaunched)
            {
                ratio += Time.deltaTime / beamGrowthTime;
                if (ratio > hitRatio) ratio = hitRatio; // 如果ratio大于hitRatio，将其设置为hitRatio
                if (Mathf.Abs(ratio - previousRatio) > Mathf.Epsilon || isUpdateBeamPositions) // 检查ratio是否发生变化
                {
                    isUpdateBeamPositions = false;
                    currentBeamPoints = beamGenerator.GetSubVectors(beamPoints, ratio); // 使用GetSubVectors更新currentBeamPoints
                    beamLength = beamGenerator.GetLength(beamPoints);
                    UpdateBeamEffect();
                    UpdateBeamCollider();
                    previousRatio = ratio; // 更新previousRatio
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

        // 查找子节点中的LineRenderer和ParticlePath组件
        private void FindComponentsInChildren()
        {
            lineRenderers.Clear();
            particlePaths.Clear();

            lineRenderers.AddRange(GetComponentsInChildren<LineRenderer>());
            particlePaths.AddRange(GetComponentsInChildren<ParticlePath>());
        }

        // 更新Beam效果
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

        // 更新Beam碰撞体
        private void UpdateBeamCollider()
        {
            List<Vector3> relativePoints = new List<Vector3>();
            foreach (var point in currentBeamPoints)
            {
                relativePoints.Add(point - startPos);
            }

            if (relativePoints == null || relativePoints.Count < 2) return;

            // 生成beam碰撞器的网格
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

        // 执行多段射线检测
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
                hitRatio = 1f; // 如果没有检测到碰撞，hitRatio保持为1
                TriggerHitEffects(endPosition); // 在endPosition处播放特效
            }
        }

        // 执行碰撞检测
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
                hitRatio = 1f; // 如果没有检测到碰撞，hitRatio保持为1
                TriggerHitEffects(endPosition); // 在endPosition处播放特效
            }
        }

        // 触发碰撞效果
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