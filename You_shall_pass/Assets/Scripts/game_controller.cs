using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class PlayerStatus
{
    public int id;
}

public class game_controller : MonoBehaviour
{
    //variable score
    public static int score = 0;
    public static int crashes;
    public static int lives = 1;
    public string state = "";
    private GameObject player;
    public GameObject pauseMenu;
    public GameObject resultsMenu;
    public GameObject spawners;
    public GameObject map;
    bool keyUp = false;

    public int PlayerId;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        crashes = 0;
        state = "Play";
        Time.timeScale = 1;
        player = GameObject.Find("Player");
        pauseMenu.SetActive(false);
        resultsMenu.SetActive(false);
        map.SetActive(false);

        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (crashes >= lives)
        {
            timer += Time.deltaTime;
            if (timer > 1)
            {
                GameOver();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == "Play")
            {
                Pause();
            }
            else if (state == "Pause")
            {
                Resume();
            }
        }
        if(Input.GetKeyDown(KeyCode.Q) && (state == "Pause" || state == "Results"))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Return) && state == "Results")
        {
            Restart();
        }
        if (state == "Play")
        {
            
            if (Input.GetKeyDown(KeyCode.M))
            {
                ShowMap();
            }
        }
        if (state == "Map")
        {
            if (Input.GetKeyUp(KeyCode.M))
            {
                keyUp = true;
            }

            if (keyUp)
            {
                player.SetActive(true);
                map.SetActive(false);
                state = "Play";
            }
        }
    }
    public void PostData(int intScore, string score = "score")
    {
        StartCoroutine(PostDataCorrutina(intScore, score));
    }

    IEnumerator PostDataCorrutina(int intScore, string strScore)
    {
        string intScoreStr = intScore.ToString(); ;
        string url = "https://localhost:5000/api/post";
        WWWForm form = new WWWForm();
        form.AddField(strScore, intScoreStr);
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
    /*public void GetData()
    {
        StartCoroutine(FetchData());
    }

    public IEnumerator FetchData()
    {
        string URL = "http://localhost/Home/Game";
        using (UnityWebRequest request = UnityWebRequest.Get(URL))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                PlayerStatus playerStat = new PlayerStatus();
                playerStat = JsonUtility.FromJson<PlayerStatus>(request.downloadHandler.text);
                PlayerId = playerStat.id;
            }
        }
    }*/

    public void GameOver()
    {  
        if (state != "Results")
        {
            Application.ExternalCall("UpdateScore", score, 9);

            spawners.SetActive(false);
            resultsMenu.SetActive(true);
            state = "Results";
            resultsMenu.GetComponentInChildren<Text>().text = score.ToString();
        }
    }
    public void Pause()
    {
        state = "Pause";
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        player.SetActive(false);
    }
    public void Resume()
    {
        state = "Play";
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        player.SetActive(true);
    }

    void ShowMap()
    {
        map.SetActive(true);
        state = "Map";
        player.SetActive(false);
        keyUp = false;
    }
    public void MainMenu()
    {
        score = 0;
        SceneManager.LoadScene("mainMenu");
    }
    public void Restart()
    {
        score = 0;
        SceneManager.LoadScene("mainScene");
    }
}
