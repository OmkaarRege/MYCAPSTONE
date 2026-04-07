using UnityEngine;

public class RaiinPrefab : MonoBehaviour
{
    public float fallSpeed;

    public GameObject player; 
    public int number,playerX;

    public bool scoreupdated =false;

    private Minigame1 minigame;

    public GameObject[] healthyrain,unhealthyrain;
    void Start()
    {
        minigame = FindObjectOfType<Minigame1>();
        number = minigame.num;
        healthyrain[number].SetActive(true);
        unhealthyrain[number].SetActive(false);
        player = GameObject.Find("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
       
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

       
        if (transform.position.y < 1.5f && transform.position.y > -1.5f)
        {
            // Get player's X position (rounded to nearest int)
            playerX = Mathf.RoundToInt(player.transform.position.x);

             // Print both values for debugging
            Debug.Log("Player X: " + playerX + " | Rain Number: " + number);

            // If player X matches this rain's index number
            if ((playerX==2 && number ==2)||(playerX==6 && number ==1)||(playerX==-2 && number ==3)||(playerX==-6 && number ==4)|| (playerX==10 && number ==0) || (playerX==-10 && number == 5))
            {
                healthyrain[number].SetActive(false);

                if (!scoreupdated)
                {
                    minigame.score++;
                    scoreupdated=true;
                }

                
            }
        }
        if (transform.position.y < -7 )
        {
            Destroy(gameObject);
        }
    
        
       
    }
   
}
