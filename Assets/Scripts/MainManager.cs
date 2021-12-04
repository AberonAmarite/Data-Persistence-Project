using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text[] BestScoreTexts;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;
    public SaveDatas savedatas;
    private bool m_GameOver = false;
    private bool newGame;

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
        newGame = true;
        ScoreText.text = MenuHandler.username + " Score : 0";
        LoadScores();
        
        for (int i = 0; i < 10; i++)
        {
            BestScoreTexts[i].text = $"{i + 1}. {savedatas.dataArr[i].username}: {savedatas.dataArr[i].maxScore}";
        }

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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
    }
   

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = MenuHandler.username + $" Score : {m_Points}";
        if (m_Points != 0 && m_Points % 96 == 0)
        { 
                SpawnBricks();
   
        }
    }

    public void GameOver()
    {
        m_GameOver = true;

        GameOverText.SetActive(true);

        if (newGame)
        {

            int pos = FindPos();
            if (pos != -1)
            {
                for (int i = 9; i > pos; i--)
                {
                    savedatas.dataArr[i] = savedatas.dataArr[i - 1];
                }
                savedatas.dataArr[pos].username = MenuHandler.username;
                savedatas.dataArr[pos].maxScore = m_Points;
                for (int i = 0; i < 10; i++)
                {
                    Debug.Log(this.savedatas.dataArr[i].username);
                }
            }
            SaveScores();
            newGame = false;
            return;
        }
    }
    [System.Serializable]
    public class SaveData{
        public string username;
        public int maxScore;
    }
    [System.Serializable]
    public class SaveDatas {
        public SaveData[] dataArr;
    }

    private int FindPos() {
        if(!newGame) return -1;
        for (int i = 0; i < 10; i++)
        {
            if (m_Points > savedatas.dataArr[i].maxScore) {
                return i;
            }
        }
        
        return -1;
    }
    public void SaveScores()
    {
        string json = JsonUtility.ToJson(savedatas);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScores()
    {
        SaveDatas savedatasLocal = new SaveDatas();
        savedatasLocal.dataArr = new SaveData[10];
        for (int i = 0; i < 10; i++) {
            savedatasLocal.dataArr[i]=new SaveData();
        }
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveDatas data = JsonUtility.FromJson<SaveDatas>(json);
                 for (int i = 0; i < 10; i++)
            {
                savedatasLocal.dataArr[i].username = data.dataArr[i].username;
                savedatasLocal.dataArr[i].maxScore = data.dataArr[i].maxScore;
            } 
        }
        else {
            for (int i = 0; i < 10; i++) {
                savedatasLocal.dataArr[i].username = "Name";
                savedatasLocal.dataArr[i].maxScore = 0;
            }
        }
        this.savedatas = savedatasLocal;
        
    }
}
