using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
	Transform t;
	Rigidbody r;
	
	void Start()
	{
		t = GetComponent<Transform>();
		r = GetComponent<Rigidbody>();
		t.position = new Vector3(0f, 0.5f, 0f);
	}

	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.W) == true) {
			if (t.position.x < 862.1) {
				t.Translate(new Vector3(53.88125f*Time.deltaTime, 0f, 0f));
			}
		}
		if (Input.GetKey(KeyCode.S) == true) {
			if (t.position.x > 0) {
				t.Translate(new Vector3(-53.88125f*Time.deltaTime, 0f, 0f));
			}
		}
	}
}
