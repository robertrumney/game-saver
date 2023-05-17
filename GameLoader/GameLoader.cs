using EnemyAI;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GameLoader : MonoBehaviour 
{
	#region VARIABLES,OBJECT REFERENCES & DATA CONSTRUCTS

	public bool ignoreSkyDome = false;

	[HideInInspector]
	public int gender;
	//PRIMITIVE VARIABLES
	//private int currentWeapon;
	public int thisLevel;
	//MODS
	public bool isTutorial = false;
	public bool bonusLevel = false;
	public bool dontSave=false;
	public bool dontLoadPosition=false;
	public bool onlyLoadPosition=false;
	public bool onlySave=false;
	public bool resetSave=false;
	public bool randomGame=false;
	public bool bossFight = false;
	public bool delaySkyMovement;
	public bool noWeapons=false;

	//SINGLETON BIZZLE
	public static GameLoader instance;

	//SKYDOME
	public TOD_Sky skydome;
	public TOD_Time cycle;

	//OBJECT REFERENCES
	public GameObject inventory;

	private bool enemyRoot = true;
	private bool physxRoot= true;
	private bool swapRoot = true;
	private bool pickupRoot= true;

	//PLAYER
	public Transform player;
	public WepModHandler wepMods;
	public SmoothMouseLook fpsCamera;

	//ENEMY SAVE ARRAYS
	public GameObject[] enemies;

	//PICKUPS & STATIC DESTROYABLES
	public GameObject[] pickups;

	//SWAPPABLE OBJECT SAVE ARRAYS
	public GameObject[] swapObjects;

	//PHYSX SAVE OBJECT ARRAYS
	public GameObject[] physXObjects;
	#endregion

	#region POSITION/ROTATION SERIALIZABLE PROXY VECTOR/QUATERNION CLASSES
	public Vector3 Construct(SerializedVector3 v)
	{
		Vector3 vv = new Vector3(v.x,v.y,v.z);
		return vv;
	}
	public Quaternion Construct(SerializedQuaternion q)
	{
		Quaternion qq = new Quaternion(q.x,q.y,q.z,q.w);
		return qq;
	}

	public SerializedVector3 Construct(Vector3 v)
	{
		SerializedVector3 vv = new SerializedVector3(v.x,v.y,v.z);
		return vv;
	}
	public SerializedQuaternion Construct(Quaternion q)
	{
		SerializedQuaternion sq = new SerializedQuaternion(q.x,q.y,q.z,q.w);
		return sq;
	}
	#endregion

	#region CORE START METHODS
	private void Awake()
	{
		instance=this;
	}

	private void StartSky()
	{
		skydome.GetComponent<TOD_Time>().enabled=true;
	}
		
	private void Start()
	{
		if (GameLoader.instance.thisLevel == 1)
		{
			if (PlayerPrefs.HasKey ("easyStart"))
			{
				if (PlayerPrefs.GetInt ("easyStart") == 1)
				{
					if (!PlayerPrefs.HasKey ("easyStarted"))
					{
						resetSave = true;
						PlayerPrefs.SetInt ("easyStarted", 1);
					}
				}
			}
		}
			
		if (resetSave)
			SaveGame.instance.Write ();

		GameControl.paused=false;
		GameControl.objectives=false;
		GameControl.inventory=false;
		GameControl.cutscene=false;
		GameControl.tutorial=false;
		//GameControl.dead=false;
		GameControl.vendor=false;
		GameControl.bulletTime=false;

		//if (Application.isPlaying) 
		//{	
			SaveGame.instance.Read();

			if(isTutorial)
            {
				gender = PlayerPrefs.GetInt("tutorialGender");
				//print(gender);
			}
			else
            {
				gender = SaveGame.instance.save.gender;
			}
			
			Game.instance.LoadChar (gender);

			if(SaveGame.instance.save.begunGame)//IF YOU HAVE ALREADY SAVED ONCE BEFORE
			{
				if(SaveGame.instance.save.level[thisLevel].firstTime)
				{
					SaveGame.instance.save.level[thisLevel].firstTime=false;
					//JUST LOAD PLAYER AMMO/HEALTH/INVENTORY/QUESTS - NOT PLAYER POSITION OR LEVEL DATA
					LoadPlayer();
					Game.instance.player.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
				}
				else
				{
					//LOAD EVERYTHING INCLUDING PLAYER POSITION IN THIS LEVEL AND OBJECT STATES
					LoadGame();
				}
			}
			else//FIRST TIME FROM THIS SAVE SLOT - BEGGINING OF GAME
			{

				//DO NOTHING :) ENJOY THE GAME! // FLAG BEGUN GAME AS TRUE - so that the next time the player saves, it will know to load when playing the level;
				SaveGame.instance.save.begunGame=true;
				SaveGame.instance.save.level[thisLevel].firstTime=false;
				//Save();
			}
		//}

		//Save();//poo ?

	}
	#endregion

	#region SAVING GAME RELATED METHODS
	public void Save()
	{
		if (dontSave)
			return;

		if (Game.instance.playerScript.hitPoints > 0) 
		{
			if (GameControl.inCar)
				SaveGame.instance.save.thirdPerson = true;
			else
				SaveGame.instance.save.thirdPerson = false;

			if(!GameProgress.instance.indoors)
				SaveGame.instance.save.outsideLevel = Application.loadedLevelName;

			if (skydome) 
			{
				if (!SaveGame.instance.ignoreTOD) 
				{
					if (!ignoreSkyDome)
					{
						SaveGame.instance.save.hour = skydome.Cycle.Hour;
						SaveGame.instance.save.day = skydome.Cycle.Day;
						SaveGame.instance.save.month = skydome.Cycle.Month;
						SaveGame.instance.save.year = skydome.Cycle.Year;
					}
				}

			}
			SaveGame.instance.save.currentLevel = thisLevel;

			if (GameProgress.instance) 
			{
				SaveGame.instance.save.level [thisLevel].progress = GameProgress.instance.currentState;
				SaveGame.instance.save.level[thisLevel].firstTime = false;
			}
			Game.instance.inventory.SaveItems ();

			GetPlayerPosition ();
			GetLevelData ();

			if (!noWeapons)
				GetPlayerInfo ();
			
			SaveGame.instance.Write ();
		}
	}

	public void GetPlayerPosition()
	{
		//INIT
		SaveGame.instance.save.level[thisLevel].vector3Array=new SerializedVector3[5];
		SaveGame.instance.save.level[thisLevel].rotationArray=new SerializedQuaternion[5];

		//POPULATE ARRAYS WITH POSITION
		SaveGame.instance.save.level[thisLevel].vector3Array[0]=Construct(Game.instance.playerObjects[0].position);
		SaveGame.instance.save.level[thisLevel].vector3Array[1]=Construct(Game.instance.playerObjects[1].position);
		SaveGame.instance.save.level[thisLevel].vector3Array[2]=Construct(Game.instance.playerObjects[2].position);
		SaveGame.instance.save.level[thisLevel].vector3Array[3]=Construct(Game.instance.playerObjects[3].position);
		SaveGame.instance.save.level[thisLevel].vector3Array[4]=Construct(Game.instance.playerObjects[4].position);

		//POPULATE ARRAYS WITH ROTATION
		SaveGame.instance.save.level[thisLevel].rotationArray[0]=Construct(Game.instance.playerObjects[0].rotation);
		SaveGame.instance.save.level[thisLevel].rotationArray[1]=Construct(Game.instance.playerObjects[1].rotation);
		SaveGame.instance.save.level[thisLevel].rotationArray[2]=Construct(Game.instance.playerObjects[2].rotation);
		SaveGame.instance.save.level[thisLevel].rotationArray[3]=Construct(Game.instance.playerObjects[3].rotation);
		SaveGame.instance.save.level[thisLevel].rotationArray[4]=Construct(Game.instance.playerObjects[4].rotation);
	}

	public void GetLevelData()
	{
		if(enemyRoot){WeighEnemies();}
		if(pickupRoot){WeighPickups();}
		if(swapRoot){WeighSwaps();}
		if(physxRoot){WeighPhysX();}	
	}

	public void GetPlayerInfo()
	{
		//GET COMPONENTS
		PlayerWeapons playerWeps = Game.instance.playerScript.PlayerWeaponsComponent;

		//CONFIGURE ARRAY SIZES
		SaveGame.instance.save.ammoArray=new int[playerWeps.wepBehave.Length];
		SaveGame.instance.save.bulletArray=new int[playerWeps.wepBehave.Length];
		SaveGame.instance.save.ownArray=new bool[playerWeps.wepBehave.Length];
		SaveGame.instance.save.modArray=new int[playerWeps.wepBehave.Length];

		SaveGame.instance.save.intArray=new int[3];
		//GET WEAPONS AND AMMO
		for(var i=0;i<playerWeps.wepBehave.Length;i++)
		{
			SaveGame.instance.save.ammoArray[i]=playerWeps.wepBehave[i].ammo;
			SaveGame.instance.save.bulletArray[i]=playerWeps.wepBehave[i].bulletsLeft;
			SaveGame.instance.save.ownArray[i]=playerWeps.wepBehave[i].haveWeapon;	
		}
		//GET HEALTH AND CURRENT WEAPON
		SaveGame.instance.save.intArray[1]=(int)Game.instance.playerScript.hitPoints;
		SaveGame.instance.save.armor = (int)Game.instance.playerScript.armor;
		SaveGame.instance.save.intArray[2]=playerWeps.currentWeapon;

		//GET MOD INFO
		SaveGame.instance.save.modArray[1]=wepMods.currentMellee;
		SaveGame.instance.save.modArray[2]=wepMods.currentPistol;
		SaveGame.instance.save.modArray[4]=wepMods.currentSMG;
		SaveGame.instance.save.modArray[5]=wepMods.currentShotgun;
		SaveGame.instance.save.modArray[6]=wepMods.currentSniper;
	}

	#endregion

	#region LOADING OBJECTS IN SCENE
	private void LoadLevel()
	{
		if (SaveGame.instance.save.thirdPerson)
			GameControl.inCar = true;

		if(!onlyLoadPosition)
		{
			if(swapRoot && SaveGame.instance.save.level[thisLevel].objects.swapSave.Length!=0)
			{
				LoadSwaps();//LOAD SWAP EXISTENCE
				SetSwaps();//LOAD SWAP INDEX
			}

			if(pickupRoot && SaveGame.instance.save.level[thisLevel].objects.pickupsSave.Length!=0)
			{
				LoadPickups();//LOAD PICKUP EXISTENCE
			}

			if(physxRoot && SaveGame.instance.save.level[thisLevel].objects.physXSave.Length!=0)
			{
				LoadPhysX();//LOAD PHYSX OBJECT EXISTENCE
				LoadPhysXVectors();//LOAD PHYSX OBJECT POSITIONS
				LoadPhysXRotation();//LOAD PHYSX OBJECT ROTATIONS
			}

			if(enemyRoot)
			{
				LoadEnemies();//LOAD ENEMY EXISTENCE
				LoadEnemyVectors();//LOAD ENEMY POSITIONS
				LoadEnemyRotation();//LOAD ENEMY ROTATIONS
			}
		}
	}

	//GET SWAPPABLE WEAPON SWAP INDEX AND EXISTENCE OF
	private void WeighSwaps()
	{
		SaveGame.instance.save.level[thisLevel].objects.swapSave=new bool[swapObjects.Length];
		SaveGame.instance.save.level[thisLevel].objects.swapIndex=new int[swapObjects.Length];

		for(int i=0;i<swapObjects.Length;i++)
		{
			if(swapObjects[i])
			{
				SaveGame.instance.save.level[thisLevel].objects.swapSave[i]=true;
				SaveGame.instance.save.level[thisLevel].objects.swapIndex[i]=swapObjects[i].GetComponent<SwitchWeaponPickup>().replaceGunIndex;
			}
			else
			{
				SaveGame.instance.save.level[thisLevel].objects.swapSave[i]=false;
			}
		}	

	}
	//GET POSITIONS & ROTATIONS OF PHYSICS OBJECTS
	private void WeighPhysX()
	{
		SaveGame.instance.save.level[thisLevel].objects.physXSave = new bool[physXObjects.Length];
		SaveGame.instance.save.level[thisLevel].objects.physXPositions = new SerializedVector3[physXObjects.Length];
		SaveGame.instance.save.level[thisLevel].objects.physXRotations = new SerializedQuaternion[physXObjects.Length];

		for(int i=0;i<physXObjects.Length;i++)
		{
			if(physXObjects[i])
			{
				SaveGame.instance.save.level[thisLevel].objects.physXSave[i]=true;
				SaveGame.instance.save.level[thisLevel].objects.physXPositions[i]=Construct(physXObjects[i].transform.position);
				SaveGame.instance.save.level[thisLevel].objects.physXRotations[i]=Construct(physXObjects[i].transform.rotation);
			}
			else
			{
				SaveGame.instance.save.level[thisLevel].objects.physXSave[i]=false;
			}
		}
	}
	//GET POSITIONS,ROTATIONS & DEAD/ALIVE STATUS OF ENEMIES 
	private void WeighEnemies()
	{
		SaveGame.instance.save.level[thisLevel].objects.enemiesSave = new bool[enemies.Length];
		SaveGame.instance.save.level[thisLevel].objects.enemyPositions = new SerializedVector3[enemies.Length];
		SaveGame.instance.save.level[thisLevel].objects.enemyRotations = new SerializedQuaternion[enemies.Length];

		for(int i=0;i<enemies.Length;i++)
		{
			if (enemies [i])
			{
				if (enemies [i].GetComponent<StateController> ())
				{
					SaveGame.instance.save.level [thisLevel].objects.enemiesSave [i] = true;
					SaveGame.instance.save.level [thisLevel].objects.enemyPositions [i] = Construct (enemies [i].transform.position);
					SaveGame.instance.save.level [thisLevel].objects.enemyRotations [i] = Construct (enemies [i].transform.rotation);
				} 
				else
				{
					SaveGame.instance.save.level [thisLevel].objects.enemiesSave [i] = false;
				}
			}
			else
			{
				SaveGame.instance.save.level [thisLevel].objects.enemiesSave [i] = false;
			}
		}
	}
	//DETERMINE IF PICKUPS HAVE BEEN CONSUMED OR NOT
	private void WeighPickups()
	{
		SaveGame.instance.save.level[thisLevel].objects.pickupsSave = new bool[pickups.Length];

		for(int i=0;i<pickups.Length;i++)
		{
			if(pickups[i])
			{
				SaveGame.instance.save.level[thisLevel].objects.pickupsSave[i]=true;
			}else
			{
				SaveGame.instance.save.level[thisLevel].objects.pickupsSave[i]=false;
			}
		}	
	}
	//DESTROY NON EXISTING SWAP ITEMS
	public void LoadSwaps()
	{
		for(int i=0;i<swapObjects.Length;i++)
		{
			if (swapObjects [i])
			{
				if (SaveGame.instance.save.level [thisLevel].objects.swapSave [i] == false)
				{
					Destroy (swapObjects [i]);
				}
			}
		}
	}
	//SET SWAPPABLE WEAPONS TO WICH THEY HAVE BEEN SWAPPED
	public void SetSwaps()
	{
		for(int i=0;i<swapObjects.Length;i++)
		{
			if(SaveGame.instance.save.level[thisLevel].objects.swapSave[i]==true)
			{
				if(swapObjects[i])
				{
					swapObjects[i].GetComponent<SwitchWeaponPickup>().replaceGunIndex=SaveGame.instance.save.level[thisLevel].objects.swapIndex[i];
					swapObjects[i].GetComponent<SwitchWeaponPickup>().LoadFromSave();
				}
			}
		}
	}
	//DESTROY CONSUMED PICKUPS
	public void LoadPickups()
	{
		for(int i=0;i<pickups.Length;i++)
		{
			if(SaveGame.instance.save.level[thisLevel].objects.pickupsSave[i]==false)
			{
				Destroy(pickups[i]);
			}
		}
	}
	//DESTROY PHYSICS OBJECTS THAT HAVE BEEN DESTROYED
	public void LoadPhysX(){
		
		for(int i=0;i<physXObjects.Length;i++){
			if(SaveGame.instance.save.level[thisLevel].objects.physXSave[i]==false){
				if(physXObjects[i]){
					Destroy(physXObjects[i]);
				}
			}
		}
	}
	//LOAD POSITIONS OF STILL EXISTING PHYSICS OBJECTS
	public void LoadPhysXVectors(){
	
		for(int i=0;i<physXObjects.Length;i++){
			if(SaveGame.instance.save.level[thisLevel].objects.physXSave[i]==true){
				if(physXObjects[i]){
					physXObjects[i].transform.position=Construct(SaveGame.instance.save.level[thisLevel].objects.physXPositions[i]);
				}
			}
		}
	}
	//LOAD ROTATIONS OF STILL EXISTING PHYSICS OBJECTS
	public void LoadPhysXRotation(){

		for(int i=0;i<physXObjects.Length;i++)
		{
			if(SaveGame.instance.save.level[thisLevel].objects.physXSave[i]==true)
			{
				if(physXObjects[i])
				{
					physXObjects[i].transform.rotation=Construct(SaveGame.instance.save.level[thisLevel].objects.physXRotations[i]);
				}
			}
		}
	}

	//KILL ENEMIES THAT HAVE BEEN KILLED
	public void LoadEnemies()
	{
		if (SaveGame.instance.save.level [thisLevel].objects.enemiesSave.Length == 0)
			return;

		for(int i=0;i<enemies.Length;i++)
		{
			if(SaveGame.instance.save.level[thisLevel].objects.enemiesSave[i]==false)
			{
				if(enemies[i])
				{
					if (enemies [i].GetComponent<EnemyAudio> ().currentWaypoint)
					{
						enemies [i].GetComponent<EnemyAudio> ().currentWaypoint.SetActive(false);
					}

					Destroy(enemies[i]);
				}
			}
		}
	}

	IEnumerator LoadKill(GameObject enemy)
	{
		yield return new WaitForSeconds (0.1f);
		enemy.transform.SetParent (null);
		enemy.GetComponent<EnemyHealth> ().LoadKill ();
	}

	//LOAD POSITIONS OF LIVING ENEMIES
	public void LoadEnemyVectors(){
		if (SaveGame.instance.save.level [thisLevel].objects.enemiesSave.Length == 0)
			return;
		
		for(int i=0;i<enemies.Length;i++){
			if(SaveGame.instance.save.level[thisLevel].objects.enemiesSave[i]==true){
				if(enemies[i]){
					enemies[i].transform.position=Construct(SaveGame.instance.save.level[thisLevel].objects.enemyPositions[i]);
				}
			}
		}
	}
	//LOAD ROTATIONS OF LIVING ENEMIES
	public void LoadEnemyRotation(){
		if (SaveGame.instance.save.level [thisLevel].objects.enemiesSave.Length == 0)
			return;
		
		for(int i=0;i<enemies.Length;i++){
			if(SaveGame.instance.save.level[thisLevel].objects.enemiesSave[i]==true){
				if(enemies[i]){
					enemies[i].transform.rotation=Construct(SaveGame.instance.save.level[thisLevel].objects.enemyRotations[i]);
				}
			}
		}
	}
	#endregion

	#region LOADING PLAYER STATE AND POSITION
	public void LoadGame()
	{
		SaveGame.instance.Read();

		LoadLevel();
		LoadPlayer();
		LoadPlayerPositionInLevel();

		if (skydome) 
		{
			if (!ignoreSkyDome)
			{
				skydome.Cycle.Hour = SaveGame.instance.save.hour;
				skydome.Cycle.Day = (int)SaveGame.instance.save.day;
				skydome.Cycle.Year = (int)SaveGame.instance.save.year;
				skydome.Cycle.Month = (int)SaveGame.instance.save.month;
			}
		}
	}

	private void LoadPlayer()
	{
		LoadPlayerHealth();

		if(!noWeapons)
			LoadWeapons();

		Game.instance.inventory.LoadItems();

		if(SaveGame.instance.save.poisoned)
        {
			Game.instance.player.AddComponent<PoisonLoop>();
			Game.instance.playerScript.weaponCameraObj.AddComponent<CameraFilterPack_Blur_Focus>();
			Game.instance.playerScript.bloodPulse.SetActive(true);
			Game.instance.Discover("YOU HAVE BEEN POISONED!\n-USE CRAFTING BENCH TO CRAFT ANTIDOTE-");
		}
	}

	private void LoadPlayerRotation(Quaternion x)
	{
		fpsCamera.LoadRotate(x);
	}

	private Vector3 GetRandomLocation()
	{
		NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

		// Pick the first indice of a random triangle in the nav mesh
		int t = Random.Range(0, navMeshData.indices.Length-3);

		// Select a random point on it
		Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t+1]], Random.value);
		Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t+2]], Random.value);

		return point;
	}

	public void LoadPlayerPositionInLevel()
	{
		if (bossFight || dontLoadPosition)
		{
			Game.instance.player.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
			return;
		}

		if (randomGame)
		{
			Vector3 randomStart = GetRandomLocation ();

			Game.instance.playerObjects[0].position = randomStart;
			Game.instance.playerObjects[1].position = randomStart;
			Game.instance.playerObjects[2].position = randomStart;
			Game.instance.playerObjects[3].position = randomStart;
		} 
		else
		{
			Game.instance.playerObjects[0].position=Construct(SaveGame.instance.save.level[thisLevel].vector3Array[0]);
			Game.instance.playerObjects[1].position=Construct(SaveGame.instance.save.level[thisLevel].vector3Array[1]);
			Game.instance.playerObjects[2].position=Construct(SaveGame.instance.save.level[thisLevel].vector3Array[2]);
			Game.instance.playerObjects[3].position=Construct(SaveGame.instance.save.level[thisLevel].vector3Array[3]);
		}

		LoadPlayerRotation(Construct(SaveGame.instance.save.level[thisLevel].rotationArray[0]));
		Game.instance.player.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
	}

	public void LoadWeapons()
	{
		//INIT
		PlayerWeapons playerWeps = Game.instance.playerScript.PlayerWeaponsComponent;
		//LOAD CURRENTLY OWNED WEAPONS
		for(var i=0;i<playerWeps.wepBehave.Length;i++){
			playerWeps.wepBehave[i].haveWeapon=SaveGame.instance.save.ownArray[i];	
		}	
		//LOAD BULLETS LEFT IN EACH MAGAZINE
		for(var i=0;i<playerWeps.wepBehave.Length;i++){
			playerWeps.wepBehave[i].bulletsLeft=SaveGame.instance.save.bulletArray[i];
		}
		//LOAD REMAINING AMMO FOR EACH WEAPON
		for(var i=0;i<playerWeps.wepBehave.Length;i++){
			playerWeps.wepBehave[i].ammo=SaveGame.instance.save.ammoArray[i];
		}
		//LOAD AND FORCE MODS
		wepMods.newMellee=SaveGame.instance.save.modArray[1];
		wepMods.newPistol=SaveGame.instance.save.modArray[2];
		wepMods.newSMG=SaveGame.instance.save.modArray[4];
		wepMods.newShotgun=SaveGame.instance.save.modArray[5];
		wepMods.newSniper=SaveGame.instance.save.modArray[6];
		wepMods.LoadMods();
		wepMods.ForceMods();
	}
		
	public void LoadPlayerHealth()
	{
		Game.instance.playerScript.hitPoints=(float)SaveGame.instance.save.intArray[1];
		Game.instance.playerScript.healthText.text = Game.instance.playerScript.hitPoints.ToString ();

		Game.instance.playerScript.armor = (float)SaveGame.instance.save.armor;
		Game.instance.playerScript.armorText.text = Game.instance.playerScript.armor.ToString();

		if(Game.instance.playerScript.armor>0)
        {
			Game.instance.playerScript.armorText.transform.parent.gameObject.SetActive(true);
		}

		//currentWeapon =SaveGame.instance.save.intArray[2];
	}
	#endregion
}
