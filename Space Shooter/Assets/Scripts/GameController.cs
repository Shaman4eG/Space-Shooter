using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public GameObject[] asteroids;
    public GameObject PurpleEnemyShip;

    public int[] numberOfAsteroids;
    public int[] numberOfPurpleEnemyShips;

    /// <summary>
    /// Numeration starts from zero, but shown to player as 'waveNumber + 1', so "Wave 0" is viewed as "Wave 1".
    /// </summary>
    public int waveNumber;
    public int numberOfWaves;

    public float startWait;
    public float[] spawnWait;
    public float[] waveWait;
    public float restartTextShownWait;

    public Vector3 spawnValues;

    public GUIText scoreText;
    public GUIText restartText;
    public GUIText gameOverText;
    public GUIText startText;
    public GUIText waveNumberText;

    private const float rateOfAliveHazardsCounting = 0.3f;

    private bool gameOver;
    private bool restartAllowed;
    private int score;
    private int numberOfAliveHazards;


    void Awake()
    {
        Screen.SetResolution(600, 900, false);
    }

    void Start()
    {
        gameOver = false;
        restartAllowed = false;

        restartText.text = "";
        gameOverText.text = "";
        startText.text = "";
        waveNumberText.text = "";

        score = 0;
        UpdateScore();

        numberOfAliveHazards = 0;

        StartCoroutine(SpawnWaves());
        StartCoroutine(CountAliveHazards());
    }



    void Update()
    {
        if (gameOver) PromptRestart();
    }

    private void PromptRestart()
    {
        if (restartAllowed && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator SpawnWaves()
    {
        List<GameObject> waveGroup;

        startText.text = "Get Ready!";
        yield return new WaitForSeconds(startWait / 2);
        startText.text = "";
        
        for (int wavesLeft = numberOfWaves; wavesLeft > 0; wavesLeft--)
        {
            waveNumberText.text = "Wave " + (waveNumber + 1);
            yield return new WaitForSeconds(startWait / 2);
            waveNumberText.text = "";

            waveGroup = FormWave();
            numberOfAliveHazards = waveGroup.Count;
            while (waveGroup.Count > 0)
            {
                InstantiateHazard(waveGroup);
                yield return new WaitForSeconds(spawnWait[waveNumber]);
                if (gameOver)
                {
                    StartCoroutine(RestartPreparation());
                    break;
                }
            }

            // When wave is fully spawned, we divide next waveWait for 1-second parts,
            // so after each second we can check if player was killed to provide fast restart.
            // If all hazards are killed, we start next wave with only 1 second delay.
            for (int i = (int)waveWait[waveNumber]; i > 0; i--)
            {
                if (gameOver)
                {
                    StartCoroutine(RestartPreparation());
                    break;
                }
                if (numberOfAliveHazards == 0) i = 0;
                yield return new WaitForSeconds(1);
            }
            if (gameOver) break;

            waveNumber++;
        }
    }

    IEnumerator RestartPreparation()
    {
        yield return new WaitForSeconds(restartTextShownWait);
        restartText.text = "Press 'R' for Restart";
        restartAllowed = true;
    }

    IEnumerator CountAliveHazards()
    {
        GameObject[] aliveHazards;

        while (!gameOver)
        {
            aliveHazards = GameObject.FindGameObjectsWithTag("Enemy");

            if (aliveHazards != null &&
                aliveHazards.Length != 0)
            {
                numberOfAliveHazards = aliveHazards.Length;
            }
            else numberOfAliveHazards = 0;

            yield return new WaitForSeconds(rateOfAliveHazardsCounting);
        }
    }


    /// <summary>
    /// Creates wave based on wave number. Wave info given in inspector.
    /// </summary>
    /// <returns> List of enemies in wave. </returns>
    private List<GameObject> FormWave()
    {
        List<GameObject> waveGroup = new List<GameObject>();
        for (int i = numberOfAsteroids[waveNumber]; i > 0; i--)
        {
            waveGroup.Add(asteroids[Random.Range(0, asteroids.Length)]);
        }

        for (int i = numberOfPurpleEnemyShips[waveNumber]; i > 0; i--)
        {
            waveGroup.Add(PurpleEnemyShip);
        }

        return waveGroup;
    }

    /// <summary>
    /// Takes random hazard from listed in current wave.
    /// </summary>
    /// <param name="waveGroup"> Hazards list. </param>
    private void InstantiateHazard(List<GameObject> waveGroup)
    {
        GameObject hazard = waveGroup[Random.Range(0, waveGroup.Count)];
        waveGroup.Remove(hazard);
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(hazard, spawnPosition, spawnRotation);
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        gameOver = true;
    }
}