using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    private const float JUMP_SPEED = 100f;

    private GameState _currentState;
    private Rigidbody2D _rigidbodyComponent;

    public event UnityAction OnDied;
    public event UnityAction OnStarted;

    public static Bird Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        _rigidbodyComponent = GetComponent<Rigidbody2D>();
        _rigidbodyComponent.bodyType = RigidbodyType2D.Static;
        _currentState = GameState.WaitingToStart;
    }


    private void Start()
    {
        OnStarted += Bird_OnStarted;
        OnDied += Bird_OnDied;
    }

    private void Bird_OnStarted()
    {
        _currentState = GameState.Playing;
        _rigidbodyComponent.bodyType = RigidbodyType2D.Dynamic;
        Jump();
    }

    private void Bird_OnDied()
    {
        _currentState = GameState.BirdDied;
    }

    private void Jump()
    {
        _rigidbodyComponent.velocity = Vector2.up * JUMP_SPEED;
        SoundManager.PlaySound(SoundManager.Sound.Jump);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            switch (_currentState)
            {
                case GameState.WaitingToStart:
                    OnStarted?.Invoke();
                    break;

                case GameState.Playing:
                    Jump();
                    break;

                case GameState.BirdDied:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _rigidbodyComponent.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Death);
        OnDied?.Invoke();
    }

    private void OnDestroy()
    {
        OnStarted -= Bird_OnStarted;
        OnDied -= Bird_OnDied;
    }
}