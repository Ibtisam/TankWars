using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class TankController : MonoBehaviour
{
    Rigidbody rb;
    public float moveSpeed = 5f;
    public float turnSpeed = 2f;
    public float turretTurnSpeed = 5f;
    public float barrelTurnSpeed = 5f;
    public Transform turretTransform;
    public Transform barrelTransform;
    public Transform firePoint;
    public GameObject crosshair;
    public CinemachineCamera aimCam;
    public AudioSource tankMoveAudio;
    public AudioSource fireAudioSource;
    public ParticleSystem muzzleFlash;
    public ParticleSystem explosionFX;
    public ParticleSystem engineSmokeFX;
    public ParticleSystem dustFX;
    Vector2 moveInput;
    Vector2 lookInput;
    float barrelRotAmt = 0f;
    RaycastHit hit;
    bool isFired = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimCam.Priority = context.performed ? 25 : 10;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Fire();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        RotateTurret();
        RotateBarrel();
        AimRay();
        MoveSFXaVFX();
    }


    void Move()
    {
        if(moveInput.x != 0 || moveInput.y != 0)
        {
            float moveAmt = moveInput.y * moveSpeed * Time.fixedDeltaTime;
            rb.linearVelocity = transform.forward * moveAmt;

            float turnAmt = moveInput.x * turnSpeed * Time.fixedDeltaTime;
            rb.angularVelocity = transform.up * turnAmt;
        }
    }

    void RotateTurret()
    {
        if(lookInput.x != 0)
        {
            float turretRotAmt = lookInput.x * turretTurnSpeed * Time.deltaTime;
            turretTransform.Rotate(transform.up * turretRotAmt);
        }
    }

    void RotateBarrel()
    {
        if(lookInput.y != 0)
        {
            barrelRotAmt -= lookInput.y * barrelTurnSpeed * Time.deltaTime;
            barrelRotAmt = Mathf.Clamp(barrelRotAmt, -10f, 10f);
            barrelTransform.localRotation = Quaternion.Euler(barrelRotAmt, 0f, 0f);
        }
    }

    void AimRay()
    {
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        if(Physics.Raycast(ray, out hit, 100f))
        {
            crosshair.transform.position = hit.point;
        }
        else
        {
            crosshair.transform.position = ray.GetPoint(100f);
        }

        Debug.DrawLine(firePoint.position, ray.GetPoint(100f), Color.red);
    }

    void Fire()
    {
        FireAudio();
        if(hit.collider != null)
        {
            if(hit.collider.tag == "Enemy")
            {
                if(isFired)
                {
                    EnemyHealth eHealth = hit.collider.GetComponent<EnemyHealth>();
                    eHealth.TakeDamage(20);
                }
            }
            else
            {
                Debug.Log("Some Other Object");
            }
        }
        else
        {
            Debug.Log("No Collider Detected");
        }
    }

    void MoveSFXaVFX()
    {
        if(moveInput.x != 0 || moveInput.y != 0)
        {
            if(!tankMoveAudio.isPlaying)
            {
                tankMoveAudio.Play();
            }
            engineSmokeFX.Play();
            dustFX.Play();
        }
        else
        {
            tankMoveAudio.Pause();
            engineSmokeFX.Stop();
            dustFX.Stop();
        }
    }

    void FireAudio()
    {
        isFired = true;
        if(!fireAudioSource.isPlaying && isFired)
        {
            fireAudioSource.Play();
            PlayMuzzleFX();
            PlayExplosionFX();
        }
        else
        {
            isFired = false;
        }
    }

    void PlayMuzzleFX()
    {
        muzzleFlash.Play();
    }

    void PlayExplosionFX()
    {
        if(hit.collider != null)
        {
            explosionFX.transform.position = hit.point;
            explosionFX.Play();
        }
    }
}
