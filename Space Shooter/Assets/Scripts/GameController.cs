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

    private bool gameOver;
    private bool restartAllowed;
    private int score;


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
        StartCoroutine(SpawnWaves());
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
            while (waveGroup.Count > 0)
            {
                InstantiateHazard(waveGroup);
                yield return new WaitForSeconds(spawnWait[waveNumber]);
                if (gameOver) break;
            }

            if (gameOver)
            {
                yield return new WaitForSeconds(restartTextShownWait);
                restartText.text = "Press 'R' for Restart";
                restartAllowed = true;
                break;
            }

            yield return new WaitForSeconds(waveWait[waveNumber]);

            waveNumber++;
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