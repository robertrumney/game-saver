using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class SaveData 
{
	//GameState
	public string  saveName;
	public bool    thirdPerson;

	//Weather
	public float   hour;
	public float   day;
	public float   month;
	public float   year;
	public bool    raining;

	//character
	public int     gender;
	public int     difficulty = 1;
	public int     hairStyle;

	public bool    begunGame = false;
	public bool    poisoned = false;
	public bool    labCoat=true;
	public bool    gasMask=true;
	public bool    camo = false;

	public float[] skinTone = new float[4];
	public float[] hairColor = new float[4];

	//player stuff
	public int currentWeapon;
	public int armor;

	public int  [] intArray;
	public int  [] ammoArray;
	public int  [] bulletArray;
	public int  [] modArray;
	public bool [] ownArray;
	
	//inventory
	public int savedInvItems;
	public List<InventoryData> inventory = new List<InventoryData>();

	//game progress
	public int currentLevel;
	public LevelProgress[] level;

	//radio 702
	public string quests;

	//locations
	public GameProgress.DiscoverableLocation[] locations;

	//stats
	public int bugs;
	public int loots;
	public int story;
	public int xp;
	public int discovery;
	public int toys;

	//other
	public string outsideLevel;
	public string waitingLevel;

	//conditions
	public List<string> conditions = new List<string>();
	public bool HasCondition(string x)
    {
		foreach(string strung in conditions)
        {
			if (strung == x) return true;
        }

		return false;
    }
}

[Serializable]
public class LevelProgress 
{

	public string name;
	public bool firstTime=true;
	public SerializedVector3 playerPosition;
	public SerializedVector3 []vector3Array;
	public SerializedQuaternion []rotationArray;
	public LevelObjects objects;
	public int progress;
	public int transmissionProgress;
}

[Serializable]
public class SerializedVector3
{
	public float x;
	public float y;
	public float z;

	public SerializedVector3(float a,float b, float c)
	{
		x=a;
		y=b;
		z=c;
	}
}

[Serializable]
public class SerializedQuaternion
{
	public float x;
	public float y;
	public float z;
	public float w;

	public SerializedQuaternion(float a,float b, float c, float d)
	{
		x=a;
		y=b;
		z=c;
		w=d;
	}
}

[Serializable]
public class LevelObjects 
{
	//Pickups
	public bool[] pickupsSave;

	//Swappable Weapons
	public bool[] swapSave;
	public int[] swapIndex;

	//Enemies
	public bool[] enemiesSave;
	public SerializedVector3[] enemyPositions;
	public SerializedQuaternion[] enemyRotations;

	//Physic Objects
	public bool[] physXSave;
	public SerializedVector3[] physXPositions;
	public SerializedQuaternion[] physXRotations;
}

[Serializable]
public class InventoryData
{
	public string Name;
	public bool IsTaken;
	public bool IsStackable;
	public int SavedItems;
	public int MaxAmount;
	public int ItemSlot;

	public int Amount;
	public string ShortDescription;

	public int CurrencyAmount;
	public string EquipmentSlot;
}
