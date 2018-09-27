using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{	
	public GameObject playerPrefab;
	GameObject player;
	public string path;
	public bool loadOnStart; 
	public bool autoLoadPlayer;

	//[HideInInspector]
	public GameData gameData;
	public string newGameDirectory;
	[HideInInspector]
	public Animator UIAnim;
	[HideInInspector]
	public float fadeInTime;
	//thinking how this is gonna work with a main menu
	//main menu maybe has new game, continue, and options. worry about options later.
	//continue: simply instantiate the manager object and call one of its methods - maybe load the scene. 
	//new game: maybe instantiate the manager object and call one of its methods to delete the game data, and load a specific scene.
	// so in the mean time, this isnt gonna work just plopping something in a scene. maybe ill make a copy or make a new one.

	// OVERRIDE THIS

	//ok so i need to ask the player their name and set that as the save file directory name. 
	//so menu loads, u go to new game, it asks ur name, u give it one and hit enter, loads a file at a predetermined directory and sets the game managers 'path' to 

	void Awake()//this class absolutely must be instantiated first, and absolutely be before player. set up any references that must be done on start .
	{
		DontDestroyOnLoad(this);
		gameData = new GameData();		
		path = Application.dataPath + "/Resources/Saves/" +path;
		UIAnim = transform.Find("Canvas").transform.Find("Panel").GetComponent<Animator>();
		SceneManager.sceneLoaded += OnSceneLoad;
		fadeInTime = UIAnim.GetCurrentAnimatorStateInfo(0).length;
		//if there is no player in the scene, spawn one at 0,0. useful for setting up new saves.
		if(autoLoadPlayer)
		{
			gameData.playerData = new PlayerData();
			player = GameObject.FindGameObjectWithTag("Player");
			if(player == null)
			{
				player = Instantiate(playerPrefab,new Vector3(0,0,0), Quaternion.identity);
			}

		}

	}

	void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		UIAnim.Play("FadeIn");
	}
	
	public void SetPath(string name)//should be called after new game is loaded so that the new path is not overridden. 
	{
		path = Application.dataPath +"/Resources/Saves/" + name;
		gameData.playerData.Name = name;
	}

	public void NewGame()
	{
		string newGameDirectory = Application.dataPath +"/Resources/Saves/" + "New Game"; // this is hardcoded for now, fix later
		print("Fix the hard coded new game");
		string b = path;
		path = newGameDirectory;
		Continue("New Game");
		path = b;
		//load predetermined save file.
		//reestablish link to player.
		//new game can be used for testing for now.
	}

	public void Continue(string identifier)
	{
		//load appropriate save file
		//path =    for now path is fixed
		gameData = (GameData)Load();
		Vector3 p = new Vector3(gameData.playerData.positionX,gameData.playerData.positionY,gameData.playerData.positionZ);
		player = Instantiate(playerPrefab,p,Quaternion.identity);
		player.GetComponent<PlayerController>().playerData = gameData.playerData;
		SceneManager.LoadScene(gameData.Scene);
		//here is where youd do more complicated stuff like loading the unit data and scene data 
	}

	public void Save(GameData d)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(path, FileMode.OpenOrCreate);
		bf.Serialize(file,d);
		file.Close();
	}

	public GameData Load()
	{
		GameData d = new GameData();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(path,FileMode.Open);
		d = (GameData)bf.Deserialize(file);
		file.Close();
		return d;
	}

	public void DeleteAll(GameData d)
	{
		Save(d);//should take a path and delete it instead.
	}

	[Serializable()]
	public class GameData
	{
		public PlayerData playerData;
		public string Scene;
		public float playTime;
		//list of Scene data , which in turn contain flag data in their scenes, and unit data of the units in the scene. 
	}
	 
}
