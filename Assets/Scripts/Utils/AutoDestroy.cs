using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
	[Range(0.01f, 30.0f)]
	public float delay = 1;

	IEnumerator Start ()
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
}
