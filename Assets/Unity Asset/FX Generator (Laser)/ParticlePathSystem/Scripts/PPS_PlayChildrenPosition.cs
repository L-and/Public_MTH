using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ParticlePathSystem
{

	public class PPS_PlayChildrenPosition : PPS_PathPositions
	{

		public Transform parent;
		// Use this for initialization
		void OnEnable()
		{
			
		}

		public override List<Vector3> GetPositions()
		{
			positions.Clear();
			int c = parent.childCount;
			for (int i = 0; i < c; i++)
			{
				positions.Add(parent.GetChild(i).position);
			}
			return positions;
		}
	}
}