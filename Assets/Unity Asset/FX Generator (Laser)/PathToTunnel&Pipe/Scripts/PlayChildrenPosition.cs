using AmazingLineFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PathToTunnel
{
	public class PlayChildrenPosition : PathPositions
	{

		public Transform parent;
		// Use this for initialization
		void OnEnable()
		{
			int c = parent.childCount;
			for (int i = 0; i < c; i++)
			{
				positions.Add(parent.GetChild(i).position);

			}
			
		}

		// Update is called once per frame
		void Update()
		{
		}
	}
}