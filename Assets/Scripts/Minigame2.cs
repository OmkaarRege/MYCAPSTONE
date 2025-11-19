using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
public class Minigame2 : MonoBehaviour
{
    public GameObject heart,timerGO,scoreGO;

    public float minScale = 3f;
    public float maxScale = 4f;
    public float speed = 2f; // heart pulse speed

    public float moveSpeed,score,timer;

    public GameObject[] healthyfood;
    public GameObject[] unhealthfood;
    public bool scoreupdated =false;

    public List<GameObject> moving = new List<GameObject>(); // changed to List

    private float nextSpawnTime = 0f;
    public float spawnInterval = 3f; // 2 seconds
    private bool spawnHealthyNext = true; // alternates each spawn

    void Start()
    {
        score=0;
        timer=25;
    }

    void Update()
    {
        // Heart pulse
        float scale = Mathf.PingPong(Time.time * speed, maxScale - minScale) + minScale;
        heart.transform.localScale = new Vector3(scale, scale, 1f);

        for (int i = moving.Count - 1; i >= 0; i--)
        
        {
        GameObject obj = moving[i];
        if (obj == null)
        {
        moving.RemoveAt(i); // clean up destroyed objects
        continue;
        }

         obj.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        if (obj.transform.position.x < -12f)
        {
            Destroy(obj);
            moving.RemoveAt(i);
        }
        }

        // Spawn food at intervals
        if (Time.time >= nextSpawnTime)
        {
            GameObject spawnedObject = null;

            if (spawnHealthyNext)
                spawnedObject = SpawnHealthyFood();
            else
                spawnedObject = SpawnUnhealthyFood();

            // Add spawned object to moving list
            if (spawnedObject != null)
                moving.Add(spawnedObject);

            // Toggle for next spawn
            spawnHealthyNext = !spawnHealthyNext;

            // Reset timer
            nextSpawnTime = Time.time + spawnInterval;
        }
        GameObject canvas = GameObject.Find("Minigame2 Canvas");
        if (canvas != null)
        {
           

           // Get the Text component
           TMP_Text scoreText = scoreGO.GetComponent<TMP_Text>();
           TMP_Text timerText = timerGO.GetComponent<TMP_Text>();

           
           scoreText.text = "Score "+score.ToString() + "/3";

           
           timerText.text = "Time left "+Mathf.CeilToInt(timer).ToString();
        }


        // Countdown timer
        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0f);
       

        if (timer <= 0f)
        {
            SceneManager.LoadScene("MiniGame 2");
        }
        else if (timer >0f)
        {
            if (score>2)
            {
                SceneManager.LoadScene("MainScene");
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Space))
       {
        foreach (GameObject obj in moving)
        {
           
            if (obj == null) continue;

            float xPos = obj.transform.position.x;
            if (xPos < -4f && xPos > -6f)
            {
                if (obj.CompareTag("FoodHealthy"))
                {
                    // Increase score
                    score++;
                    Debug.Log("Score: " + score);
                }
                else if (obj.CompareTag("FoodUnhealthy"))
                {
                    // Increase heart rate / speed
                    speed += 0.5f; // example increment
                    Debug.Log("Heart rate increased! New speed: " + speed);
                }
               
                

                

                // Optional: destroy caught object
                Destroy(obj);
                moving.Remove(obj);
                break; // only catch one object per press
            }
        }
        }
    }

    GameObject SpawnHealthyFood()
    {
        if (healthyfood.Length == 0) return null;

        Vector3 spawnPos = new Vector3(12f, 0f, -1f);
        int randomIndex = Random.Range(0, healthyfood.Length);
        return Instantiate(healthyfood[randomIndex], spawnPos, Quaternion.identity);
    }

    GameObject SpawnUnhealthyFood()
    {
        if (unhealthfood.Length == 0) return null;

        Vector3 spawnPos = new Vector3(12f, 0f, -1f);
        int randomIndex = Random.Range(0, unhealthfood.Length);
        return Instantiate(unhealthfood[randomIndex], spawnPos, Quaternion.identity);
    }

        
}
