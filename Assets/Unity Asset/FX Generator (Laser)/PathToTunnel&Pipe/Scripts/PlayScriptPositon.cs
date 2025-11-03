using AmazingLineFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PathToTunnel
{
	public class PlayScriptPositon : PathPositions
	{
		public enum DrawType
		{
			pyramid, ring, def, fence
		}

		public DrawType type = DrawType.def;
		void OnEnable()
		{
			if (type == DrawType.def)
			{
				positions.Add(new Vector3(-10, 0, -10));
				positions.Add(new Vector3(10, 0, -10));
				positions.Add(new Vector3(10, 10, -10));
				positions.Add(new Vector3(10, 10, 10));
				positions.Add(new Vector3(10, 0, 10));
				positions.Add(new Vector3(-15, 0, 15));
			}
			else if (type == DrawType.fence)
			{
				positions.Add(new Vector3(-20, 10, -20));
				positions.Add(new Vector3(-20, 10, 20));
				positions.Add(new Vector3(20, 10, 20));
				positions.Add(new Vector3(20, 10, -20));
				positions.Add(new Vector3(-20, 10, -20));
			}
			else if (type == DrawType.pyramid)
			{
				for (int i = 12; i > 0; i -= 2)
				{
					positions.Add(new Vector3(-i * 2, 10 - i * 2, -i * 2));
					positions.Add(new Vector3(i * 2, 10 - i * 2, -i * 2));
					positions.Add(new Vector3(i * 2, 10 - i * 2, i * 2));
					positions.Add(new Vector3(-i * 2, 10 - i * 2, i * 2));

				}
			}
			else if (type == DrawType.ring)
			{
				float angle = 5;
				for (float i = 20; i >= -20; i -= 0.1f)
				{
					float index = (100 - i * 10) * 2;
					float y = Mathf.Sin(angle * index * Mathf.Deg2Rad) * 10;
					float z = Mathf.Cos(angle * index * Mathf.Deg2Rad) * 10;
					positions.Add(new Vector3(i, y, z)); ;
				}
			}
		}
	}
}
