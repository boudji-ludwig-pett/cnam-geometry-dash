using UnityEngine;

public class PipeMiddleScript : MonoBehaviour
{
    public LogicManagerScript logicManager;

    public void Start()
    {
        logicManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManagerScript>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bird"))
        {

            logicManager.AddScore();
        }
    }
}
