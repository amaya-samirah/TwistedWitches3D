using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;  // content in scroll list
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    // ---TESTING PURPOSES--- //
    // public Quest testQuest;
    // public int testQuestAmount;
    // private List<QuestProgress> testQuests = new();
    // --------------------- //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ---TESTING PURPOSES--- //
        // for (int i = 0; i < testQuestAmount; i++)
        // {
        //     testQuests.Add(new QuestProgress(testQuest));
        // }
        // --------------------- //

        UpdateQuestUI();
    }

    // Call whenever change in quest progress
    public void UpdateQuestUI()
    {
        // Destroy any existing quest entries
        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        // Build quest entries
        foreach (var quest in QuestController.Instance.activeQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);  // parents questEntryPrefab to questListContent
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            //TMP_Text questNameText = entry.transform.GetChild(0).GetComponent<TMP_Text>();  // don't want to use 'Find' in game loop
            Transform objectiveList = entry.transform.GetChild(1);

            questNameText.text = quest.quest.questName;  // [quest that's in progress].[quest details from said quest].[said quest's name]

            foreach (var objective in quest.objectives)
            {
                GameObject objTextGameObject = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGameObject.GetComponent<TMP_Text>();

                objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})";  // ex. Collect 1 Sunflower (0/1)
            }
        }
    }
}
