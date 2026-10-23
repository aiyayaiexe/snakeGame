
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public GameObject foodPrefab;
    public SnakeMovement snake;
    public int targetFood = 10;

    private GameObject currentFood;
    private int foodEaten = 0;

    void Start()
    {
        SpawnFood();
    }

    void Update()
    {
        if (currentFood == null || snake == null || snake.IsGameOver)
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

    
    void SpawnFood()
    {
        int x = Random.Range(-7, 8);
        int z = Random.Range(-7, 8);

        Vector3 position = new Vector3(x * 2, 0.5f, z * 2);

        currentFood = Instantiate(
            foodPrefab, position, Quaternion.identity);
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
            Debug.Log("YOU WIN!");
        }
        else
        {
            SpawnFood();
        }
    }
}
