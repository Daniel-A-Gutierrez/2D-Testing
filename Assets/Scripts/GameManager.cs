﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class GameManager : Manager
{	
	/*public GameObject playerPrefab;
	GameObject player;
	[HideInInspector]
	
	public string path;
	public bool loadOnStart; */
	//[HideInInspector]
	public GameData gameData;

	//thinking how this is gonna work with a main menu
	//main menu maybe has new game, continue, and options. worry about options later.
	//continue: simply instantiate the manager object and call one of its methods - maybe load the scene. 
	//new game: maybe instantiate the manager object and call one of its methods to delete the game data, and load a specific scene.
	// so in the mean time, this isnt gonna work just plopping something in a scene. maybe ill make a copy or make a new one.

	// OVERRIDE THIS

	//so plan. i need to create a save file and load it. the save file needs to contain the player. it needs to be saved by this script.
	//so open up a scene, create this object and assign values, create a way for the player to save the scene, save it, change the path
	// and save it in a different save file - lets call it 'new game',  open up the menu,
	//create a way to load it, 

	void Awake()//this class absolutely must be instantiated first, and absolutely be before player. set up any references that must be done on start . this is only 
	//really necessary if youre starting the scene without any player shit made up already. 
	{
		DontDestroyOnLoad(this);
		gameData = new GameData();		
		path = Application.dataPath + "/Resources/Saves/" +path;

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
	


	public void NewGame()
	{
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

	public new GameData Load()
	{
		GameData d = new GameData();
		BinaryFormatter bf = new BinaryFormatter();
		string path2 = path.Replace('/','\\');
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
	public class GameData : Data
	{
		public PlayerData playerData;
		public string Scene;
		public float playTime;
		//list of Scene dahta , which in turn contain flag data in their scenes, and unit data of the units in the scene. 
	}
	 
}
