using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	GameManager gameManager ;
	void Start()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	public void Continue()
	{
		gameManager.GetComponent<GameManager>().Continue("madeup");
	}

	public void NewGame()
	{
		gameManager.GetComponent<GameManager>().NewGame();
	}

	public void Options()
	{

	}

	
}
