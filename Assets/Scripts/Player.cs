using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Rigidbody2D RigidBody { get; private set; }
    public Transform Transform { get; private set; }
    public ParticleSystem Particle { get; private set; }
    public LevelsLoader LevelsLoader { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public bool IsColliding { get; set; } = true;
    public bool HasStarted { get; private set; } = false;
    public bool CanJump { get; set; } = true;

    public IGameMode CurrentGameMode { get; set; }

    public void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Transform = transform;
        Particle = GetComponentInChildren<ParticleSystem>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        LevelsLoader = GameObject.FindGameObjectWithTag("LevelsLoader").GetComponent<LevelsLoader>();
    }

    public void Start()
    {
        var mainModule = Particle.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        Particle.transform.parent = null;

        Invoke(nameof(EnableInput), 0.1f);

        CurrentGameMode = new NormalGameMode();
    }

    private void EnableInput()
    {
        HasStarted = true;
    }

    public void Update()
    {
        CurrentGameMode.Update(this);
        LevelsLoader.CalculateCurrentProgressionPercent(transform.position);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        CurrentGameMode.OnCollisionEnter(this, collision);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        CurrentGameMode.OnCollisionExit(this, collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ShipPortal"))
        {
            SpriteRenderer.sprite = Resources.Load<Sprite>("Shapes/Ship");
            ChangeGameMode(new ShipGameMode());
        }
        else if (collision.CompareTag("CubePortal"))
        {
            SpriteRenderer.sprite = Resources.Load<Sprite>("Shapes/BaseSquare");
            ChangeGameMode(new NormalGameMode());
        }
    }

    public void ChangeGameMode(IGameMode newMode)
    {
        CurrentGameMode = newMode;
    }
}
