using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;
    public float speed = 3;
    [SerializeField] Transform aim;
    [SerializeField] Camera camera;
    Vector2 facingDirection;
    [SerializeField] Transform bulletPrefab;
    bool gunLoaded = true;
    [SerializeField] float fireRate = 1;
    [SerializeField] int health = 10;
    [SerializeField] int invulnerableTime = 3;
    bool powerShotEnabled;
    [SerializeField] bool invulnerable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement =  Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        moveDirection.x = horizontalMovement;
        moveDirection.y = verticalMovement;

        //Time.deltaTime is the time between frames, normalize the velocity

        transform.position += moveDirection * Time.deltaTime * speed;

        //aim movement
        facingDirection = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //When normalized set fixed position 
        aim.position = transform.position + (Vector3) facingDirection.normalized;

        if (Input.GetMouseButton(0) && gunLoaded)
        {
            gunLoaded = false;
            //Rotation between player and aim
            float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //Instanciate the transform, and position with the angle by default, because the object must create from de player
            Transform bulletClone = Instantiate(bulletPrefab, transform.position, targetRotation);
            if (powerShotEnabled)
            {
                bulletClone.GetComponent<Bullet>().powerShot = true;
            }
            StartCoroutine(ReloadGun());
        }
    }

    IEnumerator ReloadGun()
    {
        yield return new WaitForSeconds(1/fireRate);
        gunLoaded = true;
    }

    public void TakeDamage()
    {
        if (invulnerable)
            return;

        health--;
        invulnerable = true;
        StartCoroutine(MakeVulnerableAgain());
        if(health <= 0)
        {
            //TODO: GameOver
        }
    }

    IEnumerator MakeVulnerableAgain()
    {
        yield return new WaitForSeconds(invulnerableTime);
        invulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PowerUp"))
        {
            switch (collision.GetComponent<PowerUp>().powerrUpType)
            {
                case PowerUp.PowerrUpType.FireRateIncrease:
                    //TODO:Increase rate of fire
                    fireRate++;
                    break;
                case PowerUp.PowerrUpType.PowerShot:
                    //TODO: Activate PowerShot
                    powerShotEnabled = true;
                    break;
            }

            Destroy(collision.gameObject, 0.1f);
        }
    }
}
