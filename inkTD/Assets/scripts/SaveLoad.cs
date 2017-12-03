using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public class SaveState
{
	public List<int> gridsKeys;
	// public List< List<InkObject> > gridsValues;
	public List< List<TowerState> > gridsObjects;
	public List<int> incomeKeys;
	public List<float> incomeValues;
	public List<int> balanceKeys;
	public List<float> balanceValues;
	// public List<int> creaturesKeys;
	// public List< List<Creature> > creaturesValues;
	public List<int> creatureSpawnTimeKeys;
	public List<float> creatureSpawnTimeValues;
	public List<int> deadPlayers;
	public float musicVolume;
	public float soundEffectVolume;
}

public class SaveLoad : MonoBehaviour {

	public void Save(string filePath)
	{
		SaveState data = PlayerManager.MakeSave();
		string json = JsonConvert.SerializeObject(data, Formatting.Indented);
		print(json);
		StreamWriter writer = new StreamWriter(filePath, false);
		writer.WriteLine(json);
		writer.Close();
	}
	
	public void Load(string filePath)
	{
		StreamReader reader = new StreamReader(filePath);
		SaveState data = JsonConvert.DeserializeObject<SaveState>(reader.ReadToEnd());
		reader.Close();
		PauseMenu options = GetComponent<PauseMenu>();
		if (options != null)
		{
			options.SetMusicVolume(data.musicVolume);
			options.SetSoundEffectVolume(data.soundEffectVolume);
		}
		PlayerManager.LoadSave(data);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
