using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	public GameObject[] phrases;
	public Image progressBar;

	void Start ()
	{
		phrases[Random.Range(0, phrases.Length)].SetActive(true);
		StartCoroutine (LoadScene("SampleScene"));
	}

	IEnumerator LoadScene (string sceneName)
	{
		yield return new WaitForSeconds(2.0f);
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		while (!operation.isDone)
		{
			progressBar.fillAmount = operation.progress;
			yield return null;
		}
	}
}