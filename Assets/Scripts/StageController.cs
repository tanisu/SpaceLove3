using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class StageController : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] GameObject howtoPanel;
    int maxStage;
    private void Awake()
    {
        maxStage = PlayerPrefs.GetInt("Stage", 1);
        
    }

    void Start()
    {
        scoreText.text = $"YOUR RECORD  {maxStage} STAGE";
    }

    public void GameStart()
    {
        SoundManager.I.StopSE();
        SceneManager.LoadScene("Main");
    }

    public void GameReStart()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;
        SoundManager.I.StopSE();
        SceneManager.LoadScene("Main");
    }

    void GameSceneLoaded(Scene next,LoadSceneMode mode)
    {
        GameManager gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gm.SetMaxStage(maxStage);
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    public void ShowHowToPanel()
    {
        howtoPanel.SetActive(true);
    }
    public void HideHowToPanel()
    {
        howtoPanel.SetActive(false);
    }


     
}
