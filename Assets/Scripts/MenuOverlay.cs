using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOverlay : MonoBehaviour
{
    [SerializeField] Gradient colorProgression;
    public float MaxTime = 50;
    public Image overlayImage;
    private float currentTime = 0;
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= 100) currentTime -= MaxTime;
        overlayImage.color = colorProgression.Evaluate(currentTime / MaxTime);
    }
}
