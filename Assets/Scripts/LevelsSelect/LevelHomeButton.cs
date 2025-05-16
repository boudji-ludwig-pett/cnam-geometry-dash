using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHomeButton : MonoBehaviour
{
    public AudioSource sfxSource;

    public static IEnumerator PlaySoundAndLoadScene(AudioSource sfxSource, string scene)
    {
        yield return new WaitWhile(() => sfxSource.isPlaying);
        SceneManager.LoadScene(scene);
    }

    public void GoToHome()
    {
        PlayerPrefs.SetInt("CreateMode", 0);
        PlayerPrefs.SetInt("EditMode", 0);

        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();
        StartCoroutine(PlaySoundAndLoadScene(sfxSource, "HomeScene"));
    }
}
