using UnityEngine;

public class AttackBoxFlip : MonoBehaviour
{
    float movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement = Input.GetAxis("Horizontal");
        UpdateAttackBoxDirection();
    }
    private void UpdateAttackBoxDirection()
    {
        // Flips the sprite based on the direction the player is moving
        if (movement > 0f)
        {


            transform.localScale = new Vector3(1, 1, 1);

        }
        else if (movement < 0f)
        {


            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
