using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
	public float throwForce = 200;
	bool hasPlayer = false;
	bool beingCarried = false;
	bool touched = false;
	public float powerMult = 0;

	void Update()
	{
		GameObject player = GameObject.Find("FPS Player(Clone)");
		float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);
		//Debug.Log ("" + dist);
		if (dist <= 1.5f)
		{
			hasPlayer = true;
		}
		else
		{
			hasPlayer = false;
		}
		if (hasPlayer)
			//&& Input.GetKeyDown(KeyCode.E)
		{
			GetComponent<Rigidbody>().isKinematic = true;
			transform.position = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
			transform.rotation = Quaternion.Euler (0,0,90);
			transform.parent = player.transform;
			beingCarried = true;
			powerMult = 0;
		}
		if (beingCarried)
		{
			if (touched)
			{
				GetComponent<Rigidbody>().isKinematic = false;
				transform.parent = null;
				beingCarried = false;
				touched = false;
			}
			if (Input.GetMouseButton(0) && powerMult <= 10)
			{
				powerMult += 10 * Time.deltaTime; // Cap at some max value too
			}
			else if (Input.GetMouseButtonDown(1))
			{
				GetComponent<Rigidbody>().isKinematic = false;
				transform.parent = null;
				beingCarried = false;
			}
			if (Input.GetMouseButtonUp (0)) {
				GetComponent<Rigidbody>().isKinematic = false;
				transform.parent = null;
				beingCarried = false;
				GameObject camera = GameObject.FindGameObjectWithTag ("MainCamera");
				GetComponent<Rigidbody> ().AddForce (camera.transform.forward * throwForce * powerMult, ForceMode.Acceleration);
				GetComponent<Rigidbody> ().AddForce (camera.transform.up * throwForce * powerMult/2);
			}
		}
	}

	void OnTriggerEnter()
	{
		if (beingCarried)
		{
			touched = true;
		}
	}
}