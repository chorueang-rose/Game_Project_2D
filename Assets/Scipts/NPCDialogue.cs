using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public List<string> dialogueLines = new List<string>();

    private GameObject dialogueBox;
    private TextMeshProUGUI dialogueText;

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool talking = false;

    void Start()
    {
        dialogueBox = GameObject.FindWithTag("DialogueBox");
        dialogueText = GameObject.FindWithTag("DialogueText").GetComponent<TextMeshProUGUI>();

        dialogueBox.SetActive(false);
    }
    void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            if(!talking)
            {
                StartDialogue();
            }
            else
            {
                NextLine();
            }
        }
    }

    void StartDialogue()
    {
        talking = true;
        currentLine = 0;

        dialogueBox.SetActive(true);
        dialogueText.text = dialogueLines[currentLine];
    }

    void NextLine()
    {
        currentLine++;

        if(currentLine >= dialogueLines.Count)
        {
            EndDialogue();
        }
        else
        {
            dialogueText.text = dialogueLines[currentLine];
        }
    }

    void EndDialogue()
    {
        talking = false;
        dialogueBox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = false;
            EndDialogue();
        }
    }
}

