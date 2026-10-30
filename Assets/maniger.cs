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
    public GameObject creditsScreen;

    public VideoPlayer videoPlayer;

    // File names of the videos inside Assets/StreamingAssets
    public string level1VideoFile = "CutScene1_Final.mp4";
    public string level2VideoFile = "Cutscene2_Final.mp4";
    public string finalVideoFile = "CutScene3_Final.mp4";

    public int foodNeeded = 10;

    // Background materials (one per level)
    public Material[] levelBackgrounds;
    public Renderer backgroundRenderer;

    // Music
    public AudioSource musicSource;   // plays all the time
    public AudioSource deathSource;   // plays only when you die

    private int currentLevel = 0;
    private bool finalVideoPlaying = false;

    void Start()
    {
        startScreen.SetActive(true);
        videoScreen.SetActive(false);
        diedScreen.SetActive(false);
        endScreen.SetActive(false);
        creditsScreen.SetActive(false);  


        snake.StopGame();
        foodManager.StopFood();

        videoPlayer.loopPointReached += VideoFinished;
        videoPlayer.prepareCompleted += VideoPrepared;
        videoPlayer.errorReceived += VideoError;
    }

    void OnDestroy()
    {
        videoPlayer.loopPointReached -= VideoFinished;
        videoPlayer.prepareCompleted -= VideoPrepared;
        videoPlayer.errorReceived -= VideoError;
    }

    public void StartGame()
    {
        StartLevel(1);
    }

    public void ShowCredits()
    {
        creditsScreen.SetActive(true);
    }

    public void HideCredits()
    {
        creditsScreen.SetActive(false);
    }

    public void GoToTitle()
    {
        videoPlayer.Stop();
        foodManager.StopFood();

        // Stop the death song and bring the main music back
        if (deathSource != null)
            deathSource.Stop();

        if (musicSource != null)
            musicSource.UnPause();

        // Reset everything back to how it is when the game first loads
        currentLevel = 0;
        finalVideoPlaying = false;
        ChangeBackground(1);

        snake.BeginLevel(2, 1);
        snake.StopGame();

        startScreen.SetActive(true);
        videoScreen.SetActive(false);
        diedScreen.SetActive(false);
        endScreen.SetActive(false);

        if (creditsScreen != null)
            creditsScreen.SetActive(false);
    }

    void StartLevel(int level)
    {
        currentLevel = level;

        // Stop the death song and bring the main music back
        if (deathSource != null)
            deathSource.Stop();

        if (musicSource != null)
            musicSource.UnPause();

        // Change background for this level
        ChangeBackground(level);

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

        snake.BeginLevel(snakeLength, level);

        if (level == 3)
        {
            foodManager.StopFood();
        }
        else
        {
            foodManager.BeginFood(foodNeeded, level);
        }

        Debug.Log("Starting Level " + level);
    }

    // Changes the background material
    public void ChangeBackground(int level)
    {
        if (level >= 1 && level <= levelBackgrounds.Length)
        {
            backgroundRenderer.material = levelBackgrounds[level - 1];
        }
    }

    public void LevelCompleted()
    {
        snake.StopGame();

        if (currentLevel == 1)
        {
            PlayVideo(level1VideoFile, false);
        }
        else if (currentLevel == 2)
        {
            PlayVideo(level2VideoFile, false);
        }
    }

    public void SnakeDied()
    {
        foodManager.StopFood();

        // Pause the main music and play the death song
        if (musicSource != null)
            musicSource.Pause();

        if (deathSource != null)
            deathSource.Play();

        if (currentLevel == 3)
        {
            PlayVideo(finalVideoFile, true);
        }
        else
        {
            diedScreen.SetActive(true);
        }
    }

    void PlayVideo(string fileName, bool isFinal)
    {
        snake.StopGame();
        foodManager.StopFood();

        finalVideoPlaying = isFinal;

        videoScreen.SetActive(true);
        startScreen.SetActive(false);
        diedScreen.SetActive(false);
        endScreen.SetActive(false);

        videoPlayer.Stop();

        // Stream the video from StreamingAssets (works in WebGL)
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/" + fileName;
        videoPlayer.isLooping = false;
        videoPlayer.Prepare();
    }

    // Called when the video is loaded and ready
    void VideoPrepared(VideoPlayer player)
    {
        player.Play();
    }

    // If a video fails to load, log it and keep the game moving
    void VideoError(VideoPlayer player, string message)
    {
        Debug.LogError("Video error: " + message);
        VideoFinished(player);
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