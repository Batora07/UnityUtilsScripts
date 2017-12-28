using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/**
 * This class is made to parse the JSONs files from the application
 * into usable objects
 **/
public class JSONReader : MonoBehaviour
{

	string path;
	string jsonString;
	ListQuestions questionsFFR;

	public ListQuestions QuestionsFFR
	{
		get
		{
			return questionsFFR;
		}

		set
		{
			questionsFFR = value;
		}
	}

	private void Awake()
	{
		path = Application.dataPath + "/Data/Json/questions.json";
		jsonString = File.ReadAllText(path);
		questionsFFR = JsonUtility.FromJson<ListQuestions>(jsonString);
	}


}
