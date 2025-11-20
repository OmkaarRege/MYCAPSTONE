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

       
        if (transform.position.y < 1 && transform.position.y > -0.5f)
        {
            // Get player's X position (rounded to nearest int)
            playerX = Mathf.RoundToInt(player.transform.position.x);

             // Print both values for debugging
            Debug.Log("Player X: " + playerX + " | Rain Number: " + number);

            // If player X matches this rain's index number
            if ((playerX==2 && number ==4)||(playerX==4 && number ==3)||(playerX==6 && number ==2)|| (playerX==8 && number ==1) || (playerX==10 && number == 0)||(playerX==0 && number ==5)||(playerX==-2 && number ==6)||(playerX==-4 && number ==7)||(playerX==-6 && number ==8)|| (playerX==-8 && number ==9) || (playerX==-10 && number == 10))
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
