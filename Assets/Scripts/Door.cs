using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class Door : MonoBehaviour 
{
	public string SceneToLoad;
	public Vector2 positionInOtherScene;
	bool doOnce = true;
	BinaryFormatter bf;
	FileStream file;
	GameObject player;
	Vector3 pos;
	Scene current;
	Scene otherScene;
	GameObject[] otherGos;
	GameObject[] currentGos;


	void OnEnable()
	{
		pos =  new Vector3(positionInOtherScene.x, positionInOtherScene.y , 0 );
		player = GameObject.FindGameObjectWithTag("Player");
		current = gameObject.scene;
		print("ALoading: " + SceneToLoad);

		if(current == SceneManager.GetActiveScene())
		{
			print("Loading: " + SceneToLoad);
			SceneManager.LoadSceneAsync(SceneToLoad,LoadSceneMode.Additive);		
			SceneManager.sceneLoaded += FinishLoading;
		}
	}

	void FinishLoading(Scene scene, LoadSceneMode lsm)
	{
		otherScene = SceneManager.GetSceneByName(SceneToLoad);
		otherGos = otherScene.GetRootGameObjects();
		foreach(GameObject go in otherGos)
		{
			print("deactivate : " +go.name);
			//go.SetActive(false);
		}
	}

	void OnDisable()
	{
		print(gameObject.name + " door is now deactivated");
	}
	public void Travel()
	{
		print("hit door in scene " + gameObject.scene);
		SceneManager.MoveGameObjectToScene(player,otherScene);
		player.transform.position = pos;
		SceneManager.UnloadSceneAsync(current);
		SceneManager.SetActiveScene(otherScene);
		print("Set active scene : " + otherScene.name);
		foreach(GameObject go in otherGos)
		{
			print("activate : " +go.name);
			go.SetActive(true);
		}

	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if(doOnce & col.gameObject.tag=="Player" & otherScene.isLoaded)
		{
			doOnce = false;
			Travel();
		}
	}


}
