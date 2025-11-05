using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Dialogue : MonoBehaviour
{

    public TextMeshProUGUI textComponent;
    public string[] lines;  // collection of dialogue want to display
    public float textSpeed;  // want to type out characters at specific speed

    private int index;  // index to track where in speech at

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))  // if return button pushed
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {   
                // If player clicks again before text finished displaying it'll immediately show all the text
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    // Create a coroutine
    IEnumerator TypeLine()
    {
        // Type out each character 1 by 1
        foreach (char letter in lines[index].ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    // Moves to next line of dialogue
    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);  // sets game object inactive
        }
    }
}
