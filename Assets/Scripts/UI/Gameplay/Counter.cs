using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [Header("Counter Settings")]
    [SerializeField] private Sprite[] sprites;
        
    private Image image;
    private int _currentIndex = 0;

    private EventBinding<Tick> _tickBinding;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _tickBinding = new EventBinding<Tick>(OnTick);
        EventBus<Tick>.Register(_tickBinding);
    }

    private void OnDisable()
    {
        EventBus<Tick>.Deregister(_tickBinding);
    }

    private void OnTick()
    {
        if (sprites == null || sprites.Length == 0) return;

        _currentIndex = (_currentIndex + 1) % sprites.Length;

        image.sprite = sprites[_currentIndex];
    }
}
