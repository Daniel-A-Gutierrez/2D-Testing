using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable()]
public class UnitData : Manager.Data
{
	float unitPositionX;
	float unitPositionY;
	float unitPositionZ;
	bool isDead;
	bool isActive;
	int hp;
	float deadSince;//-1 if alive
	float damagedSince;
	float inactveSince;
	float spawnPositionX;
	float spawnPositionY;
	float spawnPositionZ;
	int level; //if thats a thing
}
