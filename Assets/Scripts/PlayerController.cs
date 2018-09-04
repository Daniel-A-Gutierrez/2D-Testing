using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed;
	public LayerMask blocks;

	
	Vector2 moveDirection;
	Animator anim;
	
	
	public KeyCode up;
	public KeyCode down;
	public KeyCode left;
	public KeyCode right;
	


	int xAxis;
	int yAxis;
	float FaceX;
	float FaceY;

	BoxCollider2D col;
	RaycastHit2D[] hits;
	int numContacts;
	Rigidbody2D rb2;
	
	float ROOT2;
	Action[] states;
	// Use this for initialization
	void Start () 
	{
		col = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
		hits = new RaycastHit2D[8];
		rb2 = GetComponent<Rigidbody2D>();
		ROOT2 = Mathf.Sqrt(2);

	}
	
	// Update is called once per frame
	void Update ()
	{
		SetMoveDirection();
		TryMove();
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
	// do 8 raycasts around you, then when you move, check if the appropriate raycast has collided. 
	// if it has, dont move in that direction. 
	// 0: top. 1: top right 2:right 3:bot right 4:bot etc.
	void ScanSurroundings()//starting from the center of the box collider, check in all 8 directions.
	{
		Vector2 center = col.offset + v2(transform.position);
		float width = col.size.x;
		float height = col.size.y;
		float diag = col.size.magnitude/4;
		hits[0] = Physics2D.Linecast(center,center + new Vector2(0,1)*height,blocks);
		hits[1] = Physics2D.Linecast(center,center + new Vector2(ROOT2,ROOT2)*diag,blocks);
		hits[2] = Physics2D.Linecast(center,center + new Vector2(1,0)*width,blocks);
		hits[3] = Physics2D.Linecast(center,center + new Vector2(ROOT2,-ROOT2)*diag,blocks);
		hits[4] = Physics2D.Linecast(center,center + new Vector2(0,-1)*height,blocks);
		hits[5] = Physics2D.Linecast(center,center + new Vector2(-ROOT2,-ROOT2)*diag,blocks);
		hits[6] = Physics2D.Linecast(center,center + new Vector2(-1,0)*width,blocks);
		hits[7] = Physics2D.Linecast(center,center + new Vector2(-ROOT2,ROOT2)*diag,blocks);
	}
	//thinking i should basically write a wrapper for transform.
	// take : a direction and magnitude to move
	// return : a direction and magnitude that you moved, if you did. 0,0 for both if you didnt.
	// as for the movement algorithm : determine that you cant move if you have 1 contact normal and 
	// the dot product is -1. 


	void TryMove()
	{

		anim.SetFloat("FaceX",FaceX);
		anim.SetFloat("FaceY",FaceY);
		if (xAxis == 0 & yAxis == 0) //if not moving or putting input, play Idle.
		{
			anim.Play("IDLE");
		}
		else 
		{
			
			Vector2 inputDir = moveDirection;
			moveDirection = Obstruct(moveDirection);
			Vector2 translation = moveDirection * Time.deltaTime * speed;
			FaceX = xAxis;
			FaceY = yAxis;
			if(moveDirection == new Vector2(0,0))
			{
				anim.Play("IDLE");
			}
			else //player is unobstructed
			{
				anim.Play("WALK");
				if(moveDirection!=inputDir)
				{
					FaceX = moveDirection.x;
					FaceY = moveDirection.y;
				}
			}

			
			transform.Translate(translation);
		}
	}

	Vector2 Obstruct(Vector2 movedir) //takes move direction and outputs a valid one based on scan surroundings
	{
		ScanSurroundings();//for now im only going to consider the four cardinal directions
		if(hits[0].collider != null & movedir.y>0){movedir.y += Vector2.Dot(movedir,hits[0].normal.normalized);}
		if(hits[2].collider != null & movedir.x>0){movedir.x += Vector2.Dot(movedir,hits[2].normal.normalized);}
		if(hits[4].collider != null & movedir.y<0){movedir.y -= Vector2.Dot(movedir,hits[4].normal.normalized);}
		if(hits[6].collider != null & movedir.x<0){movedir.x -= Vector2.Dot(movedir,hits[6].normal.normalized);}

		if(hits[0].collider == null)
		{
			if(hits[2].collider == null)
			{
				if(hits[1].collider != null)
				{
					if(movedir.x > 0 & movedir.y > 0)
					{
						movedir *=0;
					}
				}
			}
			if(hits[6].collider == null)
			{
				if(hits[7].collider != null)
				{
					if(movedir.x<0 & movedir.y > 0)
					{
						movedir *=0;
					}
				}
			}
		}
		if(hits[4].collider == null)
		{
			if(hits[2].collider == null)
			{
				if(hits[3].collider != null)
				{
					if(movedir.x>0 & movedir.y < 0)
					{
						movedir *=0;
					}
				}
			}
			if(hits[6].collider == null)
			{
				if(hits[5].collider != null)
				{
					if(movedir.x<0 & movedir.y<0)
					{
						movedir *=0;
					}
				}
			}
		}
		//print(hits[0].normal.x);
		if(movedir.magnitude <.01f)
		{
			return new Vector2(0,0);
		}
		else
		{
			return movedir.normalized;
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
