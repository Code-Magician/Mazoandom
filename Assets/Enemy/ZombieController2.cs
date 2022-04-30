using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController2 : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        WANDER,
        CHASE,
        ATTTACK,
        DEATH
    }
    public STATE state = STATE.IDLE;

    [Header("References")]
    public Transform target;
    [SerializeField] float damageAmount = 5f;
    [SerializeField] float walkingSpeed = 1f;
    [SerializeField] float runningSpeed = 5f;
    [SerializeField] float rotSpeed = 5f;
    [SerializeField] float approachDistance = 20;
    [SerializeField] float forgetPlayerDistance = 30f;
    [SerializeField] float attackDistance = 3f;
    [SerializeField] float minDistFromWall = 1f;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource AttackAudioSource;
    [SerializeField] AudioClip[] AttackClips;
    [SerializeField] GameObject ragDoll;



    bool idle, wander, chase, attack, death;
    Vector3 tempTarget = Vector3.zero;
    bool hasWanderTarget = false;
    bool isAlive = true;



    private void Update()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
            return;
        }

        switch (state)
        {
            case STATE.IDLE:
                ToggleAnimationTriggers();
                if (!GameStats.gameOver && CanSeePlayer())
                    state = STATE.CHASE;
                else if (Random.Range(0, 5000) < 10)
                    state = STATE.WANDER;
                break;
            case STATE.WANDER:
                if (hasWanderTarget)
                {
                    if (!MoveToTarget(tempTarget, 0.1f, walkingSpeed))
                        hasWanderTarget = false;
                }
                else
                {
                    tempTarget = GetNewWanderTaget();
                    if (tempTarget == Vector3.zero)
                        state = STATE.IDLE;
                    else
                    {
                        hasWanderTarget = true;
                        ToggleAnimationTriggers();
                        anim.SetBool("Walk", true);
                    }
                }
                if (!GameStats.gameOver && CanSeePlayer())
                    state = STATE.CHASE;
                else if (Random.Range(0, 5000) < 10)
                {
                    state = STATE.IDLE;
                    ToggleAnimationTriggers();
                    // agent.ResetPath();
                }
                break;
            case STATE.CHASE:
                if (!GameStats.gameOver)
                {
                    if (!MoveToTarget(target.position, attackDistance, runningSpeed))
                    {
                        state = STATE.ATTTACK;
                        hasWanderTarget = false;
                    }

                    if (!anim.GetBool("Run"))
                    {
                        ToggleAnimationTriggers();
                        anim.SetBool("Run", true);
                    }

                    if (InAttackRange())
                    {
                        state = STATE.ATTTACK;
                        hasWanderTarget = false;
                    }
                    if (ForgetPlayer())
                    {
                        state = STATE.WANDER;
                    }
                }
                break;
            case STATE.ATTTACK:
                if (!GameStats.gameOver)
                {
                    if (!anim.GetBool("Attack"))
                    {
                        ToggleAnimationTriggers();
                        anim.SetBool("Attack", true);
                    }

                    Vector3 lookAtPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                    this.transform.LookAt(lookAtPos);

                    if (OutOfAttackRange())
                        state = STATE.CHASE;
                }
                else
                {
                    GameOver();
                }
                break;
            case STATE.DEATH:
                ToggleAnimationTriggers();
                anim.SetBool("Death", true);

                if (isAlive)
                {
                    isAlive = false;
                    Sink sink = GetComponent<Sink>();
                    if (sink != null)
                        sink.StartSink();
                }

                break;
        }
    }

    private void GameOver()
    {
        state = STATE.WANDER;
    }

    private bool MoveToTarget(Vector3 destination, float stoppingDistance, float speed)
    {
        // Rotate
        Vector3 ourPos = new Vector3(transform.position.x, 0, transform.position.z);
        destination.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(destination - ourPos),
                                                rotSpeed * Time.deltaTime);

        // Move towards player
        float dist = Vector3.Distance(transform.position, destination);
        if (!(dist >= 0 && dist <= stoppingDistance))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            return true;
        }
        else return false;
    }


    private Vector3 GetNewWanderTaget()
    {
        int count = 0;

        float x = transform.position.x + Random.Range(-5f, 5f);
        float z = transform.position.z + Random.Range(-5f, 5f);
        float y = transform.position.y;
        Vector3 newTarget = new Vector3(x, y, z);

        while (count <= 500)
        {
            RaycastHit hitInfo;
            Ray ray = new Ray(newTarget + new Vector3(0, 2, 0), -Vector3.up);
            if (Physics.Raycast(ray, out hitInfo))
            {
                MapLoc mLoc = hitInfo.collider.gameObject.GetComponent<MapLoc>();
                if (mLoc == null)
                {
                    Ray rayX = new Ray(transform.position + new Vector3(0, 2, 0), Vector3.right);
                    Ray raynX = new Ray(transform.position + new Vector3(0, 2, 0), -Vector3.right);
                    Ray rayZ = new Ray(transform.position + new Vector3(0, 2, 0), Vector3.forward);
                    Ray raynZ = new Ray(transform.position + new Vector3(0, 2, 0), -Vector3.back);

                    bool right = false, left = false, forward = false, backward = false;
                    if (Physics.Raycast(rayX, out hitInfo))
                    {
                        right = true;
                    }

                    if (Physics.Raycast(raynX, out hitInfo))
                    {
                        left = true;
                    }
                    if (Physics.Raycast(rayZ, out hitInfo))
                    {
                        forward = true;
                    }
                    if (Physics.Raycast(raynZ, out hitInfo))
                    {
                        backward = true;
                    }

                    if (right && left)
                    {
                        x = transform.position.x;
                        z = transform.position.z + Random.Range(-5f, 5f);
                    }
                    else if (forward && backward)
                    {
                        x = transform.position.x + Random.Range(-5f, 5f);
                        z = transform.position.z;
                    }
                    else
                    {
                        x = transform.position.x + Random.Range(-5f, 5f);
                        z = transform.position.z + Random.Range(-5f, 5f);
                    }

                    y = transform.position.y;
                    newTarget = new Vector3(x, y, z);
                }
                else
                {
                    // Ray rayX = new Ray(newTarget + new Vector3(minDistFromWall, 2, 0), -Vector3.up);
                    // Ray raynX = new Ray(newTarget + new Vector3(-minDistFromWall, 2, 0), -Vector3.up);
                    // Ray rayZ = new Ray(newTarget + new Vector3(0, 2, minDistFromWall), -Vector3.up);
                    // Ray raynZ = new Ray(newTarget + new Vector3(0, 2, -minDistFromWall), -Vector3.up);

                    // bool possible = true;
                    // if (Physics.Raycast(rayX, out hitInfo))
                    // {
                    //     mLoc = hitInfo.collider.gameObject.GetComponent<MapLoc>();
                    //     if (mLoc == null)
                    //     {
                    //         possible = false;
                    //     }
                    // }
                    // else
                    //     possible = false;
                    // if (possible && Physics.Raycast(raynX, out hitInfo))
                    // {
                    //     mLoc = hitInfo.collider.gameObject.GetComponent<MapLoc>();
                    //     if (mLoc == null)
                    //     {
                    //         possible = false;
                    //     }
                    // }
                    // else
                    //     possible = false;
                    // if (possible && Physics.Raycast(rayZ, out hitInfo))
                    // {
                    //     mLoc = hitInfo.collider.gameObject.GetComponent<MapLoc>();
                    //     if (mLoc == null)
                    //     {
                    //         possible = false;
                    //     }
                    // }
                    // else
                    //     possible = false;
                    // if (possible && Physics.Raycast(raynZ, out hitInfo))
                    // {
                    //     mLoc = hitInfo.collider.gameObject.GetComponent<MapLoc>();
                    //     if (mLoc == null)
                    //     {
                    //         possible = false;
                    //     }
                    // }
                    // else
                    //     possible = false;


                    // if (possible)
                    return newTarget;
                }
            }
            count++;
        }

        return Vector3.zero;
    }




    private void ToggleAnimationTriggers()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Run", false);
        anim.SetBool("Attack", false);
        anim.SetBool("Death", false);
    }



    bool CanSeePlayer()
    {
        RaycastHit hitInfo;
        Vector3 direction = (target.position - transform.position);
        Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), direction);
        Debug.DrawRay(transform.position, direction);

        if (Vector3.Distance(this.transform.position, target.position) <= approachDistance && Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject.tag == "Player")
            {
                Debug.Log("Can See Player");
                return true;
            }
        }

        return false;
    }

    bool ForgetPlayer()
    {
        RaycastHit hitInfo;
        Vector3 direction = (target.position - transform.position);
        Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), direction);
        if (Vector3.Distance(this.transform.position, target.position) > forgetPlayerDistance)
        {
            return true;
        }
        else if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject.tag != "Player")
                return true;
        }


        return false;
    }

    bool OutOfAttackRange()
    {
        RaycastHit hitInfo;
        Vector3 direction = (target.position - transform.position);
        Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), direction);
        if ((Vector3.Distance(target.position, this.transform.position) > attackDistance + 2f))
        {
            return true;
        }
        else if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject.tag != "Player")
                return true;
        }
        return false;
    }

    bool InAttackRange()
    {
        RaycastHit hitInfo;
        Vector3 direction = (target.position - transform.position);
        Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), direction);
        if ((Vector3.Distance(transform.position, target.position) <= attackDistance) && Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject.tag == "Player")
                return true;
        }

        return false;
    }

    private void DamagePlayer()
    {
        if (!GameStats.gameOver)
        {
            target.GetComponent<FPController>().TakeDamage(damageAmount);
            RandomAttackSound();
        }
        else
        {
            Invoke("GameOver", 5f);
        }
    }


    public void RandomAttackSound()
    {
        int idx = Random.Range(1, AttackClips.Length);
        AudioClip clip = AttackClips[idx];

        AttackAudioSource.Stop();
        AttackAudioSource.clip = clip;
        AttackAudioSource.Play();


        AudioClip temp = clip;
        AttackClips[idx] = AttackClips[0];
        AttackClips[0] = temp;
    }

    public void KillSelf()
    {
        if (Random.Range(0, 100) < 50)
        {
            GameObject temp = Instantiate(ragDoll, transform.position, transform.rotation);
            temp.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.gameObject.transform.forward * 100, ForceMode.Impulse);
            Destroy(gameObject);
            return;
        }
        else
        {
            state = STATE.DEATH;
        }
    }
}
