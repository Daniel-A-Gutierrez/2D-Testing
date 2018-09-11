using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour {
	public string Destination;
	public Vector2 position;

	GameObject player;
	GameObject manager;
	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		//no managers exist yet
	}

	void Update()
	{
		if (player==null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
		}
	}
	
	void Travel()
	{
		player.transform.position = new Vector3(position.x,position.y,0);
		SceneManager.LoadSceneAsync(Destination);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "Player")
		{
			Travel();
			//save scene
		}
	}

	void Save()
	{
		//save the state of the room you are leaving
	}
}
