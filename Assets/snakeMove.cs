
using UnityEngine;
using System.Collections.Generic;

public class SnakeMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 2f;
    public GameObject bodyPrefab;
    public int startingLength = 2;
    public int growthPerFruit = 3;
    public float gridLimit = 14f;

    public GameManager gameManager;

    public SpriteRenderer headRenderer;

    // Level 1 head sprites
    public Sprite headL1Up;
    public Sprite headL1Down;
    public Sprite headL1Left;
    public Sprite headL1Right;

    // Level 2 head sprites
    public Sprite headL2Up;
    public Sprite headL2Down;
    public Sprite headL2Left;
    public Sprite headL2Right;

    // Level 3 head sprites
    public Sprite headL3Up;
    public Sprite headL3Down;
    public Sprite headL3Left;
    public Sprite headL3Right;

    // Body straight sprites
    public Sprite bodyVertical;
    public Sprite bodyHorizontal;

    // Body corner sprites
    public Sprite bodyUpRight;
    public Sprite bodyUpLeft;
    public Sprite bodyDownRight;
    public Sprite bodyDownLeft;

    public Sprite tailDown;
    public Sprite tailLeft;
    public Sprite tailUp;
    public Sprite tailRight;

    private int currentLevel = 1;

    private Vector3 direction = Vector3.forward;
    private Vector3 startPosition;
    private float timer;

    private List<Transform> body = new List<Transform>();
    private List<Vector3> positions = new List<Vector3>();

    private bool gameOver = false;
    private bool gameActive = false;

    public bool IsGameOver
    {
        get { return gameOver; }
    }

    void Start()
    {
        startPosition = transform.position;

        if (headRenderer == null)
            headRenderer = GetComponent<SpriteRenderer>();

        ResetSnake(startingLength);
        UpdateHeadSprite();

        gameActive = false;
    }

    void Update()
    {
        if (!gameActive || gameOver)
            return;

        // Movement controls
        if (Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W))
        {
            if (direction != Vector3.back)
                direction = Vector3.forward;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                 Input.GetKeyDown(KeyCode.S))
        {
            if (direction != Vector3.forward)
                direction = Vector3.back;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) ||
                 Input.GetKeyDown(KeyCode.A))
        {
            if (direction != Vector3.right)
                direction = Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) ||
                 Input.GetKeyDown(KeyCode.D))
        {
            if (direction != Vector3.left)
                direction = Vector3.right;
        }

        UpdateHeadSprite();

        timer += Time.deltaTime;

        if (timer >= 1f / moveSpeed)
        {
            MoveSnake();
            timer = 0f;
        }
    }

    void MoveSnake()
    {
        Vector3 newPosition =
            transform.position + direction * gridSize;

        // Wall collision
        if (Mathf.Abs(newPosition.x) > gridLimit ||
            Mathf.Abs(newPosition.z) > gridLimit)
        {
            LoseGame("You hit the wall!");
            return;
        }

        // Self collision
        foreach (Transform segment in body)
        {
            if (Vector3.Distance(
                newPosition, segment.position) < 0.1f)
            {
                LoseGame("You hit yourself!");
                return;
            }
        }

        // Remember the new head position
        positions.Insert(0, newPosition);

        transform.position = newPosition;

        // Move each body segment
        for (int i = 0; i < body.Count; i++)
        {
            body[i].position = positions[i + 1];
        }

        // Remove the last position
        positions.RemoveAt(positions.Count - 1);

        // Update body sprites after moving
        UpdateBodySprites();
    }

    public void Grow()
    {
        if (gameOver)
            return;

        if (positions.Count == 0)
            return;

        for (int i = 0; i < growthPerFruit; i++)
        {
            Vector3 lastPosition =
                positions[positions.Count - 1];

            // Use the prefab's rotation
            GameObject segment = Instantiate(
                bodyPrefab,
                lastPosition,
                bodyPrefab.transform.rotation
            );

            body.Add(segment.transform);
            positions.Add(lastPosition);
        }

        UpdateBodySprites();
    }

    public void BeginLevel(int newLength, int level)
    {
        currentLevel = level;

        ResetSnake(newLength);
        UpdateHeadSprite();

        gameActive = true;
    }

    public void StopGame()
    {
        gameActive = false;
    }

    void ResetSnake(int newLength)
    {
        gameActive = false;
        gameOver = false;
        timer = 0f;
        direction = Vector3.forward;

        // Remove old body segments
        foreach (Transform segment in body)
        {
            Destroy(segment.gameObject);
        }

        body.Clear();
        positions.Clear();

        // Reset head
        transform.position = startPosition;
        positions.Add(startPosition);

        // Create starting body
        for (int i = 0; i < newLength; i++)
        {
            Vector3 position = startPosition
                + Vector3.back * gridSize * (i + 1);

            GameObject segment = Instantiate(
                bodyPrefab,
                position,
                bodyPrefab.transform.rotation
            );

            body.Add(segment.transform);
            positions.Add(position);
        }

        UpdateBodySprites();
    }

    void UpdateBodySprites()
    {
        for (int i = 0; i < body.Count; i++)
        {
            SpriteRenderer sr =
                body[i].GetComponent<SpriteRenderer>();

            if (sr == null)
                continue;

            // Direction toward the head
            Vector3 toHead =
                positions[i] - positions[i + 1];

            // The last segment has no segment behind it
            if (i + 2 >= positions.Count)
            {
                if (Mathf.Abs(toHead.x) < 0.1f)
                    sr.sprite = bodyVertical;
                else
                    sr.sprite = bodyHorizontal;

                continue;
            }

            // Direction toward the tail
            Vector3 toTail =
                positions[i + 2] - positions[i + 1];

            // Straight vertical
            if (Mathf.Abs(toHead.x) < 0.1f &&
                Mathf.Abs(toTail.x) < 0.1f)
            {
                sr.sprite = bodyVertical;
            }

            // Straight horizontal
            else if (Mathf.Abs(toHead.z) < 0.1f &&
                     Mathf.Abs(toTail.z) < 0.1f)
            {
                sr.sprite = bodyHorizontal;
            }

            // Corner pieces
            else
            {
                bool up =
                    toHead.z > 0 || toTail.z > 0;

                bool down =
                    toHead.z < 0 || toTail.z < 0;

                bool left =
                    toHead.x < 0 || toTail.x < 0;

                bool right =
                    toHead.x > 0 || toTail.x > 0;

                if (up && right)
                    sr.sprite = bodyUpRight;

                else if (up && left)
                    sr.sprite = bodyUpLeft;

                else if (down && right)
                    sr.sprite = bodyDownRight;

                else if (down && left)
                    sr.sprite = bodyDownLeft;
            }
        }

        // Change the tail sprite
    if (body.Count > 0)
    {
        int last = body.Count - 1;

        Vector3 tailDirection = body[last].transform.position -
                                body[last - 1].transform.position;

        SpriteRenderer tailRenderer =
            body[last].GetComponent<SpriteRenderer>();

        if (tailDirection.z > 0)
            tailRenderer.sprite = tailUp;

        else if (tailDirection.z < 0)
            tailRenderer.sprite = tailDown;

        else if (tailDirection.x > 0)
            tailRenderer.sprite = tailRight;

        else if (tailDirection.x < 0)
            tailRenderer.sprite = tailLeft;
    }


        }

    void LoseGame(string reason)
    {
        gameOver = true;
        gameActive = false;

        Debug.Log("GAME OVER! " + reason);

        if (gameManager != null)
            gameManager.SnakeDied();
    }

    void UpdateHeadSprite()
    {
        if (headRenderer == null)
            return;

        if (currentLevel == 1)
        {
            if (direction == Vector3.forward)
                headRenderer.sprite = headL1Up;
            else if (direction == Vector3.back)
                headRenderer.sprite = headL1Down;
            else if (direction == Vector3.left)
                headRenderer.sprite = headL1Left;
            else if (direction == Vector3.right)
                headRenderer.sprite = headL1Right;
        }
        else if (currentLevel == 2)
        {
            if (direction == Vector3.forward)
                headRenderer.sprite = headL2Up;
            else if (direction == Vector3.back)
                headRenderer.sprite = headL2Down;
            else if (direction == Vector3.left)
                headRenderer.sprite = headL2Left;
            else if (direction == Vector3.right)
                headRenderer.sprite = headL2Right;
        }
        else if (currentLevel == 3)
        {
            if (direction == Vector3.forward)
                headRenderer.sprite = headL3Up;
            else if (direction == Vector3.back)
                headRenderer.sprite = headL3Down;
            else if (direction == Vector3.left)
                headRenderer.sprite = headL3Left;
            else if (direction == Vector3.right)
                headRenderer.sprite = headL3Right;
        }
    }
}
