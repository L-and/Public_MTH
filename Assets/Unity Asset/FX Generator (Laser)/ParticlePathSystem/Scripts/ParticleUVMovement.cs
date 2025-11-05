using UnityEngine;
using System.Collections.Generic;
namespace ParticlePathSystem
{
    public class ParticleUVMovement : MonoBehaviour
    {
        public Mesh mesh; // 设置粒子使用的网格
        private ParticleSystem particleSystem;
        private ParticleSystem.Particle[] particles;
        private Vector3[] initialPositions;
        private PPSPath path;

        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

            // 获取网格的顶点数据
            List<Vector3> vertices = new List<Vector3>(mesh.vertices);

            initialPositions = new Vector3[particles.Length];

            // 创建路径
            path = new PPSPath(vertices);

            // 初始化粒子的位置
            InitializeParticles();
        }

        void InitializeParticles()
        {
            int aliveParticles = particleSystem.GetParticles(particles);
            for (int i = 0; i < aliveParticles; i++)
            {
                // 随机选择一个顶点
                int index = Random.Range(0, path.points.Count);
                initialPositions[i] = path.points[index];
                particles[i].position = initialPositions[i];
            }
            particleSystem.SetParticles(particles, aliveParticles);
        }

        void Update()
        {
            int aliveParticles = particleSystem.GetParticles(particles);

            for (int i = 0; i < aliveParticles; i++)
            {
                float lifetime = particles[i].startLifetime - particles[i].remainingLifetime;
                float distance = lifetime * path.length;

                // 获取粒子的方向并移动
                Vector3 direction = path.GetDirectionAtDistance(distance);
                particles[i].position += direction * Time.deltaTime;
            }

            particleSystem.SetParticles(particles, aliveParticles);
        }
    }
}