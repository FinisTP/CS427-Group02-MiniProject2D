using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Level Stat", menuName="Level")]
public class LevelStatistics : ScriptableObject
{
    public int sceneId;
    public PlayerProgression[] Progression;
    public int MaxLive;
    public Sprite[] StageSprites;
    public string CharacterName;
    public Sprite HeartIcon;
    public Vector2 LevelTopLeft;
    public Vector2 LevelBottomRight;
}