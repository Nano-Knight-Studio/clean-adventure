using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Translation : MonoBehaviour
{
	[SerializeField] private string englishText;
	[SerializeField] private string portugueseText;

	void Start ()
	{
		if (Application.systemLanguage == SystemLanguage.Portuguese)
		{
			GetComponent<TextMeshProUGUI>().text = portugueseText;
		}
		else
		{
			GetComponent<TextMeshProUGUI>().text = englishText;
		}
	}
}