using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
	Transform t;
	public Transform sphere;

	void Start()
	{
		t = GetComponent<Transform>();
		t.position = new Vector3(-5f, 5f, 0f);
		t.rotation = Quaternion.Euler(30f, 90f, 0f);
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.A) == true) {
			t.Rotate(new Vector3(0f, -120f*Time.deltaTime, 0f));
		}
		if (Input.GetKey(KeyCode.D) == true) {
			t.Rotate(new Vector3(0f, 120f*Time.deltaTime, 0f));
		}
		if (Input.GetKey(KeyCode.R) == true) {
			t.rotation = Quaternion.Euler(30f, 90f, 0f);
		}
	}

	void LateUpdate()
	{
		t.position = new Vector3(sphere.position.x-5f, 5, 0);
	}
}
