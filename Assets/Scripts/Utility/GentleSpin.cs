using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GentleSpin : MonoBehaviour {

	public float rotateXAmt;
	public float rotateYAmt;
	public float rotateZAmt;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (rotateXAmt > 0.0f) 
			transform.Rotate(Vector3.right * Time.deltaTime * rotateXAmt);

		if (rotateYAmt > 0.0f)
			transform.Rotate(Vector3.forward * Time.deltaTime * rotateYAmt);

		if (rotateZAmt > 0.0f)
			transform.Rotate(Vector3.up * Time.deltaTime * rotateZAmt);
		
	}
}
