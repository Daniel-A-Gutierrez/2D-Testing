using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed;
	
	
	Vector3 moveDirection;
	Animator anim;
	
	
	public KeyCode up;
	public KeyCode down;
	public KeyCode left;
	public KeyCode right;
	
	int xAxis;
	int yAxis;
	int FaceX;
	int FaceY;

	Collider2D col;
	ContactPoint2D[] contacts;
	int numContacts;

	// Use this for initialization
	void Start () 
	{
		col = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
		contacts = new ContactPoint2D[16];

	}
	
	// Update is called once per frame
	void Update ()
	{
		SetMoveDirection();
		Move();
	}

	void SetMoveDirection()
	{
		xAxis = 0;
		yAxis = 0;
		xAxis += (Input.GetKey(right)) ? 1 : 0;
		xAxis -= (Input.GetKey(left)) ? 1 : 0;
		yAxis += (Input.GetKey(up)) ? 1 : 0;
		yAxis -= (Input.GetKey(down)) ? 1 : 0;
		moveDirection = new Vector2(xAxis,yAxis).normalized;
	}

	void Move()
	{
		if (xAxis == 0 & yAxis == 0) {anim.Play("IDLE");}
		else
		{ 
			anim.Play("WALK");
			FaceX = xAxis;
			FaceY = yAxis;
			anim.SetFloat("FaceX",FaceX);
			anim.SetFloat("FaceY",FaceY);
		}
		HandleWalls();
		transform.position += moveDirection*speed*Time.deltaTime;
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		numContacts = collision.GetContacts(contacts) -1 ;
	}

	void HandleWalls()
	{

		for (;numContacts>=0;numContacts--)
		{
			Vector3 norm = v3(contacts[numContacts].normal).normalized;
			moveDirection -= norm*Vector3.Dot(moveDirection,norm);
			print(moveDirection);
		}
	}

	Vector2 v2(Vector3 v3)
	{
		return new Vector2(v3.x,v3.y);
	}

	Vector3 v3(Vector2 v2)
	{
		return new Vector3(v2.x,v2.y,0);
	}
}
