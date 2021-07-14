using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Gradient lightColor;
    private Light2D light2D;
    [SerializeField] GameObject PlayerLight;

    private int days;
    public int Days => days;
    [SerializeField] private float time = 0;
    private bool canChangeDay = true;
    public delegate void OnDayChanged();
    public OnDayChanged DayChanged;
    public float TimeFlow = 2f;

    public float RainStartTime = 100;
    public AudioSource RainSound;
    public float RainStopTime = 200;
    public ParticleSystem RainParticle;

    private void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    private void Update()
    {
        if (!GameManager_.Instance.IsRunningGame) return;
        else PlayerLight = GameManager_.Instance.Player.GetComponentInChildren<Light2D>().gameObject;
        if (time > 500)
        {
            time = 0;
        }
        if ((int)time == 250 && canChangeDay)
        {
            canChangeDay = false;
            if (DayChanged != null) DayChanged();
            days++;
        }
        if ((int)time == 100)
        {
            RainSound.Play();
            RainParticle.Play();
        }
        if ((int)time == 200)
        {
            RainSound.Stop();
            RainParticle.Stop();
        }

        // night
        if (time >= 180 && time <= 320)
        {
            light2D.intensity = (1 / 4900f) * time * time - (5 / 49f) * time + (625 / 49f);
            PlayerLight.GetComponent<Light2D>().enabled = true;
        } else
        {
            PlayerLight.GetComponent<Light2D>().enabled = false;
            light2D.intensity = 1;
        }

        if ((int)time == 255)
        {
            canChangeDay = true;
        }
        time += Time.deltaTime * TimeFlow;
        light2D.color = lightColor.Evaluate(time * 0.002f);
    }

}
