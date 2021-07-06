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

    public Vector3 Target;
    public float speed = 200f;
    public float DetectionRadius = 5f;
    public float toNextWaypointDistance = 1.5f;
    public float value = 1f;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    // private Animator anim;

    private Seeker seeker;
    private Rigidbody2D rb;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        // anim = GetComponent<Animator>();
        InvokeRepeating("UpdatePath", 2f, .5f);
        Target = new Vector3(Random.Range(GameManager_.WEST_LIMIT, GameManager_.EAST_LIMIT), Random.Range(GameManager_.SOUTH_LIMIT, GameManager_.NORTH_LIMIT), 0f);
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
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            // Target = new Vector3(Random.Range(GameManager_.WEST_LIMIT, GameManager_.EAST_LIMIT), Random.Range(GameManager_.SOUTH_LIMIT, GameManager_.NORTH_LIMIT), 0f);
            DetectSurrounding();
            return;
        } else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < toNextWaypointDistance) currentWaypoint++;

        if (force.x >= 0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else if (force.x <= -0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private bool CheckNature(Collider2D col)
    {
        if (col.GetComponent<EnemyBehavior>() != null)
        {
            return col.gameObject != this.gameObject && col.GetComponent<EnemyBehavior>().Stage != this.Stage;
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
            //if (nearestEntity.CompareTag("Player"))
            //{
            //    if (Stage > GameManager_.Instance.Stage)
            //    {
            //        Target = nearestEntity.transform.position;
            //    }
            //    else
            //    {
            //        Target = transform.position + (nearestEntity.transform.position - this.transform.position).normalized * 10f;
            //    }
            //}
            if (nearestEntity.CompareTag("Enemy"))
            {
                EnemyBehavior other = nearestEntity.GetComponent<EnemyBehavior>();
                if (other.Stage > this.Stage)
                {
                    // Avoid predator
                    Target = transform.position + (other.transform.position - this.transform.position).normalized * 10f;
                }
                else if (other.Stage < this.Stage)
                {
                    Target = other.transform.position;
                    // chase prey
                }
            } // else Target = new Vector3(Random.Range(GameManager_.WEST_LIMIT, GameManager_.EAST_LIMIT), Random.Range(GameManager_.SOUTH_LIMIT, GameManager_.NORTH_LIMIT), 0f);
        } else Target = new Vector3(Random.Range(GameManager_.WEST_LIMIT, GameManager_.EAST_LIMIT), Random.Range(GameManager_.SOUTH_LIMIT, GameManager_.NORTH_LIMIT), 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponent<EnemyBehavior>().Stage < this.Stage)
            {
                collision.gameObject.SetActive(false);
            } else if (collision.GetComponent<EnemyBehavior>().Stage > this.Stage)
            {
                gameObject.SetActive(false);
            }
        }
    }

}
