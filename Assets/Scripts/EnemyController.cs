using UnityEngine;
using UnityEngine.Animations;

public class EnemyController : MonoBehaviour
{
    RaycastHit hit;
    bool isFired = false;
    [Header("Vision Settings")]
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 90f;

    [Header("Transforms & Layer Masks")]
    public Transform playerTarget;
    public Transform firePoint;
    public Transform canonTransform;
    public LayerMask obstacleMask;

    [Header("Read Only")]
    public bool canSeePlayer;

    [Header("SFX & VFX")]
    public AudioSource fireAudioSource;
    public ParticleSystem muzzleFlash;
    public ParticleSystem bulletFX;

    private void Start()
    {
        // Automatically find player if not assigned
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) 
            {
                playerTarget = player.transform;
            }
        }
    }

    private void Update()
    {
        canSeePlayer = CheckLineOfSight();
        if(canSeePlayer)
        {
            GunRotation();
            Aim();
        }
    }

    public bool CheckLineOfSight()
    {
        if (playerTarget == null) 
        {
            return false;
        }

        // 1. Distance Check
        Vector3 directionToPlayer = playerTarget.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= viewRadius)
        { 
            // 2. Angle Check (Using Vector3.Angle or Dot Product)
            if (Vector3.Angle(transform.forward, directionToPlayer.normalized) < viewAngle / 2)
            {
                // 3. Obstruction Check (Raycast)
                // We raycast up to the distance of the player, filtering for obstacles
                Ray ray = new Ray(transform.position, directionToPlayer.normalized);
                if (!Physics.Raycast(ray, distanceToPlayer, obstacleMask))
                {
                    return true; // Unobstructed view!
                }
            }
        }
        return false;
    }

    void GunRotation()
    {
        Vector3 gunDirection = playerTarget.position - canonTransform.position;
        gunDirection.Normalize();
        canonTransform.rotation = Quaternion.LookRotation(gunDirection);
    }

    void Aim()
    {
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        if(Physics.Raycast(ray, out hit, viewRadius))
        {
            Fire();
        }
        Debug.DrawLine(firePoint.position, ray.GetPoint(viewRadius), Color.red);
    }

    void Fire()
    {
        FireAudio();
        if(isFired)
        {
            PlayerHealth pHealth = playerTarget.GetComponent<PlayerHealth>();
            pHealth.TakeDamage(20);
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
            bulletFX.transform.position = hit.point;
            bulletFX.Play();
        }
    }

    // Draw the view cone in the Editor for easy debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        if (canSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, playerTarget.position);
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}