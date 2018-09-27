using System.Collections;
using System.Collections.Generic;
using System; // you need to import this to serialize objects
using UnityEngine;

[Serializable()]
public class PlayerData 
{
	public float positionX;
	public float positionY;
	public float positionZ;
	public string Name;
	public int hp;
	public int xp;
	public int level;
	public List<object> inventory;
	public Dictionary<string,object> flags;
	//etc. 

}
