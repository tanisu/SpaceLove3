using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class UIController : MonoBehaviour
{
    [SerializeField] Text stageText,lifeText,blockText;
    [SerializeField] GameObject clearText, gameOverText,missText,titlePanel,controllerPanel,controllerWrapper;
    [SerializeField] Button StartButton,reStartButton,Abutton,titleButton,adButton;
    [SerializeField] Transform houiTf;
    [SerializeField] Image lifeAreaBG, blockAreaBg;
    public static UIController I;
    Canvas canvas;
    public UnityAction ReStart,ChangeMode,ToTitle;
    public UnityAction<int> PushArrow;
    Arrow[] arrows;
    Abutton abutton;


    private void Awake()
    {
        GameManager.I.ui = this;
    }

    private void Start()
    {

        lifeText.text = GameManager.I.life.ToString();
        blockText.text = GameManager.I.block.ToString();
        canvas = GetComponent<Canvas>();
        reStartButton.onClick.AddListener(() => ReStart?.Invoke());
        titleButton.onClick.AddListener(() =>ToTitle?.Invoke());
        abutton = Abutton.GetComponent<Abutton>();
        arrows = GetComponentsInChildren<Arrow>();
        Abutton.onClick.AddListener(() => {
            StartCoroutine(abutton._pushA());
            ChangeMode?.Invoke(); 
        });
        adButton.onClick.AddListener(() =>  ADManager.I.ShowAdmobReward());

    }

    public void SwitchAdButton(bool flag)
    {
        adButton.interactable = flag;
        if(flag == false)
        {
            adButton.gameObject.SetActive(false);
        }
        else
        {
            adButton.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit && hit.collider.GetComponent<Arrow>())
            {
                for (int i = 0; i < arrows.Length; i++)
                {
                    if(hit.collider.gameObject == arrows[i].gameObject)
                    {
                        StartCoroutine(arrows[i].ChangeSprite());
                        PushArrow?.Invoke(i);
                    }
                }
            }
        }
    }

    public void UpdateBGColor(string _tag)
    {
        SoundManager.I.PlaySE(SESoundData.SE.ITEM);
        switch (_tag)
        {
            case "Life":
                
                StartCoroutine(_rainbow(lifeAreaBG));
                break;
            case "Block":
                StartCoroutine(_rainbow(blockAreaBg));
                break;
        }
    }

    IEnumerator _rainbow(Image _image)
    {
        Color tmpColor = _image.color;
        for (int i = 0; i < 60; i++)
        {
            
            _image.color = Color.HSVToRGB(Time.time % 1, 1, 1);
            yield return new WaitForSeconds(0.01f);
        }
        _image.color = tmpColor;
        
    }

    public void SetHoui(Vector2 playerPos,Vector2 goalPos)
    {
        Vector2 lookDir = goalPos-playerPos;
        
        float angle = Mathf.Atan2(lookDir.y,lookDir.x) * Mathf.Rad2Deg -90f;
        
        houiTf.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void ShowLife()
    {
        lifeText.text = GameManager.I.life.ToString();
    }


    public void HideLife(int idx)
    {
        lifeText.text = idx.ToString();
    }

    public void UpdateBlockText(int i)
    {
        blockText.text = i.ToString();
    }

    public void UpdateStageText(int _stage)
    {
        stageText.text = $"STAGE {_stage}";
    }

    public void ShowClearText()
    {
        clearText.SetActive(true);
    }
    public void HideClearText()
    {
        clearText.SetActive(false);
    }

    public void ShowGameOverText()
    {
        controllerWrapper.SetActive(false);
        gameOverText.SetActive(true);
    }

    public void ShowController()
    {
        controllerWrapper.SetActive(true);
    }

    public void HideGameOverText()
    {
        gameOverText.SetActive(false);
    }

    public void ShowMissText()
    {
        missText.SetActive(true);
    }
    public void HideMissText()
    {
        missText.SetActive(false);
    }



    public void UIInit()
    {
        
        HideClearText();
        HideGameOverText();
        
        Camera cam = Camera.main;
        canvas.worldCamera = cam;
    }
}
