using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Rigidbody2D RigidBody { get; private set; }
    public Transform Transform { get; private set; }
    public ParticleSystem Particle { get; private set; }
    public LevelsLoader LevelsLoader { get; set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public bool IsColliding { get; set; } = true;
    public bool HasStarted { get; set; } = false;
    public bool CanJump { get; set; } = true;

    public IGameMode CurrentGameMode { get; set; }
    public float SpeedMultiplier = 1f;

    public void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Transform = transform;
        Particle = GetComponentInChildren<ParticleSystem>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        GameObject loaderObj = GameObject.FindGameObjectWithTag("LevelsLoader");
        if (loaderObj != null)
            LevelsLoader = loaderObj.GetComponent<LevelsLoader>();
        else
            Debug.LogWarning("LevelsLoader introuvable : Progression désactivée pour ce niveau.");
    }

    public void Start()
    {
        var mainModule = Particle.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        Particle.transform.parent = null;

        CurrentGameMode = new NormalGameMode();
    }

    public void Update()
    {
        if (!HasStarted)
            return;

        if (CurrentGameMode != null)
            CurrentGameMode.Update(this);

        if (LevelsLoader != null)
            LevelsLoader.CalculateCurrentProgressionPercent(transform.position);
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        CurrentGameMode?.OnCollisionEnter(this, collision);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        CurrentGameMode?.OnCollisionExit(this, collision);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
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
        else if (collision.CompareTag("BonusBoostSpeed"))
        {
            SpeedMultiplier *= 1.5f;
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("BonusSlowSpeed"))
        {
            SpeedMultiplier /= 1.5f;
            Destroy(collision.gameObject);
        }
    }

    public void ChangeGameMode(IGameMode newMode)
    {
        CurrentGameMode = newMode;
    }

    // ➔ Ajout pour supporter le TestManager directement :
    public void StartTest()
    {
        HasStarted = true;
    }

    public void StopTest()
    {
        HasStarted = false;
        RigidBody.linearVelocity = Vector2.zero; // Reset la vitesse proprement
    }
}
