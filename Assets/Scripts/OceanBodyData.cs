using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanBodyData : MonoBehaviour
{
	public enum OceanBodyType { EnemyWave, Floatsam, Obstacle };

	public OceanBodyType type;
	public float scale;
}
