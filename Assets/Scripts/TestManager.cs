using UnityEngine;

public class TestManager : MonoBehaviour
{
    [Header("References")]
    public IGameMode gameMode;
    public Player currentPlayer;
    public Transform spawnPoint;
    public GameObject editorUI;
    public PlayerCamera playerCamera;

    private bool isTesting = false;

    void Start()
    {
        if (spawnPoint == null)
        {
            GameObject spawn = new GameObject("AutoSpawnPoint");
            spawn.transform.position = new Vector3(-16, -3, 0f);
            spawnPoint = spawn.transform;
        }

        if (currentPlayer == null)
        {
            Debug.LogError("[TestManager] Aucun Player assigné !");
        }
        else
        {
            gameMode = new NormalGameMode();
            currentPlayer.ChangeGameMode(gameMode);
            currentPlayer.SpeedMultiplier = 0f;

            if (currentPlayer.SpriteRenderer != null)
                currentPlayer.SpriteRenderer.enabled = false;

            if (currentPlayer.Particle != null)
                currentPlayer.Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // 🛑 Stop propre
        }

        if (playerCamera != null)
        {
            playerCamera.isPlaying = false;
        }
    }

    void Update()
    {
        if (isTesting && currentPlayer == null)
        {
            StopTest();
        }
    }

    public void StartOrStop()
    {
        if (isTesting)
            StopTest();
        else
            StartTest();
    }

    public void StartTest()
    {
        if (currentPlayer == null)
        {
            Debug.LogError("[TestManager] Player manquant pour lancer le test !");
            return;
        }

        if (editorUI != null)
            editorUI.SetActive(false);

        currentPlayer.transform.position = spawnPoint.position;
        currentPlayer.RigidBody.linearVelocity = Vector2.zero;
        currentPlayer.SpeedMultiplier = 1f;
        currentPlayer.SpriteRenderer.sprite = Resources.Load<Sprite>("Shapes/BaseSquare");

        currentPlayer.ChangeGameMode(gameMode);
        isTesting = true;

        if (playerCamera != null)
        {
            playerCamera.playerObject = currentPlayer.gameObject;
            playerCamera.isPlaying = true;
        }

        if (currentPlayer.SpriteRenderer != null)
            currentPlayer.SpriteRenderer.enabled = true;

        if (currentPlayer.Particle != null)
            currentPlayer.Particle.Play(); // ✅ Démarrer la particule

        Debug.Log("[TestManager] Test du niveau démarré !");
    }

    public void StopTest()
    {
        if (currentPlayer != null)
        {
            currentPlayer.transform.position = spawnPoint.position;
            currentPlayer.RigidBody.linearVelocity = Vector2.zero;
            currentPlayer.RigidBody.angularVelocity = 0f;
            currentPlayer.transform.rotation = Quaternion.identity;
            currentPlayer.SpriteRenderer.sprite = Resources.Load<Sprite>("Shapes/BaseSquare");
            currentPlayer.SpeedMultiplier = 0f;

            if (currentPlayer.Particle != null)
                currentPlayer.Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // ✅ Arrêter proprement

            if (currentPlayer.SpriteRenderer != null)
                currentPlayer.SpriteRenderer.enabled = false;
        }

        if (editorUI != null)
            editorUI.SetActive(true);

        if (playerCamera != null)
        {
            playerCamera.isPlaying = false;
            playerCamera.transform.position = new Vector3(0f, 0f, -10f);
        }

        isTesting = false;

        Debug.Log("[TestManager] Test du niveau arrêté, joueur reset et caméra recentrée !");
    }
}
