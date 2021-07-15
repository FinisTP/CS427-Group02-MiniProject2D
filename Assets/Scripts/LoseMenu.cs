using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoseMenu : MonoBehaviour
{
    private Animator _anim;
    public TMP_Text ScoreText;
    public TMP_Text LeavesText;

    private void OnEnable()
    {
        _anim = GetComponent<Animator>();
    }
    public void PlayLoseMenu(int score, int leaves)
    {
        StartCoroutine(StartShowing());
        ScoreText.text = score.ToString("N0");
        LeavesText.text = leaves.ToString("N0");
    }

    IEnumerator StartShowing()
    {
        _anim.Play("LoseMenu");
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
    }
}
