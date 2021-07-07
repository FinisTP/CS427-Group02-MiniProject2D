using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteShadow : MonoBehaviour
{
    public Vector2 offset = new Vector2(-0.2f, -0.2f);
    private SpriteRenderer caster;
    private SpriteRenderer shadow;

    private Transform transCaster;
    private Transform transShadow;

    public Material shadowMaterial;
    public Color shadowColor;

    private void Start()
    {
        transCaster = transform;
        transShadow = new GameObject().transform;
        transShadow.localScale = new Vector3(transCaster.localScale.x, 0.5f  , 0f);
        transShadow.parent = transCaster;
        transShadow.gameObject.name = "Shadow";
        transShadow.localRotation = Quaternion.identity;
        

        caster = GetComponent<SpriteRenderer>();
        shadow = transShadow.gameObject.AddComponent<SpriteRenderer>();

        shadow.material = shadowMaterial;
        shadow.color = shadowColor;
        shadow.sortingLayerName = caster.sortingLayerName;
        shadow.sortingOrder = caster.sortingOrder - 1;

    }

    private void LateUpdate()
    {
        transShadow.position = new Vector2(transCaster.position.x + offset.x,
            transCaster.position.y + offset.y);

        
        shadow.sprite = caster.sprite;
    }
}
