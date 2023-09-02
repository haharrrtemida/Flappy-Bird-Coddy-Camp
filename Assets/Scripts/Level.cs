using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    class Pipe
    {
        private Transform _pipeBodyTransform;
        private Transform _pipeHeadTransform;
        private bool _isBottom;

        public Pipe(Transform pipeBodyTransform, Transform pipeHeadTransform, bool isBottom)
        {
            _pipeBodyTransform = pipeBodyTransform;
            _pipeHeadTransform = pipeHeadTransform;
            _isBottom = isBottom;
        }

        public void Move()
        {
            _pipeBodyTransform.position += Vector3.left * PIPE_SPEED * Time.deltaTime;
            _pipeHeadTransform.position += Vector3.left * PIPE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return _pipeHeadTransform.position.x;
        }

        public bool IsBottom()
        {
            return _isBottom;
        }

        public void DestroyPipe()
        {
            Destroy(_pipeBodyTransform.gameObject);
            Destroy(_pipeHeadTransform.gameObject);
        }
    }

    private const float PIPE_WIDTH = 10.4f;
    private const float PIPE_HEAD_HEIGHT = 5f;
    private const float ORTO_CAM_SIZE = 50f;
    private const float PIPE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION = -115f;
    private const float PIPE_SPAWN_X_POSITION = 100f;
    private const float BIRD_X_POSITION = 0f;

    private GameState _currentState;
    private float _pipeTimer;
    private List<Pipe> _pipeList = new List<Pipe>();

    [SerializeField] private float _pipeTimerMax = 1f;
    [SerializeField] private float _gapSize = 50f;

    public static Level Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _currentState = GameState.WaitingToStart;
    }

    private void Start()
    {
        Bird.Instance.OnDied += Bird_OnDied;
        Bird.Instance.OnStarted += Bird_OnStarted;
        Score.Init();
        Score.OnScoreChanged += Level_OnScoreChanged;
    }


    private void Bird_OnStarted()
    {
        _currentState = GameState.Playing;
    }

    private void Bird_OnDied()
    {
        _currentState = GameState.BirdDied;
    }

    private void Level_OnScoreChanged()
    {
        SoundManager.PlaySound(SoundManager.Sound.Score);
    }

    private void CreateGapPipes(float gapSize, float gapY, float xPosition)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPosition, true);
        CreatePipe(ORTO_CAM_SIZE * 2f - gapY -gapSize * 0.5f, xPosition, false);
    }


    private void HandlerMovement()
    {
        for (int i = 0; i < _pipeList.Count; i++)
        {
            Pipe pipe = _pipeList[i];
            bool isToTheRight = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if (isToTheRight && pipe.GetXPosition() <= BIRD_X_POSITION && pipe.IsBottom())
            {
                Score.ScoreUp();
            }
            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)
            {
                pipe.DestroyPipe();
                _pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void HandlerPipeSpawing()
    {
        _pipeTimer -= Time.deltaTime;
        if (_pipeTimer < 0.1f)
        {
            _pipeTimer = _pipeTimerMax;

            float heightEdgeLimit = 10f;
            float totalHeight = ORTO_CAM_SIZE * 2f;

            float minHeight = _gapSize * 0.5f + heightEdgeLimit;
            float maxHeight = totalHeight - _gapSize * 0.5f - heightEdgeLimit;

            float height = UnityEngine.Random.Range(minHeight, maxHeight);
            CreateGapPipes(_gapSize, height, PIPE_SPAWN_X_POSITION);
        }
    }
    private void Update()
    {
        if(_currentState == GameState.Playing)
        {
            HandlerMovement();
            HandlerPipeSpawing();
        }
    }

    private void CreatePipe(float height, float xPosition, bool isBottom)
    {
        Transform pipeBody = Instantiate(GameAssets.Instance.pipeBody);
        
        float yPos;
        if (isBottom)
        {
            yPos = -ORTO_CAM_SIZE;
        }
        else
        {
            yPos = ORTO_CAM_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        
        pipeBody.position = new Vector2(xPosition, yPos);
        SpriteRenderer pipeSprite = pipeBody.GetComponent<SpriteRenderer>();
        pipeSprite.size = new Vector2(PIPE_WIDTH, height);
        Transform pipeHead = Instantiate(GameAssets.Instance.pipeHead);
        
        if (isBottom) yPos = -ORTO_CAM_SIZE + height - PIPE_HEAD_HEIGHT * 0.5f;
        else yPos = ORTO_CAM_SIZE - height + PIPE_HEAD_HEIGHT * 0.5f;

        pipeHead.position = new Vector3(xPosition, yPos, -1);
        
        BoxCollider2D boxCollider = pipeBody.GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(PIPE_WIDTH, height - PIPE_HEAD_HEIGHT * 0.5f);
        boxCollider.offset = new Vector2(0f, height * 0.5f);

        Pipe pipe = new Pipe(pipeBody, pipeHead, isBottom);
        _pipeList.Add(pipe);
    }

    private void OnDestroy()
    {
        Bird.Instance.OnDied -= Bird_OnDied;
        Bird.Instance.OnStarted -= Bird_OnStarted;
        Score.OnScoreChanged -= Level_OnScoreChanged;
    }
}