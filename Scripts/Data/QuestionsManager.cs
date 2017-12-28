using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class QuestionsManager : MonoBehaviour
{
	public static QuestionsManager instance;
	public List<Questions> questions;
	private List<Questions> questionsUpdated;

	private List<Questions> questionsBeginner;
	private List<Questions> questionsExpert;

	private int indexCurrentQuestion;
	public Questions CurrentQuestion
	{
		get { return questionsUpdated[indexCurrentQuestion]; }
	}

	public List<Questions> QuestionsUpdated
	{
		get
		{
			return questionsUpdated;
		}

		set
		{
			questionsUpdated = value;
		}
	}

	public List<Questions> QuestionsBeginner
	{
		get
		{
			return questionsBeginner;
		}

		set
		{
			questionsBeginner = value;
		}
	}

	public List<Questions> QuestionsExpert
	{
		get
		{
			return questionsExpert;
		}

		set
		{
			questionsExpert = value;
		}
	}

	public ChallengerExpert GetChallengerExpertForQuestion(Questions currentQuestion)
	{
		int nbChallengers = PersonnageManager.instance.challExpert.Count;
		for(int i = 0; i < nbChallengers; ++i)
		{
			if(currentQuestion.personnage == PersonnageManager.instance.challExpert[i].name)
			{
				return PersonnageManager.instance.challExpert[i];
			}
		}
		return null;
	}

	public class IndexQuestionEvent : UnityEvent<int> { };
	public IndexQuestionEvent indexQuestionEvent = new IndexQuestionEvent();

	private void Awake()
    {
		instance = this;
	}

	private void Start()
	{
		questionsUpdated = this.GetComponent<JSONReader>().QuestionsFFR.questions;
		Debug.Log("taille questionsupdated " + questionsUpdated.Count);

		indexCurrentQuestion = 0;
	}

	public Questions GetQuestion(int index)
	{
		return questionsUpdated[index];
	}

	public Questions NextQuestion()
	{
		++indexCurrentQuestion;
		indexQuestionEvent.Invoke(indexCurrentQuestion);
		if (indexCurrentQuestion == questionsUpdated.Count)
		{
			return null;
		}
		else
		{
			return GetQuestion(indexCurrentQuestion);
		}
	}

	public void Reset()
	{
		indexCurrentQuestion = 0;
	}

	/**
	 * Set the list of questions for the beginner mode
	 **/
	public void SetQuestionsBeginner()
	{
		questionsBeginner = new List<Questions>();
		questionsBeginner = SortQuestionsByCharacterBeginner(SortListQuestionsBeginner(SetChallengersQuestion()));
		questionsUpdated = questionsBeginner;
	}

	/**
	 * Set the list of questions for the expert mode
	 **/
	public void SetQuestionsExpert()
	{
		questionsExpert = new List<Questions>();
		questionsExpert = RandomizeQuestionsExpert(SortQuestionsByCharacterExpert(SetChallengersQuestion()));
		questionsUpdated = questionsExpert;
	}

	/**
	 * Call 1/
	 * We get the questions only for the challengers we selected
	 **/
	public List<Questions> SetChallengersQuestion()
	{
		List<Questions> tempArray = new List<Questions>();
		List<ChallengerExpert> nameChallengersArray = PersonnageManager.instance.challExpert;

		int nbQuestions = questionsUpdated.Count;
		int nbChallengers = PersonnageManager.instance.challExpert.Count;
		for(int i = 0; i < nbQuestions; ++i)
		{
			for(int j = 0; j < nbChallengers; ++j)
			{
				if(questionsUpdated[i].personnage == nameChallengersArray[j].name)
				{
					tempArray.Add(questionsUpdated[i]);
				}
			}
		}

		return tempArray;
	}

	/**
	 * Call 2/ Beginner
	 * we sort the questions to only get the questions that are asked to our selected character (do this only if Beginner)
	 **/
	public List<Questions> SortListQuestionsBeginner(List<Questions> questionsList)
	{
		List<Questions> finalQuestionsToAsk = new List<Questions>();

		int nbElemList = questionsList.Count;
		for(int i = 0; i < nbElemList; ++i)
		{
			if("" == questionsList[i].askToBeginner ||
				PersonnageManager.instance.PersonnageSelected.name == questionsList[i].askToBeginner)
			{
				finalQuestionsToAsk.Add(questionsList[i]);
			}
		}

		return finalQuestionsToAsk;
	}

	/**
	 * Call 2/ Expert
	 *  we sort the questions to order them by characters, we do this to get the number of questions by characters
	 *  it's also useful for next step and get a better randomization for the list
	 **/
	public List<Questions> SortQuestionsByCharacterExpert(List<Questions> questionsList)
	{
		List<Questions> finalQuestionsList = new List<Questions>();
		List<ChallengerExpert> nameChallengersArray = PersonnageManager.instance.challExpert;

		int nbChallengers = nameChallengersArray.Count;
		int nbQuestions = questionsList.Count;
		for(int i = 0; i < nbChallengers; ++i)
		{
			for(int j = 0; j < nbQuestions; ++j)
			{
				if(nameChallengersArray[i].name == questionsList[j].personnage)
				{
					finalQuestionsList.Add(questionsList[j]);
					UpdateChallengersCounter(questionsList[j].personnage, 1);
				}
			}
		}

		return finalQuestionsList;
	}


	/**
	 * Call 3/ Expert
	 *  we randomize the questions, only do this for the expert
	 *  Each time the player selects the expert level, 
	 *  the questions list must be randomized
	 **/
	public List<Questions> RandomizeQuestionsExpert(List<Questions> questionsList)
	{
		List<Questions> temp = new List<Questions>();

		int nbQuestions = questionsList.Count;
		Debug.Log(nbQuestions);
		int random;
		for(int i = 0; i < nbQuestions; ++i)
		{
			random = Random.Range(0, questionsList.Count);
			temp.Add(questionsList[random]);
			questionsList.RemoveAt(random);			
		}
		return temp;
	}

	/**
	 * Call 3/ Beginner
	 *  we sort the questions to order them by characters, once again, only do this for the beginner
	 **/
	public List<Questions> SortQuestionsByCharacterBeginner(List<Questions> questionsList)
	{
		List<Questions> finalQuestionsList = new List<Questions>();
		List<ChallengerExpert> nameChallengersArray = PersonnageManager.instance.challExpert;

		int nbChallengers = nameChallengersArray.Count;
		int nbQuestions = questionsList.Count;
		for(int i = 0; i < nbChallengers; ++i)
		{
			for(int j = 0; j < nbQuestions; ++j)
			{
				if(nameChallengersArray[i].name == questionsList[j].personnage)
				{
					finalQuestionsList.Add(questionsList[j]);
				}
			}
		}

		return finalQuestionsList;
	}


	/**
	 * Update the counter for the challengers
	 **/ 
	public void UpdateChallengersCounter(string name, int updatedCounter)
	{
		bool alreadySetEmptyList = false;
		int nbChallengers = PersonnageManager.instance.challExpert.Count;

		for(int i = 0; i < nbChallengers; ++i)
		{
			if("" == PersonnageManager.instance.challExpert[i].name && !alreadySetEmptyList)
			{
				PersonnageManager.instance.challExpert[i].name = name;
				++PersonnageManager.instance.challExpert[i].counterQuestions;
				alreadySetEmptyList = true;
			}
			else if("" == PersonnageManager.instance.challExpert[i].name 
				&& alreadySetEmptyList)
			{
				alreadySetEmptyList = false;
				continue;
			}
			else if(name == PersonnageManager.instance.challExpert[i].name)
			{
				PersonnageManager.instance.challExpert[i].counterQuestions += updatedCounter;
			}
		}
	}

/**
 * Update the counter for the challengers
 **/
	public void UpdateChallengerCurrentCounter(string name, int updatedCounter)
	{
		int nbChallengers = PersonnageManager.instance.challExpert.Count;

		for(int i = 0; i < nbChallengers; ++i)
		{
			if(name == PersonnageManager.instance.challExpert[i].name)
			{
				PersonnageManager.instance.challExpert[i].currentCounter += updatedCounter;
			}
		}
	}


	[ContextMenu("Export Json")]
	void DoSomething()
	{
		string jsonString = JsonUtility.ToJson(this);

		string path = Application.dataPath + "/Data/Json/questions.json";
		Debug.Log("AssetPath:" + path);
		File.WriteAllText(path, jsonString);
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif
	}
}
