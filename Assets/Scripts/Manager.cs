using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Xml.Linq;
using System.Text;
using System.Runtime.Serialization;

public class Manager : MonoBehaviour
{	
	public GameObject playerPrefab;
	public GameObject player;
	[HideInInspector]
	public Data _data;
	public string path;
	public bool loadOnStart;
	public bool autoLoadPlayer;
	//thinking how this is gonna work with a main menu
	//main menu maybe has new game, continue, and options. worry about options later.
	//continue: simply instantiate the manager object and call one of its methods - maybe load the scene. 
	//new game: maybe instantiate the manager object and call one of its methods to delete the game data, and load a specific scene.
	// so in the mean time, this isnt gonna work just plopping something in a scene. maybe ill make a copy or make a new one.

	// Use this for initialization
	void Awake () 
	{
		DontDestroyOnLoad(this);
		_data = new Data();
		path = Path.GetFullPath("Saves")+ "\\" + path;
		


		player = GameObject.FindGameObjectWithTag("Player");
		if(player == null)
		{
			player = Instantiate(playerPrefab,new Vector3(0,0,0), Quaternion.identity);
		}

		if(loadOnStart)
		{
			_data = Load();
		}

	}

	public void Save(Data d)
	{
		print(d);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(path, FileMode.OpenOrCreate);
		bf.Serialize(file,d);
		file.Close();
	}
	/*DataContractSerializer bf = new DataContractSerializer(d.GetType());
		MemoryStream streamer = new MemoryStream();
		bf.WriteObject(streamer,d);
		streamer.Seek(0,SeekOrigin.Begin);
		FileStream file = File.Open(path, FileMode.OpenOrCreate);
		file.Write(streamer.GetBuffer(),0,streamer.GetBuffer().Length);
		file.Close() */

	public Data Load()
	{
		Data d = new Data();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(path,FileMode.Open);
		d = (Data)bf.Deserialize(file);
		file.Close();
		return d;
	}

	public void DeleteAll(Data d)
	{
		//not what i want it to do- i want it to take a path and delete it.
		Save(d);
	}

	[Serializable()]
	public class Data 
	{
	}
	
}
