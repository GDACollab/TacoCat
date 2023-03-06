using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
	Transform t;
	public Transform sphere;
	
	void Start()
	{
		t = GetComponent<Transform>();
		t.localPosition = new Vector3(-200f, 196.875f, 0f);
	}

	void LateUpdate()
	{
		t.localPosition = new Vector3(((400f/862.1f)*(sphere.position.x)-200f), 196.875f, 0f);
	}
}
