using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
	[Range(1,20)]
	public float delay = 1;

	IEnumerator Start ()
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
}
