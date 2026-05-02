using UnityEngine;
using TMPro;

public class DialogBox : MonoBehaviour
{
    [Header("Dialogue Content")]
    public string[] texts;
    public float[] durations;

    [Header("UI Reference")]
    public TMP_Text dialogueText; // assign your canvas text here

    private int currentIndex = 0;
    private float timer = 0f;

    public GameObject pauseMenuObject;

    PauseMenu pauseMenu;

    public int dialogueID;

    private bool hasStarted = false;
    private bool isPlaying = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenuObject = GameObject.Find("PauseMenu");
        if (pauseMenuObject!=null)
        {
            pauseMenu=pauseMenuObject.GetComponent<PauseMenu>();
        }
         if (texts.Length == 0 || durations.Length == 0)
        {
            Debug.LogWarning("Dialog arrays are empty.");
            return;
        }

        if (texts.Length != durations.Length)
        {
            Debug.LogError("Texts and durations must be the same length.");
            return;
        }

        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasStarted) return;

        if (other.CompareTag("Player"))
        {
            if (pauseMenu != null)
            {
              if (dialogueID == 1 && !pauseMenu.isdialoguebox1done)
                {
                        StartDialogue();
                }
              if (dialogueID == 2 && !pauseMenu.isdialoguebox2done&& pauseMenu.isdialoguebox1done)
                {
                        StartDialogue();
                }
              if (dialogueID == 3&&!pauseMenu.isdialoguebox3done&& pauseMenu.isdialoguebox2done && pauseMenu.isdialoguebox1done)
                {
                        StartDialogue();
                }
            }
            
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying) return;

        timer += Time.deltaTime;

        if (timer >= durations[currentIndex])
        {
            NextDialogue();
        }
    }
    void StartDialogue()
    {
        hasStarted = true;
        currentIndex = 0;
        timer = 0f;
        isPlaying = true;

        dialogueText.text = texts[currentIndex];
    }

    void NextDialogue()
    {
        currentIndex++;
        timer = 0f;

        if (currentIndex >= texts.Length)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = texts[currentIndex];
    }

    void EndDialogue()
    {
        isPlaying = false;
        
        dialogueText.text = ""; // clear text (optional)
        if (pauseMenu != null)
        {
        if (dialogueID == 1)
            pauseMenu.isdialoguebox1done = true;
        else if (dialogueID == 2)
            pauseMenu.isdialoguebox2done = true;
        else if (dialogueID == 3)
            pauseMenu.isdialoguebox3done = true;
        }
    }
    
}
