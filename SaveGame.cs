using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveGame : MonoBehaviour 
{
	#region VARS/REFS
	public bool ignoreTOD=false;
	
	public static SaveGame instance;
	public SaveData save;

	public string saveSlot = "1";
	#endregion

	#region SAVE SLOT HANDLING
	public string DoesExist(string x)
	{
		if(File.Exists(Application.persistentDataPath+"/"+x+"sav.dat"))
		{
			return "[OCCUPIED]";
		}
		else
		{
			return "";
		}
	}

	public void SetSaveName(string x)
	{
		save.saveName=x;
		Write();
	}
	#endregion

	#region INITIALISATION
	void Awake()
	{
		instance=this;
		saveSlot = PlayerPrefs.GetString("saveSlot");
	}
	#endregion

	#region COLORS
	public Color SkinTone()
	{
		Color color = new Color(save.skinTone[0],save.skinTone[1],save.skinTone[2]);
		return color;
	}

	public Color HairColor()
	{
		Color color = new Color(save.hairColor[0],save.hairColor[1],save.hairColor[2]);
		return color;
	}
	#endregion

	#region READ/WRITE
	public void Write()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath+"/"+GameControl.saveName+"sav"+ saveSlot + ".dat");
		bf.Serialize(file,save);
		file.Close();

		bool savingDuringFade = false;

		if (Game.instance.fadeImage)
		{
			if (Game.instance.fadeImage.gameObject.activeSelf)
			{
				savingDuringFade = true;
			}
		}

		if (!savingDuringFade)
		{
			if (this.gameObject.activeInHierarchy)
			{
				StartCoroutine(TakeScreenShot());
			}
		}

		PlayerPrefs.SetString("saveDate" + saveSlot, DateTime.Now.ToString());
		PlayerPrefs.Save();
	}


	IEnumerator TakeScreenShot()
    {
		List<Canvas> canvases = new List<Canvas>();

		Canvas[] c = FindObjectsOfType<Canvas>();

		foreach (Canvas canvas in c)
		{
			if (canvas.gameObject.activeSelf)
			{
				canvases.Add(canvas);
				canvas.gameObject.SetActive(false);
			}
		}

		bool a = false;
		bool b = false;

		if (!Game.instance.isPreGameMenu)
		{
			if (Game.instance.playerScript.weaponCameraObj.GetComponent<CameraFilterPack_TV_ARCADE_2>().enabled)
			{
				a = true;
				Game.instance.playerScript.weaponCameraObj.GetComponent<CameraFilterPack_TV_ARCADE_2>().enabled = false;
			}
			if (Game.instance.playerScript.weaponCameraObj.GetComponent<CameraFilterPack_Drawing_Paper2>().enabled)
			{
				b = true;
				Game.instance.playerScript.weaponCameraObj.GetComponent<CameraFilterPack_Drawing_Paper2>().enabled = false;
			}
		}

		yield return null;
		ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/" + saveSlot + ".png");
		yield return null;

		foreach (Canvas cc in canvases)
		{
			cc.gameObject.SetActive(true);
		}

		if (!Game.instance.isPreGameMenu)
		{
			if (a) Game.instance.playerScript.weaponCameraObj.GetComponent<CameraFilterPack_TV_ARCADE_2>().enabled = true;
			if (b) Game.instance.playerScript.weaponCameraObj.GetComponent<CameraFilterPack_Drawing_Paper2>().enabled = true;
		}
	}

	public void Read()
	{
		if(File.Exists(Application.persistentDataPath+"/"+GameControl.saveName+ "sav" + saveSlot + ".dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath+"/"+GameControl.saveName+ "sav" + saveSlot + ".dat", FileMode.Open);
			save = (SaveData)bf.Deserialize(file);
			file.Close();

			if (save.conditions == null) save.conditions = new List<string>();
		}
	}
	#endregion
}
