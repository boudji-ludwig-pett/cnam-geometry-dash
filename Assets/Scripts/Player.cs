using System.IO;
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
    public PauseMenu pauseMenu;
    public AudioSource sfxSource;
    public bool editMode { get; set; } = false;

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
        {
            LevelsLoader = loaderObj.GetComponent<LevelsLoader>();
        }
        else
        {
            Debug.LogWarning("LevelsLoader introuvable : Progression désactivée pour ce niveau.");
        }
    }

    public void Start()
    {
        var mainModule = Particle.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        Particle.transform.parent = null;

        CurrentGameMode = new NormalGameMode();
    }

    public bool IsAI
    {
        get
        {
            if (PlayerPrefs.HasKey("AI"))
            {
                return PlayerPrefs.GetInt("AI") == 1;
            }
            else
            {
                return false;
            }
        }
        set
        {
            PlayerPrefs.SetInt("AI", value ? 1 : 0);
        }
    }

    public void Update()
    {
        CurrentGameMode?.Update(this);

        if (LevelsLoader != null)
        {
            LevelsLoader.CalculateCurrentProgressionPercent(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.pauseMenu.activeSelf)
            {
                pauseMenu.Resume();
            }
            else
            {
                pauseMenu.Pause();
            }
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        CurrentGameMode?.OnCollisionEnter(this, collision);

        if (editMode && (collision.gameObject.CompareTag("Kill") || collision.gameObject.CompareTag("Win")))
        {
            GameObject spawn = new GameObject("AutoSpawnPoint");
            spawn.transform.position = new Vector3(-16, -3, 0f);
            transform.position = spawn.transform.position;
            RigidBody.linearVelocity = Vector2.zero;
            SpeedMultiplier = 1f;
            CurrentGameMode = new NormalGameMode();
            SpriteRenderer.sprite = Resources.Load<Sprite>("Shapes/BaseSquare");
            return;
        }

        if (collision.gameObject.CompareTag("Kill"))
        {

            sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "death"));
            sfxSource.Play();
            StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, SceneManager.GetActiveScene().name));

        }

        if (collision.gameObject.CompareTag("Win"))
        {
            sfxSource.clip = Resources.Load<AudioClip>(Path.Combine("Sounds", "win"));
            sfxSource.Play();
            StartCoroutine(LevelHomeButton.PlaySoundAndLoadScene(sfxSource, "SelectLevelScene"));
        }
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
        else if (collision.CompareTag("AICollider") && IsAI)
        {
            CurrentGameMode.Jump(this);
        }
    }

    public void ChangeGameMode(IGameMode newMode)
    {
        CurrentGameMode = newMode;
    }
}
