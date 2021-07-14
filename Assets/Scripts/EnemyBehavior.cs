using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public enum EnemyState
{
    PATROL,
    RUNAWAY,
    HUNT
}

public class EnemyBehavior : MonoBehaviour
{
    public int Stage = 1;
    public bool Eatable = true;

    public Vector3 Target;
    public float speed = 200f;
    private float runawaySpeed = 400f;
    private float currSpeed;
    private float DetectionRadius = 3f;
    public float toNextWaypointDistance = 1.5f;
    public float value = 1f;
    public int scoreValue = 200;

    public bool initialSpriteFacingLeft = true;

    private bool isVisible = false;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    // private Animator anim;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator anim;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // anim = GetComponent<Animator>();
        InvokeRepeating("UpdatePath", 2f, .5f);
        Target = new Vector3(Random.Range(GameManager_.Instance.WEST_LIMIT, GameManager_.Instance.EAST_LIMIT), Random.Range(GameManager_.Instance.SOUTH_LIMIT, GameManager_.Instance.NORTH_LIMIT), 0f);
        InvokeRepeating("DetectSurrounding", 1f, 1f);
        runawaySpeed = speed * 2f;
        currSpeed = speed;
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            
            seeker.StartPath(rb.position, Target, OnPathComplete);
        }
        
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager_.Instance.IsRunningGame) return;
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            Target = new Vector3(Random.Range(GameManager_.Instance.WEST_LIMIT, GameManager_.Instance.EAST_LIMIT), Random.Range(GameManager_.Instance.SOUTH_LIMIT, GameManager_.Instance.NORTH_LIMIT), 0f);
            // DetectSurrounding();
            return;
        } else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * currSpeed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < toNextWaypointDistance) currentWaypoint++;
        float faceFactor = initialSpriteFacingLeft ? 1 : -1;
        if (force.x >= 0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x) * faceFactor, transform.localScale.y, transform.localScale.z);
        } else if (force.x <= -0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * faceFactor, transform.localScale.y, transform.localScale.z);
        }
    }

    private bool CheckNature(Collider2D col)
    {
        if (col.GetComponent<EnemyBehavior>() != null)
        {
            return col.gameObject != this.gameObject && col.GetComponent<EnemyBehavior>().Stage != this.Stage;
        } else if (col.GetComponent<PlayerController>() != null)
        {
            return true;
        }
        return false;
    }

    private void DetectSurrounding()
    {
        Collider2D[] checkNear = Physics2D.OverlapCircleAll(transform.position, DetectionRadius).Where(col => CheckNature(col)).ToArray();
        Collider2D nearestEntity = null;
        if (checkNear.Length > 0)
        {
            nearestEntity = checkNear.OrderBy(col => Vector2.Distance(col.transform.position, transform.position)).First();
            if (nearestEntity.CompareTag("Player"))
            {
                if (Stage > GameManager_.Instance.Stage)
                {
                    Target = (Vector2)nearestEntity.transform.position;
                    currSpeed = runawaySpeed;
                }
                else
                {
                    Target = (Vector2)transform.position - ((Vector2)nearestEntity.transform.position - (Vector2)this.transform.position).normalized * 10f;
                    // print(Target);
                    currSpeed = runawaySpeed;
                }
            }
            if (nearestEntity.CompareTag("Enemy"))
            {
                EnemyBehavior other = nearestEntity.GetComponent<EnemyBehavior>();
                if (other.Stage > this.Stage)
                {
                    // Avoid predator
                    Target = (Vector2)transform.position - ((Vector2)other.transform.position - (Vector2)this.transform.position).normalized * 10f;
                    currSpeed = runawaySpeed;
                }
                else if (other.Stage < this.Stage)
                {
                    Target = (Vector2)other.transform.position;
                    // chase prey
                    currSpeed = runawaySpeed;
                }
            } // else Target = new Vector3(Random.Range(GameManager_.WEST_LIMIT, GameManager_.EAST_LIMIT), Random.Range(GameManager_.SOUTH_LIMIT, GameManager_.NORTH_LIMIT), 0f);
        } else
        {
            // Target = new Vector3(Random.Range(GameManager_.Instance.WEST_LIMIT, GameManager_.Instance.EAST_LIMIT), Random.Range(GameManager_.Instance.SOUTH_LIMIT, GameManager_.Instance.NORTH_LIMIT), 0f);
            currSpeed = speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isVisible || gameObject.CompareTag("Live")) return;
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponent<EnemyBehavior>().Stage < this.Stage)
            {
                GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", collision.transform.position);
                //GameManager_.Instance.SoundPlayer.PlayClip("Eat", 0.5f);
                collision.gameObject.SetActive(false);
                anim.SetTrigger("Attack");
            } else if (collision.GetComponent<EnemyBehavior>().Stage > this.Stage)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

}
