using UnityEngine;
using System.Collections.Generic;

namespace ParticlePathSystem
{
    public class ParticlePath : MonoBehaviour
    {
        public PPSPath path;
        public float speed = 5f;
        private ParticleSystem.Particle[] particles;

        void Awake()
        {
            var particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem == null)
            {
                Debug.LogError("Particle System component missing.");
                return;
            }

            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        }

        public void InitializePath(List<Vector3> positions, float _speed)
        {
            path = new PPSPath(positions);
            this.speed = _speed;

            var particleSystem = GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.startLifetime = path.length / speed;
            particleSystem.Stop();
            particleSystem.Clear();
            particleSystem.Play();
        }
        public void UpdatePath(List<Vector3> positions, float _speed)
        {
            path = new PPSPath(positions);
            this.speed = _speed;

            var particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem == null)
            {
                Debug.LogWarning("No ParticleSystem component found on this GameObject.");
                return;
            }

            var main = particleSystem.main;
            float calculatedLifetime = path.length / speed;

            main.startLifetimeMultiplier = calculatedLifetime;

            // 更新已经存在的粒子的生命周期
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
            int numParticlesAlive = particleSystem.GetParticles(particles);

            for (int i = 0; i < numParticlesAlive; i++)
            {
                float remainingLifetime = particles[i].remainingLifetime;
                particles[i].startLifetime = calculatedLifetime;
                particles[i].remainingLifetime = Mathf.Min(remainingLifetime, calculatedLifetime);
            }

            particleSystem.SetParticles(particles, numParticlesAlive);

            // 开始播放粒子系统
            if (!particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
        }



        void Update()
        {
            if (path == null) return;

            var particleSystem = GetComponent<ParticleSystem>();
            int aliveParticles = particleSystem.GetParticles(particles);

            for (int i = 0; i < aliveParticles; i++)
            {
                float lifetime = particles[i].startLifetime - particles[i].remainingLifetime;
                float distance = lifetime * speed;
                Vector3 direction = path.GetDirectionAtDistance(distance);
                particles[i].position += direction * speed * Time.deltaTime;
            }

            particleSystem.SetParticles(particles, aliveParticles);
        }
    }

}
