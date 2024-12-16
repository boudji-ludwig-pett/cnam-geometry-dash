using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
    public GameObject pipe;
    public BirdScript bird;
    public float spawnRate = 1;
    private float timer = 0;
    public float heightOffset = 10;

    public void Start()
    {
        SpawnPipe();
    }

    public void Update()
    {
        if (!bird.isAlive)
        {
            return;
        }
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
            return;
        }

        SpawnPipe();
        timer = 0;
    }

    public void SpawnPipe()
    {
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;
        Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint)), transform.rotation);
    }
}
