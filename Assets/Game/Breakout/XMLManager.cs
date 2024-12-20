using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class BreakoutPlaythroughXML
{
	public int Score { get; set; }
	public int Waves { get; set; }
	public int BestCombo { get; set; }

	public BreakoutPlaythroughXML()
	{
	}

	public BreakoutPlaythroughXML(int score, int waves, int bestCombo)
	{
		Score = score;
		Waves = waves;
		BestCombo = bestCombo;
	}
}

[System.Serializable]
public class GameDataXML
{
	public List<BreakoutPlaythroughXML> breakoutPlaythrough = new List<BreakoutPlaythroughXML>();

	public int GetHighScore()
	{ 
		if (breakoutPlaythrough.Count == 0)
		{
			return 0;
		}
		return breakoutPlaythrough.Max(x => x.Score);
	}
}

public class XMLManager : MonoBehaviour
{
	public static XMLManager instance;
	public GameDataXML data;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}

		if (!Directory.Exists(Application.persistentDataPath + "/Data/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Data/");
		}
	}

	public void AddScore(BreakoutPlaythroughXML score)
	{
		data.breakoutPlaythrough.Add(score);
	}

	public void Save()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(GameDataXML));
		FileStream stream = new FileStream(Application.persistentDataPath + "/Data/data.xml", FileMode.Create);
		serializer.Serialize(stream, data);
		stream.Close();
	}

	public void Load()
	{
		if (File.Exists(Application.persistentDataPath + "/Data/data.xml"))
		{
			XmlSerializer serializer = new XmlSerializer(typeof(GameDataXML));
			FileStream stream = new FileStream(Application.persistentDataPath + "/Data/data.xml", FileMode.Open);
			data = serializer.Deserialize(stream) as GameDataXML;
			stream.Close();
		}
	}
}