using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{

	public string name = "";
	public Sprite npcSprite = null;
	public Sprite background = null;
	public AudioClip BGM;

	[TextArea(3, 10)]
	public string sentence;

}
