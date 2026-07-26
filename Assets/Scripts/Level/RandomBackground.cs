using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomBackground : MonoBehaviour
{
    [SerializeField] private Sprite background1;
    [SerializeField] private Sprite background2;

    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        spriteRenderer.sprite = SetRandomSprite();
    }

    private Sprite SetRandomSprite()
    {
        var roll = UnityEngine.Random.value;
        return roll > 0.5f ? background1 : background2;
    }
    
}
