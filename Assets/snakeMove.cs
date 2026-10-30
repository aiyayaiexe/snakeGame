
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
        ResetSnake(startingLength);
        gameActive = false;
    }

    void Update()
    {
        if (!gameActive || gameOver)
        {
            return;
        }

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

        if (Mathf.Abs(newPosition.x) > gridLimit ||
            Mathf.Abs(newPosition.z) > gridLimit)
        {
            LoseGame("You hit the wall!");
            return;
        }

        foreach (Transform segment in body)
        {
            if (Vector3.Distance(
                newPosition, segment.position) < 0.1f)
            {
                LoseGame("You hit yourself!");
                return;
            }
        }

        positions.Insert(0, newPosition);
        transform.position = newPosition;

        for (int i = 0; i < body.Count; i++)
        {
            body[i].position = positions[i + 1];
        }

        positions.RemoveAt(positions.Count - 1);
    }

    public void Grow()
    {
        if (gameOver)
        {
            return;
        }

        for (int i = 0; i < growthPerFruit; i++)
        {
            Vector3 lastPosition =
                positions[positions.Count - 1];

            GameObject segment = Instantiate(
                bodyPrefab, lastPosition, Quaternion.identity);

            body.Add(segment.transform);
            positions.Add(lastPosition);
        }
    }

    public void BeginLevel(int newLength)
    {
        ResetSnake(newLength);
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

        foreach (Transform segment in body)
        {
            Destroy(segment.gameObject);
        }

        body.Clear();
        positions.Clear();

        transform.position = startPosition;
        positions.Add(startPosition);

        for (int i = 0; i < newLength; i++)
        {
            Vector3 position = startPosition
                + Vector3.back * gridSize * (i + 1);

            GameObject segment = Instantiate(
                bodyPrefab, position, Quaternion.identity);

            body.Add(segment.transform);
            positions.Add(position);
        }
    }

    void LoseGame(string reason)
    {
        gameOver = true;
        gameActive = false;

        Debug.Log("GAME OVER! " + reason);

        gameManager.SnakeDied();
    }
}
