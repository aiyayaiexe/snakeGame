
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public SnakeMovement snake;
    public FoodManager foodManager;

    public GameObject startScreen;
    public GameObject videoScreen;
    public GameObject diedScreen;
    public GameObject endScreen;

    public VideoPlayer videoPlayer;

    public VideoClip level1Video;
    public VideoClip level2Video;
    public VideoClip finalVideo;

    public int foodNeeded = 10;

    private int currentLevel = 0;
    private bool finalVideoPlaying = false;

    void Start()
    {
        startScreen.SetActive(true);
        videoScreen.SetActive(false);
        diedScreen.SetActive(false);
        endScreen.SetActive(false);

        snake.StopGame();
        foodManager.StopFood();

        videoPlayer.loopPointReached += VideoFinished;
    }

    void OnDestroy()
    {
        videoPlayer.loopPointReached -= VideoFinished;
    }

    public void StartGame()
    {
        StartLevel(1);
    }

    void StartLevel(int level)
    {
        currentLevel = level;

        startScreen.SetActive(false);
        videoScreen.SetActive(false);
        diedScreen.SetActive(false);
        endScreen.SetActive(false);

        videoPlayer.Stop();

        int snakeLength = 2;

        if (level == 2)
        {
            snakeLength = 5;
        }
        else if (level == 3)
        {
            snakeLength = 8;
        }

        snake.BeginLevel(snakeLength);

        if (level == 3)
        {
            foodManager.StopFood();
        }
        else
        {
            foodManager.BeginFood(foodNeeded);
        }

        Debug.Log("Starting Level " + level);
    }

    public void LevelCompleted()
    {
        snake.StopGame();

        if (currentLevel == 1)
        {
            PlayVideo(level1Video, false);
        }
        else if (currentLevel == 2)
        {
            PlayVideo(level2Video, false);
        }
    }

    public void SnakeDied()
    {
        foodManager.StopFood();

        if (currentLevel == 3)
        {
            PlayVideo(finalVideo, true);
        }
        else
        {
            diedScreen.SetActive(true);
        }
    }

    void PlayVideo(VideoClip clip, bool isFinal)
    {
        snake.StopGame();
        foodManager.StopFood();

        finalVideoPlaying = isFinal;

        videoScreen.SetActive(true);
        startScreen.SetActive(false);
        diedScreen.SetActive(false);
        endScreen.SetActive(false);

        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }

    void VideoFinished(VideoPlayer player)
    {
        videoScreen.SetActive(false);

        if (finalVideoPlaying)
        {
            endScreen.SetActive(true);
            finalVideoPlaying = false;
        }
        else if (currentLevel == 1)
        {
            StartLevel(2);
        }
        else if (currentLevel == 2)
        {
            StartLevel(3);
        }
    }

    public void RetryLevel()
    {
        StartLevel(currentLevel);
    }
}
