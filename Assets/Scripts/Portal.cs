using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour {
	public string Destination;
	public Vector2 position;

	GameObject player;
	GameManager manager;
	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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

	IEnumerator FadeOut() // fade out is handled by doors, fade in is handled by game Managers.
	{
		manager.UIAnim.Play("FadeToBlack");
		player.GetComponent<PlayerController>().Transitioning();
		yield return new WaitForSeconds(manager.fadeInTime);
		Travel();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "Player")
		{
			StartCoroutine("FadeOut");
			//save scene
		}
	}

	void Save()
	{
		//save the state of the room you are leaving
	}
}
