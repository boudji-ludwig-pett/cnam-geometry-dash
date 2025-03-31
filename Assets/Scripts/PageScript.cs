using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageScript : MonoBehaviour
{
    public List<GameObject> buttons; // À assigner dans l’inspector
    public int visibleCount = 4;
    private int currentIndex = 0;

    public void ShowNext()
    {
        if (currentIndex + visibleCount < buttons.Count)
        {
            currentIndex++;
            UpdateVisibility();
        }
    }

    public void ShowPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateVisibility();
        }
    }

    void Start()
    {
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(i >= currentIndex && i < currentIndex + visibleCount);
        }
    }
}
