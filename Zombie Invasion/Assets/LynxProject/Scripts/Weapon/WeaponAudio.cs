using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAudio : MonoBehaviour {

    public AudioClip[] weaponEject;
    public AudioClip[] weaponRechamber;
    public AudioClip[] weaponFire;

    public void OnReloadStart()
    {
        StartCoroutine(reloadSequence());
    }

    public void OnFire(float delay)
    {
        StartCoroutine(play(delay, weaponFire));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator reloadSequence()
    {
        yield return play(0.1f, weaponEject);
        yield return play(0.6f, weaponRechamber);
    }

    private void play(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    private void play(AudioClip[] clips)
    {
        if (clips.Length > 0)
            play(clips[Random.Range(0, clips.Length)]);
    }

    private IEnumerator play(float delay, AudioClip[] clips)
    {
        yield return new WaitForSeconds(delay);
        play(clips);
    }
}
