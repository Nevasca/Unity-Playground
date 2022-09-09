using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestructParticle : MonoBehaviour
{
	[SerializeField] private bool _onlyDeactivate;

	private static readonly WaitForSeconds _wait = new WaitForSeconds(0.5f);

	void OnEnable()
	{
		StartCoroutine(CheckIfAliveCoroutine());
	}

	IEnumerator CheckIfAliveCoroutine()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();

		while (true)
		{
			yield return _wait;

			if (particleSystem.IsAlive(true))
				continue;

			if (_onlyDeactivate)
				gameObject.SetActive(false);
			else
				Destroy(gameObject);

			break;
		}
	}
}