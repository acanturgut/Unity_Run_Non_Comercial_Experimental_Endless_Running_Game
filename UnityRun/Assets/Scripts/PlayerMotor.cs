using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour {

	private CharacterController controller;
	private Animator anim;
	//private Collider coll;

	private Vector3 moveVector;
	private float speed = 5.0f;
	private float tempSpeed = 5.0f;
	private float VerticalVelocity = 0.0f;
	private float gravity = 11.0f;
	private float jumpForce = 5.0f;

	private bool isDead = false;

	public AudioClip jumpSound;
	public AudioClip deathSound;
	public AudioClip hitSound;
	public AudioClip runSound;
	public AudioClip powerUpStartSound;
	public AudioClip powerUpFinishSound;
	AudioSource audio;

	private float startTime;

	private float animationDuration = 3.0f;

	public bool isPowerUpOn = false;
	public float powerUpDuration = 5.0f;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
		audio = GetComponent<AudioSource> ();
		//coll = GetComponent<Collider> ();
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		if (isDead)
			return;

		if (Time.time - startTime< animationDuration)
		{
			controller.Move (Vector3.forward * speed * Time.deltaTime);
			return;
		} 
		moveVector = Vector3.zero;




		// X - L & R
		moveVector.x = Input.GetAxisRaw("Horizontal")*speed;

		if (Input.GetMouseButton (0))
		{
			if (Input.mousePosition.y > Screen.height / 2) {
				if (controller.isGrounded) {
					Jump ();
				}
					
			} else {
				if (Input.mousePosition.x > Screen.width / 2)
					moveVector.x = speed;
				else 
					moveVector.x = -speed;
			}
				

		}
		// Y - U & D
		moveVector.y = VerticalVelocity;

		//Jump controls
		if (controller.isGrounded) 
		{
			
			VerticalVelocity -= gravity * Time.deltaTime;
			if (Input.GetKeyDown (KeyCode.Space)) 
			{
				Jump ();
			}
		} else {
			VerticalVelocity -= gravity * Time.deltaTime;
		}

		//Falling Controls
		if (transform.position.y < -4)
			Death ();
		// Z - F & B
		moveVector.z = speed;

		controller.Move (moveVector* Time.deltaTime);

		if (isPowerUpOn) {
			powerUpDuration -= Time.deltaTime;
			if (powerUpDuration <= 4.0f) {
				speed = tempSpeed;
				//powerUpDuration = 5.0f;
			}

			if (powerUpDuration <= 0.0f) {
				isPowerUpOn = false;
				audio.PlayOneShot (powerUpFinishSound, 0.7f);
				//powerUpDuration = 5.0f;
			}

		}

	}

	public void SetSpeed (float modifier){
	
		speed = 5.0f + modifier;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (!isPowerUpOn) {
			if (hit.point.z > transform.position.z + 0.1f && hit.gameObject.tag == "Obstacle")
				Death ();
		} else {
			if (hit.gameObject.tag == "Obstacle") {
				audio.PlayOneShot (hitSound, 0.7f);
				anim.Play ("hit01",-1,0f);
				hit.gameObject.SetActive (false);
			}
				
		}
			
		if (hit.gameObject.tag == "Chest") {
			hit.gameObject.SetActive (false);
			audio.PlayOneShot (powerUpStartSound, 0.7f);
			powerUpDuration = 5.0f;
			tempSpeed = speed;
			speed += 2;
			isPowerUpOn = true;
		}
			
	}

	private void Death ()
	{
		anim.Play ("damage",-1,0f);
		audio.PlayOneShot (deathSound, 0.7f);
		isDead = true;
		GetComponent<Score> ().OnDeath ();			
	}

	private void Jump(){
		VerticalVelocity = jumpForce;
		anim.Play ("jump",-1,0f);
		audio.PlayOneShot (jumpSound, 0.7f);
	}
}
