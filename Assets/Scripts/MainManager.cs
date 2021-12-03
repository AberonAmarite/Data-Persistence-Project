using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;
    public string recordUsername;
    public int recordScore;

    private void SpawnBricks() {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
       SpawnBricks();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (m_Points > recordScore)
            {
                recordScore = m_Points;
                recordUsername = MenuHandler.username;
                SaveScore();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        if (m_Points != 0 && m_Points % 96 == 0) {
            SpawnBricks();
        }
    }
    private void Awake()
    {

        ScoreText.text = MenuHandler.username + " Score : 0";
        LoadScore();
        BestScoreText.text = $"Best Score: {recordUsername}: {recordScore}";
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = MenuHandler.username + $" Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;

        GameOverText.SetActive(true);
    }
    [System.Serializable]
    class SaveData{
        public string username;
        public int maxScore;
    }

    public void SaveScore()
    {
        SaveData saveData = new SaveData();
        saveData.username = recordUsername;
        saveData.maxScore = recordScore;
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            recordUsername = data.username;
            recordScore = data.maxScore;
        }
        else {
            recordUsername = "Name";
            recordScore = 0;
        }
    }
}
