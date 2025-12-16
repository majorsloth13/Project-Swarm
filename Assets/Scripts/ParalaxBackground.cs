using UnityEngine;

public class ParalaxBackground : MonoBehaviour
{
    private float StartPos;
    public GameObject cam;
    public float Paralaxeffect;
    void Start()
    {
        StartPos = transform.position.x;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = cam.transform.position.x * Paralaxeffect;

        transform.position = new Vector3(StartPos + distance, transform.position.y, transform.position.z);
    }
}
