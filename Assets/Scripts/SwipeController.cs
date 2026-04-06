using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] int maxPage;
    public int currentPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;

    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;
    float dragThreshould;

    [SerializeField] Button nextBtn, previousBtn;

    public TMP_Text levelName;
    public TMP_Text levelScore;
    public TMP_Text levelDifficulty;
    public Button play;

    private sceneManager sceneManager;
    private Leaderboard leaderboard;

    void Start()
    {
        sceneManager = FindObjectOfType<sceneManager>();
        leaderboard = FindObjectOfType<Leaderboard>();
    }

    private void Awake()
    {
        targetPos = levelPagesRect.localPosition;
        dragThreshould = Screen.width/15;

        currentPage = 1;
        UpdateButton();
        PageDetail();
    }

    public void Next()
    {
        if(currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
            PageDetail();
            leaderboard.DisplayLeaderboard();
        }
    }
    public void Previous() 
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
            PageDetail();
            leaderboard.DisplayLeaderboard();
        }
    }


    void MovePage()
    {
        levelPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
        audioManager.instance.PlaySFX("Button");
        UpdateButton();
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        if(Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshould)
        {
            if (eventData.position.x > eventData.pressPosition.x) 
            { 
                Previous(); 
            }
            else
            {
                Next();
            }
        }
        else
        {
            MovePage();
        }
    }

    void UpdateButton()
    {
        nextBtn.interactable = true;
        previousBtn.interactable = true;

        if (currentPage == 1)
        {
            previousBtn.interactable = false;
        }
        else if (currentPage == maxPage)
        {
            nextBtn.interactable = false;
        }
    }

    private void PageDetail()
    {

        switch (currentPage)
        {
            case 1:
                levelName.text = "Forest";

                if (PlayerPrefs.HasKey("ForestScore"))
                    levelScore.text = "score: " + PlayerPrefs.GetInt("ForestScore");
                else
                    levelScore.text = "Score: " + 0;

                levelDifficulty.text = "Easy";
                play.onClick.RemoveAllListeners();
                play.onClick.AddListener(() => sceneManager.ChangeScene("ForestLevel"));
                break;

            case 2:
                levelName.text = "Desert";

                if (PlayerPrefs.HasKey("DesertScore"))
                    levelScore.text = "score: " + PlayerPrefs.GetInt("DesertScore");
                else
                    levelScore.text = "Score: " + 0;

                levelDifficulty.text = "Normal";
                play.onClick.RemoveAllListeners();
                play.onClick.AddListener(() => sceneManager.ChangeScene("DesertLevel"));
                break;

            case 3:
                levelName.text = "Glitch";

                if (PlayerPrefs.HasKey("GlitchScore"))
                    levelScore.text = "score: " + PlayerPrefs.GetInt("GlitchScore");
                else
                    levelScore.text = "Score: " + 0;

                levelDifficulty.text = "Hard";
                play.onClick.RemoveAllListeners();
                play.onClick.AddListener(() => sceneManager.ChangeScene("GlitchLevel"));
                break;
        }
    }
}
