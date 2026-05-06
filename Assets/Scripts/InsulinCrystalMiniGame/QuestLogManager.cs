using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestLogManager : MonoBehaviour
{
    public static QuestLogManager Instance { get; private set; }
    [SerializeField] 

    private int currentBeat = 0;
    [SerializeField]
    private Text header;
    [SerializeField]
    private Text subtext;
    [SerializeField]
    private Text header_menu;
    [SerializeField]
    private Text subtext_menu;
    [SerializeField]
    private Text progressText; 
    [SerializeField]
    private QuestLogData questLogData;
    [SerializeField]
    private Image checkbox;
    [SerializeField]
    private Sprite[] checkboxSprites;
    [SerializeField]
    private QuestLogGameObject questLogGameObject;

    public void Awake(){
        Instance = this;
    }

    public void Start()
    {
        UpdateQuestLog();
    }

    public void NextBeat(){
        currentBeat++;
        UpdateQuestLog();
    }

    public void SetBeat(int beat)
    {
        currentBeat = beat;
        UpdateQuestLog();
    }

    public void UpdateQuestLog(){
        questLogGameObject.SetMenuState(true);
        header.text = questLogData.questlog[currentBeat].header;
        subtext.text = questLogData.questlog[currentBeat].subtext;
        header_menu.text = questLogData.questlog[currentBeat].header;
        subtext_menu.text = questLogData.questlog[currentBeat].subtext;
        progressText.text = questLogData.questlog[currentBeat].progressIndex + "/" + questLogData.questlog[currentBeat].progressCount;
        checkbox.sprite = checkboxSprites[Convert.ToInt32(questLogData.questlog[currentBeat].complete)];
    }
    public void UpdateProgressIndex(int index, int max) {
        if (index > 9) {
            progressText.text = index + "/" + "\n" + max;

        }
        else {
            progressText.text = index + "/" + max;

        }
        header_menu.text = questLogData.questlog[currentBeat].header;
        subtext_menu.text = questLogData.questlog[currentBeat].subtext + "\n" + "\n" + 
            "Hexamers placed in current layer:" + "\n" + 
            index + "/" + max;
    }



}
