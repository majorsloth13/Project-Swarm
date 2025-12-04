/*using UnityEngine;

public class GunScript : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    private bool canFire;
    private float timeBetweenFiring;
    [SerializeField] private float firingTime;
    public GameObject Bullet;
    public Transform bulletTransform;
    public float bulletActiveTimer;
    
    public GameObject gun;
    public Movement_Slash BuffTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main;
        BuffTimer = FindObjectOfType<Movement_Slash>();

    }

    // Update is called once per frame
    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (BuffTimer.buffTimer >= 15f)
        {
            

            if (!canFire)
            {
                timeBetweenFiring += Time.deltaTime;
                if (timeBetweenFiring > firingTime)
                {
                    Debug.Log("time to fire");
                    canFire = true;
                    timeBetweenFiring = 0;
                }
            }

            if (Input.GetMouseButtonDown(0) && canFire)
            {
                canFire = false;
                GameObject instatiatedBullet = Instantiate(Bullet, bulletTransform.position, Quaternion.identity);

                Destroy(instatiatedBullet, bulletActiveTimer);

            }

        }
        else if (BuffTimer.buffTimer < 15f)
        {
            
        }
        


    }
}*/
