using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinMenu : MonoBehaviour
{
    private Animator _anim;
    public List<Image> stars;
    public TMP_Text ScoreText;
    public TMP_Text LeavesText;

    private void OnEnable()
    {
        _anim = GetComponent<Animator>();
    }
    public void PlayWinMenu(int starCount, int score, int leaves)
    {
        for (int i = 0; i < stars.Count; ++i)
        {
            stars[i].color = new Color(0.25f, 0.25f, 0.25f, 0f);
        }
        StartCoroutine(StartShowing(starCount));
        ScoreText.text = score.ToString("N0");
        LeavesText.text = leaves.ToString("N0");
    }

    IEnumerator StartShowing(int count)
    {
        _anim.Play("WinMenu");
        yield return new WaitForSeconds(1f);
        SetStar(count);
        Time.timeScale = 0f;
    }

    private void SetStar(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            stars[i].color = new Color(1, 1, 1, 1);
        }
        for (int i = count; i < stars.Count; ++i)
        {
            stars[i].color = new Color(0.25f, 0.25f, 0.25f, 1f);
        }
    }

}
