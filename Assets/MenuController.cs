using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void LoadLevel (int levelId)
    {
        GameObject.FindObjectOfType<GameManager_>().LoadLevel(levelId);
    }
}
