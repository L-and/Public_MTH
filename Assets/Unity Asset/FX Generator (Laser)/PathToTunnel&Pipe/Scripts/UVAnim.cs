using UnityEngine;
using System.Collections;

namespace PathToTunnel
{
    public class UVAnim : MonoBehaviour
    {
        public enum AnimState
        {
            xAxis, yAxis, xyAxis
        }
        public AnimState m_animState;
        public float speedX;
        public float speedY;

        void Start()
        {
        }

        void Update()
        {
            Vector2 offset = new Vector2(0, 0);
            if (m_animState == AnimState.xAxis)
            {
                offset = new Vector2(speedX * Time.deltaTime, 0);
            }
            else if (m_animState == AnimState.yAxis)
            {
                offset = new Vector2(0, speedY * Time.deltaTime);
            }
            else if (m_animState == AnimState.xyAxis)
            {
                offset = new Vector2(speedX * Time.deltaTime, speedY * Time.deltaTime);
            }

            Renderer renderer = transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTextureOffset += offset;
            }
        }
    }
}
