using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public GameObject[] foodPrefabs;   // element 0 = level 1, element 1 = level 2
    public SnakeMovement snake;
    public GameManager gameManager;

    public int targetFood = 10;

    private GameObject currentFood;
    private int foodEaten = 0;
    private bool foodActive = false;
    private int currentLevel = 1;

    void Update()
    {
        if (!foodActive || currentFood == null ||
            snake == null || snake.IsGameOver)
        {
            return;
        }

        Vector3 head = snake.transform.position;
        Vector3 food = currentFood.transform.position;

        if (Mathf.Abs(head.x - food.x) < 0.5f &&
            Mathf.Abs(head.z - food.z) < 0.5f)
        {
            EatFood();
        }
    }

    public void BeginFood(int amount, int level)
    {
        StopFood();

        currentLevel = level;
        targetFood = amount;
        foodEaten = 0;
        foodActive = true;

        SpawnFood();
    }

    public void StopFood()
    {
        foodActive = false;

        if (currentFood != null)
        {
            Destroy(currentFood);
            currentFood = null;
        }
    }

    void SpawnFood()
    {
        if (foodPrefabs == null || foodPrefabs.Length == 0)
        {
            Debug.LogError("FoodManager: assign Food Prefabs in the Inspector!");
            return;
        }

        int x = Random.Range(-7, 8);
        int z = Random.Range(-7, 8);

        Vector3 position = new Vector3(x * 2, 0.5f, z * 2);

        // Pick the food prefab for the current level
        GameObject prefab = foodPrefabs[
            Mathf.Clamp(currentLevel - 1, 0, foodPrefabs.Length - 1)];

        currentFood = Instantiate(
            prefab, position, prefab.transform.rotation);
    }

    void EatFood()
    {
        Destroy(currentFood);
        currentFood = null;

        snake.Grow();
        foodEaten++;

        Debug.Log("Fruit eaten: " + foodEaten + " / " + targetFood);

        if (foodEaten >= targetFood)
        {
            StopFood();
            gameManager.LevelCompleted();
        }
        else
        {
            SpawnFood();
        }
    }
}