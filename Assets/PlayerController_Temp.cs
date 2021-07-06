using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController_Temp : MonoBehaviour
{
    public float MaxSpeed;
    private Rigidbody2D _rb;
    private Animator _anim;
    private Vector2 _mousePos;
    private bool isFacingRight;
    private GameManager_ gameManager;
    private Vector2 _mousePosOnScreen;
    public GameObject cursor;

    public Vector2 StartPosition;
    public ParticleSystem GrowParticle;
    public GameObject Point;
    

    public int DashMultiplier = 2;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        gameManager = GameManager_.Instance;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        StartPosition = transform.position;
    }

    private void Update()
    {
        if (!GameManager_.Instance.IsRunningGame) return;
        //if (Vector2.Distance(_mousePosOnScreen, Input.mousePosition) >= 0.05f)
        //{
            //_rb.drag = 0f;
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.transform.position = _mousePos;
            // Consider using add force
        Vector2 dir = _mousePos - (Vector2)transform.position;
        if (dir.magnitude >= 0.5f)
        {
            float speed = Mathf.Clamp(dir.magnitude * 5, 0, MaxSpeed);

            if (Input.GetMouseButton(0))
            {
                _rb.velocity = dir.normalized * speed * DashMultiplier;
            }
            else _rb.velocity = dir.normalized * speed;
         }
        else
        {
            _rb.velocity = Vector2.zero;
        }
        _anim.SetFloat("Speed", dir.magnitude);
        if (_rb.velocity.x > 0)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (_rb.velocity.x < 0)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        //}
        //else _rb.drag = 2f;
        

        _mousePosOnScreen = Input.mousePosition;

    }

    

    private bool CheckMouseInBound()
    {
        return (Input.mousePosition.x <= Screen.width && Input.mousePosition.x >= 0
            && Input.mousePosition.y <= Screen.height && Input.mousePosition.y >= 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior eb = collision.gameObject.GetComponent<EnemyBehavior>();
            if (gameManager.Stage >= eb.Stage)
            {
                eb.gameObject.SetActive(false);
                gameManager.AddProgress(eb.value);
                GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", transform.position);
                if (gameManager.Stage == eb.Stage) CinemachineShake.Instance.ShakeCamera(3f, .2f);
                SpawnFloatingPoint(eb.scoreValue);
                gameManager.AddScore(eb.scoreValue);
            } else
            {
                // TODO: reset stage
                transform.position = StartPosition;
                gameManager.AddLive(-1);
                gameManager.ResetProgress();
                GrowParticle.Play();
                GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", collision.transform.position);
            }
            
            GameManager_.Instance.SoundPlayer.PlayClip("Eat", 0.5f);
        }
        if (collision.gameObject.CompareTag("Live"))
        {
            Destroy(collision.gameObject);
            gameManager.AddLive(1);
        }
    }

    private void SpawnFloatingPoint(int score)
    {
        GameObject point = Instantiate(Point, transform.position, Quaternion.identity);
        point.GetComponentInChildren<TextMesh>().text = score.ToString();
        point.GetComponentInChildren<MeshRenderer>().sortingOrder = 10;
        Destroy(point, 2f);
    }

}
