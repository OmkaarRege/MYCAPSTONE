using UnityEngine;

public class Mirror : MonoBehaviour
{
    public GameObject player,mirrorCamera;
    public float smoothSpeed = 3f;
    void Update()
{
    // Check if the player is close enough along the Z-axis
    if (Mathf.Abs(player.transform.position.z - transform.position.z) < 2)
    {
        float newY = Mathf.Lerp(mirrorCamera.transform.position.y, player.transform.position.y, smoothSpeed * Time.deltaTime);

        // Update the mirror camera's position with the new smoothed X value, maintaining its Y and Z positions
        mirrorCamera.transform.position = new Vector3(player.transform.position.y, newY, mirrorCamera.transform.position.z);
    }
    else
    {
        // When the player is far away, smoothly move the camera back to the mirror's original X position
        float targetX = transform.position.x; // Original X position of the mirror/target object
        float newX = Mathf.Lerp(mirrorCamera.transform.position.x, targetX, smoothSpeed * Time.deltaTime);

        mirrorCamera.transform.position = new Vector3(newX, mirrorCamera.transform.position.y, mirrorCamera.transform.position.z);
    }
}
}

