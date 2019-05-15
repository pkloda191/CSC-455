using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownMarking : MonoBehaviour 
{
	bool downed = false;
	bool notDowned = true;
	float distanceTraveled;
	float blueLinePos;
	bool touchdown = false;
	bool deadBall = false;
	bool gameStarted = false;
	public GameObject ball;
	public GameObject yellowLine;
	GameObject player;
	GameObject camera;
	float playerXLocation;
	CharacterController playerController;
	public Text downText;
	public Text snapTimer;
	int down = 1;
	float yard = 2.5f;
	int remainingYardage;
	IEnumerator countdownIEnumerator;


	// Use this for initialization
	void Start () 
	{
		gameObject.transform.position.x.Equals(-0.8);
		downText.text = "1st & 10";
		snapTimer.text = "Snap in: ";
		countdownIEnumerator = StartCountdown ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (down == 1) {
			yellowLine.transform.position = new Vector3 (gameObject.transform.position.x + 12.28f, gameObject.transform.position.y, gameObject.transform.position.z);
		}
		player = GameObject.Find("FPS Player(Clone)");
		playerController = player.GetComponent (typeof(CharacterController)) as CharacterController;
		gotDowned ();
		rightClickListener ();

		if (ball.transform.position.y < 5.17)  //if ball hits ground, start new play
		{
			//deadBall = true;
			//downed = true;
			//nextPlay ();
		}
			
		if (player.transform.position.x > 60f) //if in endzone, touchdown
		{
			touchdown = true;
			gotDowned2 ();
			downText.text = "Touchdown";
		}
		float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);
		//Debug.Log ("" + dist);
			//move x by 2.6 per yard
		//12.28 for 10 yards
	}//end update

	void nextPlay()
	{
		distanceTraveled = playerXLocation - blueLinePos;
		remainingYardage = (int)((yellowLine.transform.position.x - playerXLocation)/1.5);
		//Debug.Log ("" + remainingYardage);
		if (distanceTraveled < 12.2f) 
		{
			if (gameStarted == true && remainingYardage > 0) {
				down++;
			}

			if (remainingYardage < 1) {
				down = 1;
				remainingYardage = 10;
				downText.text = "1st & 10";
				yellowLine.transform.position = new Vector3 (gameObject.transform.position.x + 12.28f, gameObject.transform.position.y, gameObject.transform.position.z);
			}

			if (down == 2) {
				downText.text = "2nd & " + remainingYardage;
			} else if (down == 3) {
				downText.text = "3rd & " + remainingYardage;
			} else if (down == 4) {
				downText.text = "4th & " + remainingYardage;
			} else if (down == 5) {
				downText.text = "Turnover on downs";
			}

		} else {
			down = 1;
			remainingYardage = 10;
			downText.text = "1st & 10";
		}
		//Debug.Log ("" + distanceTraveled);
		if (downed == true) {
			gameObject.transform.position = new Vector3 (playerXLocation, gameObject.transform.position.y, gameObject.transform.position.z);
			ball.transform.position = new Vector3 (playerXLocation, player.transform.position.y, 1);
		}
		//update hud
		StartCoroutine (Wait ());
		StartNewCoroutine ();

		//move player
		//freeze player
		//10 second countdown 
		//right click undoes freeze

		downed = false;

	}

	void rightClickListener()
	{
		if (Input.GetKeyDown (KeyCode.Mouse1)) {
			//unfreeze player
			playerController.enabled = true;
			ball.GetComponent<Rigidbody> ().AddForce (ball.transform.right * 300, ForceMode.Acceleration);
			ball.GetComponent<Rigidbody> ().AddForce (ball.transform.up * 500, ForceMode.Acceleration);
			StopCoroutine (countdownIEnumerator);
		}

	}

	void gotDowned()
	{
		if (Input.GetKeyDown (KeyCode.L)) { //if melee'd representation
			ball.transform.parent = null;
			blueLinePos = gameObject.transform.position.x;
			ball.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			ball.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
			ball.transform.rotation = Quaternion.Euler (0,0,90);
			downed = true;
			playerXLocation = (player.transform.position.x);
			//Debug.Log ("" + playerXLocation);
			nextPlay ();
			gameStarted = true;
		}
	}

	void gotDowned2()
	{
		//if melee'd representation
			blueLinePos = gameObject.transform.position.x;
			ball.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			downed = true;
			playerXLocation = (player.transform.position.x);
			//Debug.Log ("" + playerXLocation);
			nextPlay ();
	}

	void StartNewCoroutine()
	{
		countdownIEnumerator = StartCountdown ();
		StartCoroutine (countdownIEnumerator);
	}

	IEnumerator Wait(){
		yield return new WaitForSeconds (3.0f);
		player.transform.position = new Vector3 (playerXLocation - 10, player.transform.position.y, 1);
		playerController.enabled = false;
	}

	IEnumerator StartCountdown(float countDownValue = 13)
	{
		float currCountdownValue = countDownValue;
		while (currCountdownValue > 0) 
		{
			//Debug.Log ("" + currCountdownValue);
			snapTimer.text = ("Snap in: " + currCountdownValue);
			yield return new WaitForSeconds (1.0f);
			currCountdownValue--;
		}
		playerController.enabled = true;
	}
}
