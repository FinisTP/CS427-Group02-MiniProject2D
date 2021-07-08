using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueManager : MonoBehaviour
{
	public TMP_Text dialogueText;

	public int SceneIdAfterDialogue = -1;

	public Animator animator;
	public Image BackgroundImage;
	public Image CharacterImage;

	private Queue<Dialogue> dialogues;
	private string currentSentence;
	private string npcName = "";

	private GameManager_ manager;

	private bool isTyping = false;
	// Use this for initialization
	void Start()
	{
		dialogues = new Queue<Dialogue>();
		manager = GameObject.FindObjectOfType<GameManager_>();
	}

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
			DisplayNextSentence();
        }
    }

    public void StartDialogue(DialogueScriptable dialogueHolder)
	{
		// animator.SetBool("IsOpen", true);

		dialogues.Clear();

		foreach (Dialogue dialogue in dialogueHolder.DialogueList)
		{
			dialogues.Enqueue(dialogue);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		if (!isTyping)
        {
			if (dialogues.Count == 0)
			{
				EndDialogue();
				return;
			}
			Dialogue dia = dialogues.Dequeue();
			currentSentence = dia.sentence;
			if (dia.background != null)
			BackgroundImage.sprite = dia.background;
			if (dia.npcSprite != null)
			CharacterImage.sprite = dia.npcSprite;
			if (dia.name != "") npcName = dia.name;
			StopAllCoroutines();
			StartCoroutine(TypeSentence());
			isTyping = true;
		} else
        {
			StopAllCoroutines();
			dialogueText.text = currentSentence;
			isTyping = false;
        }
		
	}

	IEnumerator TypeSentence()
	{
		dialogueText.text = "";
		char[] letters = currentSentence.ToCharArray();
		for (int i = 0; i < letters.Length; ++i)
		{
			dialogueText.text += letters[i];
			yield return new WaitForSeconds(0.05f);
			if (i % 2 == 0) GameManager_.Instance.SoundPlayer.PlayClip("Blip", 0.2f);
		}
		isTyping = false;
	}

	public void EndDialogue()
	{
		// animator.SetBool("IsOpen", false);
		if (SceneIdAfterDialogue == -1)
		FindObjectOfType<GameManager_>().LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
		else FindObjectOfType<GameManager_>().LoadLevel(SceneIdAfterDialogue);
	}

}