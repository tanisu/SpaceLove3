using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    const int MAX_STAGE = 30;
    const int FIRST_BLOCK = 3;
    const int FIRST_LIFE = 2;
    const int DELIMITER_STAGE = 10;
    public static GameManager I;
    public UIController ui;
    public MapGenerator map;
    Player player;
    int stage = 1;
    public int life = FIRST_LIFE;
    public int block = FIRST_BLOCK;
    public int w, h;
    bool isRetry,pushAd;
    
   // int maxStage;
    public enum GAME_STATE
    {
        WAIT,
        PLAY,
        SETBLOCK,
        CLEAR,
        GAMEOVER,
        ENDING
    }

    public GAME_STATE state;

    private void Awake()
    {
        if(I == null)
        {
            I = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (isRetry)
        {
            ADManager.I.ShowInter();
            isRetry = false;
            ui.ShowAdButton();
        }

        SoundManager.I.StartSE();
        SoundManager.I.PlayBGM(BGMSoundData.BGM.MAIN);
        InitGame();
    }

    public void SetMaxStage(int _stage)
    {
        if(_stage < 20)
        {
            stage = _stage;
        }
        else
        {
            stage = 20;
        }
        
        
        w = 2 * _stage -2;
        h = 2 * _stage -2;
    }

    private void InitGame()
    {
        
        map.InitMap();
        ui.ReStart = ResetGame;
        ui.ToTitle = ToTitle;
        ui.ChangeMode = _changeMode;
        ui.HideClearText();
        state = GAME_STATE.PLAY;
        if (ADManager.I.loaded && !pushAd)
        {
            ShowAdButton();
        }
        ui.UpdateStageText(stage);
        player = map.player;
        player.GetItem = _getItem;
        ui.PushArrow = _pushArrow;
        ui.SetHoui(player.transform.localPosition,map.currentGoalPos);
        if(stage > 2)
        {
            int aliens = stage / 3;
            map.PutAlien(aliens);
        }
        if(stage > 6)
        {
            int birdHs = stage / 6;
            map.PutBird(birdHs);
        }
        if(stage > 10)
        {
            int birdVs = stage / 7;
            map.PutBirdV(birdVs);
        }
        if(stage > 15)
        {
            int ballAliens = 3;
            map.PutBallAliens(ballAliens);
        }
        
    }

    


    private void _getItem(string _itemTag)
    {
        switch (_itemTag)
        {
            case "Life":
                life++;
                ui.ShowLife();
                
                break;
            case "Block":
                block +=3;
                ui.UpdateBlockText(block);
                break;
        }
        ui.UpdateBGColor(_itemTag);
    }

    public void GetItemFromAd()
    {
        block += 5;
        ui.UpdateBlockText(block);
        ui.UpdateBGColor("Block");
        ui.HideAdButton();
        pushAd = true;
    }



    private void _pushArrow(int _idx)
    {
        if (state == GAME_STATE.WAIT || state == GAME_STATE.GAMEOVER ) return;
        switch (state)
        {
            case GAME_STATE.PLAY:
                player.Move(_idx);
                ui.SetHoui(player.transform.localPosition, map.currentGoalPos);
                break;
            case GAME_STATE.SETBLOCK:
                player.PutGround(_idx);
                if (player.canPut)
                {
                    block--;
                    ui.UpdateBlockText(block);
                    player.canPut = false;
                }
                state = GAME_STATE.PLAY;
                break;
   
        }
        
    }

    private void _changeMode()
    {
        if(state == GAME_STATE.PLAY)
        {
            if (block < 1) return;
            state = GAME_STATE.SETBLOCK;
            player.ShowCursor();
        }else if(state == GAME_STATE.SETBLOCK)
        {
            state = GAME_STATE.PLAY;
            player.HideCursor();
        }else if(state == GAME_STATE.CLEAR)
        {
            StartCoroutine(_hideClear());
        }else if(state == GAME_STATE.ENDING)
        {
            ToTitle();
        }
    }

    public void CancelMode()
    {
        _changeMode();
    }



    public void ResetGame()
    {
        SoundManager.I.PlaySE(SESoundData.SE.CLICK);
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene("Main");
        Destroy(gameObject);
    }


    void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        GameManager gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gm.SetMaxStage(stage);
        gm.isRetry = true;
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    public void StageClear()
    {
        if(stage == MAX_STAGE)
        {
            StartCoroutine(_showEnding());
            return;
        }
        stage++;
        if(stage > PlayerPrefs.GetInt("Stage"))
        {
            PlayerPrefs.SetInt("Stage", stage);
        }
        
        if(stage < 20)
        {
            w += 2;
            h += 2;
        }
        
        if(stage % DELIMITER_STAGE == 1)
        {
            StartCoroutine(_showClear());
        }
        else
        {
            StartCoroutine(_reloadScene());
        }
        
    }
    public void Miss()
    {
        SoundManager.I.PlaySE(SESoundData.SE.MISS);
        ui.ShowMissText();
        state = GAME_STATE.WAIT;
        life--;
        ui.HideLife(life);
        player.HideCursor();
        if(life >= 0 && block == 0)
        {
            block = FIRST_BLOCK;
            ui.UpdateBlockText(block);
        }

        if(life < 0)
        {
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.75f);
        state = GAME_STATE.GAMEOVER;
        SoundManager.I.PlaySE(SESoundData.SE.DEAD);
        SoundManager.I.StopBGM();
       
        ui.ShowGameOverText();
    }

    private void ToTitle()
    {
        SoundManager.I.StopBGM();
        SceneManager.LoadScene("Title");
        Destroy(gameObject);
    }
    //miss
    public void Restart()
    {
        StartCoroutine(_restart());

    }

    public void ShowAdButton()
    {
        ui.ShowAdButton();
    }

    IEnumerator _restart()
    {
        yield return new WaitForSeconds(1f);
        ui.HideMissText();
        state = GAME_STATE.PLAY;
        
    }

    IEnumerator _showClear()
    {
        ui.ShowClearText();
        ui.HideAdButton();
        state = GAME_STATE.CLEAR;
        yield return new WaitForSeconds(1f);
        ui.ShowClearPanel();
    }

    IEnumerator _showEnding()
    {
        
        state = GAME_STATE.ENDING;
        yield return new WaitForSeconds(1f);
        SoundManager.I.PlayBGM(BGMSoundData.BGM.CLEAR);
        ui.ShowEndingPanel();
    }

    IEnumerator _reloadScene()
    {
        ui.ShowClearText();
        state = GAME_STATE.WAIT;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main");
        yield return new WaitForSeconds(0.1f);
        InitGame();
        ui.UIInit();
    }

    IEnumerator _hideClear()
    {
        ui.HideClearPanel();
        SceneManager.LoadScene("Main");
        yield return new WaitForSeconds(0.1f);
        if (ADManager.I.loaded)
        {
            pushAd = false;
        }

        InitGame();
        ui.UIInit();
    }
    


}
