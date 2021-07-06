using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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
    public float toNextWaypointDistance = 1.5f;

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
            Target = new Vector3(Random.Range(GameManager_.WEST_LIMIT, GameManager_.EAST_LIMIT), Random.Range(GameManager_.SOUTH_LIMIT, GameManager_.NORTH_LIMIT), 0f);
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

}
