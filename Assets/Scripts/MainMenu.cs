using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource sfxSource;

    public void LaunchGame()
    {
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, "SelectLevelScene"));
    }

    public void OpenImport()
    {
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, "ImportScene"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LevelEditor()
    {
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, "LevelEditorScene"));
    }

    public void CreateVoidLevel()
    {
        PlayerPrefs.SetInt("CreateMode", 1);
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, "LevelEditorScene"));
    }

    public void EditorChoice()
    {
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, "EditorChoiceScene"));
    }

    public void EditLevel()
    {
        sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "click"));
        sfxSource.Play();

        StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, "SelectLevelToEditScene"));
    }
}
