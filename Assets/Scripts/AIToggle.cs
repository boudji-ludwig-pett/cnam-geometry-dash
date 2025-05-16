using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    public Toggle AIToggle;
    public Player player;

    public void OnAiChange()
    {
        player.IsAI = AIToggle.isOn;
    }

    public void Awake()
    {
        AIToggle = GetComponent<Toggle>();
        AIToggle.isOn = player.IsAI;
    }
}
