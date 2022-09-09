using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueAudio : MonoBehaviour
{
    [SerializeField] private Transform audioOrigin;

    [Header("Audio")]
    [SerializeField] private AudioClip[] voices;
    [SerializeField] private AudioClip[] ponctuations;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float pitchRange = 0f;

    private NPC _npc;
    private float _pitch = 1f;

    private void Start()
    {
        _npc = GetComponent<NPC>();
        InterfaceManager.Instance.AnimatedText.onTextReveal.AddListener(ReproduceSound);
        if (_npc.data != null)
            _pitch = _npc.data.voicePitch;
    }

    private void ReproduceSound(char c)
    {
        if (InterfaceManager.Instance.CurrentNPC != _npc)
            return;

        if(char.IsPunctuation(c))
        {
            SoundManager.Instance.PlaySFXAt(ponctuations[Random.Range(0, ponctuations.Length)], 
                audioOrigin.transform.position, volume: volume, pitch: _pitch, spatialBlend: 0f);
        }

        if(char.IsLetter(c))
        {
            SoundManager.Instance.PlaySFXAt(voices[Random.Range(0, voices.Length)],
                audioOrigin.transform.position, volume: volume, pitch: _pitch, pitchRange: pitchRange, spatialBlend: 0.6f);
        }
    }
}