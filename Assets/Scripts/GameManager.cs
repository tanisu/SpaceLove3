using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    public UIController ui;
    public MapGenerator map;
    Player player;
    int stage = 1;
    public int life = 2;
    public int block = 3;
    public int w, h;
    bool isBlockAd,isRetry;
   // int maxStage;
    public enum GAME_STATE
    {
        WAIT,
        PLAY,
        SETBLOCK,
        CLEAR,
        GAMEOVER
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
        if (stage % 10 == 0)
        {
            isBlockAd = false;
        }
        ui.SwitchAdButton(!isBlockAd);
        map.InitMap();
        ui.ReStart = ResetGame;
        ui.ToTitle = ToTitle;
        ui.ChangeMode = _changeMode;
        ui.HideClearText();
        state = GAME_STATE.PLAY;
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
        if(stage > 9)
        {
            int birdHs = stage / 6;
            map.PutBird(birdHs);
        }
        if(stage > 15)
        {
            int birdVs = stage / 11;
            map.PutBirdV(birdVs);
        }
        if(stage > 20)
        {
            int ballAliens = stage / 20;
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
        isBlockAd = true;
        ui.UpdateBlockText(block);
        ui.UpdateBGColor("Block");
        ui.SwitchAdButton(!isBlockAd);
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
        stage++;
        if(stage > PlayerPrefs.GetInt("Stage"))
        {
            PlayerPrefs.SetInt("Stage", stage);
        }
        
        w += 2;
        h += 2;
        if(stage % 10 == 1)
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
    //Ž€‚ñ‚¾‚Æ‚«
    public void Restart()
    {
        StartCoroutine(_restart());

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
        state = GAME_STATE.CLEAR;
        yield return new WaitForSeconds(1f);
        ui.ShowClearPanel();
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
        InitGame();
        ui.UIInit();
    }
    


}
