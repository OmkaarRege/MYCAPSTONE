using UnityEngine;

public class RaiinPrefab : MonoBehaviour
{
    public float fallSpeed;
    public int number;

    private Minigame1 minigame;

    public GameObject[] healthyrain,unhealthyrain;
    void Start()
    {
        minigame = FindObjectOfType<Minigame1>();
        number = minigame.num;
        healthyrain[number].SetActive(true);
        unhealthyrain[number].SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
       
       
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
       
    }
}
