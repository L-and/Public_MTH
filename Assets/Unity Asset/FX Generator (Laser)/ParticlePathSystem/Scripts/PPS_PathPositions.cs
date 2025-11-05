using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ParticlePathSystem
{
    public class PPS_PathPositions : MonoBehaviour
    {
        public List<Vector3> positions = new List<Vector3>();


        public virtual List<Vector3> GetPositions()
        {
            return positions;
        }
    }
}