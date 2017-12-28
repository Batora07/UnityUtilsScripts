using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBackToMainMenu : MonoBehaviour {

	public GameObject panelRegles;
	public GameObject panelCredits;

	private Button button = null;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	void Start()
	{
		button.onClick.AddListener(OnClick);
	}

	public void OnClick()
	{
		panelRegles.SetActive(false);
		panelCredits.SetActive(false);
	}
}