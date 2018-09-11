using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
	public float speed;
	public LayerMask blocks;
	public LayerMask selectable;
	public LayerMask hittable;
	public float hitBoxWidth;
	public float hitBoxHeight;
	public KeyCode up;
	public KeyCode down;
	public KeyCode left;
	public KeyCode right;
	public KeyCode select;

	public float selectOffset;
	public float selectRadius;
	
	Vector2 moveDirection;
	Animator anim;

	GameManager.GameData gameData;
	GameManager gameManager ;
	[HideInInspector]
	public PlayerData playerData; 

	int xAxis;
	int yAxis;
	float FaceX;
	float FaceY;

	BoxCollider2D col;
	RaycastHit2D[] hits;
	int numContacts;
	
	float ROOT2;
	Dictionary<string,Action> states; // need to have namespace system
	string nextState;
	// Use this for initialization
	void Awake()
	{
		DontDestroyOnLoad(this);
	}
	void Start () 
	{
		states = new Dictionary<string,Action>();
		states["Default"] = Default;
		states["Attacking"] = Attacking;
		nextState = "Default";
		col = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
		hits = new RaycastHit2D[8];
		ROOT2 = Mathf.Sqrt(2);
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		gameData = gameManager.gameData;
		playerData = gameData.playerData;
		print(gameData);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Track();
		states[nextState]();
		//print(anim.GetCurrentAnimatorStateInfo(0).nameHash);
	}

//states
	void Default()
	{
		nextState = "Default";
		SetMoveDirection();
		CheckInput();
		TryMove();
	}

	void Attacking()
	{
		//basically you cant do anything while attacking.
		//with this setup it would be possible to make anim cancelling possible after hitframe ends.
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("IDLE"))
		{
			nextState = "Default";
		}
		Hit();
	}
//serialization things
//------------------------------------------------------------------------------------//
	void Track()
	{
		gameData.playTime += Time.deltaTime;
		playerData.positionX = transform.position.x;
		playerData.positionY = transform.position.y;
		playerData.positionZ = transform.position.z;
		gameData.Scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		//anything else you want to save
	}
//-----------------------------------------------------------------------------------//

//input things
	void CheckInput() // should be called using the raw input, so only directly after set move direction.
	{
		if(Input.GetKeyDown(select))
		{
			Collider2D toSelect = Physics2D.OverlapCircle(v2(transform.position) + new Vector2(FaceX,FaceY)*selectOffset,
				selectRadius,selectable);
			if(toSelect != null)
				toSelect.gameObject.GetComponent<Behavior>().Do();
		}
		if(Input.GetKeyDown(KeyCode.Space))
		{
			
			if (!anim.GetCurrentAnimatorStateInfo(0).IsName("ATK"))
			{
				anim.Play("ATK");
				Attacking();
				nextState = "Attacking";
			}
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			print("Saving...");
			print(gameManager);
			gameManager.Save(gameData);
		}
	}


// movement methods
//-----------------------------------------------------------------------------------//

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
	void ScanSurroundings()
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
		ScanSurroundings();
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
		if(movedir.magnitude <.01f)
		{
			return new Vector2(0,0);
		}
		else
		{
			return movedir.normalized;
		}
	}


//nice to haves
//---------------------------------------------------------------------------------------------//
	public Vector2 v2(Vector3 v3)
	{
		return new Vector2(v3.x,v3.y);
	}

	public Vector3 v3(Vector2 v2)
	{
		return new Vector3(v2.x,v2.y,0);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(v2(transform.position) + moveDirection*selectOffset,selectRadius);
		Gizmos.DrawWireCube(v2(transform.position) + new Vector2(0,-1) * hitBoxHeight, new Vector3(1,1,1) * hitBoxHeight);
	}



	// hit frame animations for animation events
	/*------------------------------------------------------------------------------- */
	List<GameObject> slapped;
	bool hitFrames = false;
	
	public void ActivateHitFrames()
	{
		slapped = new List<GameObject>();
		hitFrames = true;
	}

	public void DeactivateHitFrames()
	{
		hitFrames = false;
	}

	public void EndAttackAnim()
	{
		nextState = "Default";
		anim.Play("IDLE");//dont call the default method bc it causes an error if both this and get key down in check input are
		//called on the same frame.
	}


	void Hit()
	{
		if(hitFrames)
		{	
			Vector2 FaceDir = new Vector2(FaceX,FaceY);
			Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position + v3(FaceDir) * hitBoxHeight/2,
				 new Vector2(hitBoxHeight,hitBoxWidth) , Mathf.Atan2(FaceDir.y,FaceDir.x), hittable );
			foreach(Collider2D col in cols)
			{
				// if its in the hittable layer handle taht somehow
				//if it hasnt already been hit
				if(!slapped.Contains(col.gameObject))
				{
					slapped.Add(col.gameObject);
					print("hit");
				}
			}
			
		}
	}
	
}
