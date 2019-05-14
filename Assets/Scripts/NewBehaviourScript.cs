using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NewBehaviourScript : NetworkBehaviour
{
	[SerializeField]
	private float distance = 1.0f;
	private int range = 2;
	private RaycastHit hit;
	[SyncVar]
	private GameObject objectID;
	private NetworkIdentity objNetId;

	private int layerMask = 1 << 9; //pickUpLayer is 9

	void Update()
	{
		if (isLocalPlayer)
		{
			CheckIfMoving();
		}
	}

	void CheckIfMoving()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 100, Color.cyan);


		if (isLocalPlayer && Input.GetMouseButton(0))
		{

			if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, layerMask))
			{
				objectID = GameObject.Find(hit.transform.name);                                    // this gets the object that is hit
				CmdPickUp(objectID);
			}
		}
	}


	[ClientRpc]
	void RpcPickUp(GameObject obj, Vector3 objPosition, Quaternion objRotation)
	{
		obj.transform.position = objPosition;
		obj.transform.rotation = objRotation;
	}

	[Command]
	void CmdPickUp(GameObject obj)
	{
		objNetId = obj.GetComponent<NetworkIdentity>();        // get the object's network ID
		objNetId.AssignClientAuthority(connectionToClient);    // assign authority to the player who is changing the color

		Camera playerCamera = Camera.main;
		Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
		Vector3 objPosition = playerCamera.ScreenToWorldPoint(mousePosition);
		Quaternion objRotation = playerCamera.transform.rotation;

		RpcPickUp(obj, objPosition, objRotation);                                    // use a Client RPC function to "paint" the object on all clients

		this.GetComponent<Renderer>().material.color = Color.clear; //finally change color to transparent
		objNetId.RemoveClientAuthority(connectionToClient);    // remove the authority from the player who changed the color
	}
}