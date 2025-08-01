using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // Create bullet and aim it where the camera is looking
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);

        // Set its direction to match the camera's forward
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = cam.transform.forward * 20f;
    }
}
