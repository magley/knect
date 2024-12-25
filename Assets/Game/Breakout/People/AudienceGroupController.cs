using System.Collections.Generic;
using UnityEngine;

public class AudienceGroupController : MonoBehaviour
{
	private AudioSource AudienceAudioSource;
	[SerializeField] private List<AudioClip> SndAudienceClap = new();
	[SerializeField] private List<AudioClip> SndAudienceCheer = new();

	void Start()
    {
		AudienceAudioSource = GetComponent<AudioSource>();
	}

	public void PlayClap()
	{
		AudienceAudioSource.clip = SndAudienceClap[Random.Range(0, SndAudienceClap.Count)];
		AudienceAudioSource.Play();
	}

	public void PlayCheer()
	{
		AudienceAudioSource.clip = SndAudienceCheer[Random.Range(0, SndAudienceCheer.Count)];
		AudienceAudioSource.Play();
	}
}
