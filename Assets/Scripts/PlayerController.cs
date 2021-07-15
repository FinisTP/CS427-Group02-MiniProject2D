using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float MaxSpeed = 5f;
    private Rigidbody2D rb;
    private Animator _anim;
    private Vector2 _mousePos;
    private bool isFacingRight;
    private GameManager_ gameManager;
    private Vector2 _mousePosOnScreen;
    public GameObject GFX;

    public Vector2 StartPosition;
    public ParticleSystem GrowParticle;
    public GameObject Point;
    public Texture2D CursorSprite;

    public ParticleSystem DustParticle;
    public TrailRenderer Trail;

    public Joystick joystick;

    private bool isInvincible = false;
    public int DashMultiplier = 2;
    private bool isDead = false;
    private Vector2 MouseDirection;
    private bool usingMouse = true;

    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float boostFactor = 2;
    [SerializeField] private float boostUnit = 25;

    private float moveX = 0, moveY = 0;
    private bool boosting = false;
    private float factor;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _anim = GFX.GetComponent<Animator>();
        gameManager = GameManager_.Instance;
        Cursor.lockState = CursorLockMode.Confined;
        // Cursor.SetCursor(CursorSprite, new Vector2(0, 0), CursorMode.Auto);
        //Cursor.visible = false;
        StartPosition = transform.position;
        joystick = GameObject.FindObjectOfType<FixedJoystick>();
    }

    private void Update()
    {
        

        if (!GameManager_.Instance.IsRunningGame || isDead) return;
        if (Input.GetMouseButton(0))
        {
            boosting = true;
        }
        else boosting = false;
        boosting = Input.GetKey(KeyCode.Space) || boosting;
        // Keyboard
        if (!Application.isMobilePlatform)
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
            
            Vector2 log = new Vector2(moveX, moveY).normalized;
            moveX = log.x;
            moveY = log.y;
            // _anim.SetFloat("Speed", 1);
            if ((moveX < -0.01f && isFacingRight) || (moveX > 0.01f && !isFacingRight))
            {
                FlipSprite();
            }
        }
        else if (Application.isMobilePlatform)
        {
            moveX = joystick.Horizontal;
            moveY = joystick.Vertical;
            Vector2 log = new Vector2(moveX, moveY).normalized;
            moveX = log.x;
            moveY = log.y;
            // _anim.SetFloat("Speed", 1);
            if ((moveX < -0.01f && isFacingRight) || (moveX > 0.01f && !isFacingRight))
            {
                FlipSprite();
            }
        }

        if (!Application.isMobilePlatform && _mousePosOnScreen != (Vector2)Input.mousePosition && moveX == 0 && moveY == 0)
        {
            // Mouse
            usingMouse = true;
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            MouseDirection = _mousePos - (Vector2)transform.position;

            float speed = Mathf.Clamp(MouseDirection.magnitude * (MaxSpeed + gameManager.speedBoost), 0, (MaxSpeed + gameManager.speedBoost));
            if (MouseDirection.magnitude >= 0.5f)
            {
                rb.velocity = MouseDirection.normalized * speed * factor;
            }
            else
            {
                // rb.velocity = Vector2.zero;
            }

            // _anim.SetFloat("Speed", MouseDirection.magnitude);
            if (rb.velocity.x > 0 && !isFacingRight)
            {
                FlipSprite();
            }
            else if (rb.velocity.x < 0 && isFacingRight)
            {
                FlipSprite();
            }
        }
        else usingMouse = false;
        _mousePosOnScreen = (Vector2) Input.mousePosition;

        _anim.SetFloat("Speed", rb.velocity.magnitude/2);
        if (rb.velocity.magnitude <= 1)
        {
            _anim.Play("Trunk_Idle");
        } 
        if (boosting) Trail.enabled = true;
        else Trail.enabled = false;

    }

    private void FixedUpdate()
    {
        if (!gameManager.IsRunningGame || isDead) return;
        factor = 1;
        if (boosting)
        {
            gameManager.StopChargingBoost();
            factor = gameManager.UseBoost(boostUnit * Time.fixedDeltaTime) ? boostFactor : factor;
            // GameManager_.Instance.SoundPlayer.PlayClip("Dash", 0.5f);
        }
        else
        {
            gameManager.ContinueChargingBoost();
        }
        if (!usingMouse && (moveX != 0 || moveY != 0))
            rb.velocity = new Vector2(moveX * (MaxSpeed + gameManager.speedBoost) * factor, moveY * (MaxSpeed + gameManager.speedBoost) * factor);

        if (rb.velocity.magnitude > 0) { if (!DustParticle.isPlaying) DustParticle.Play(); }
        else DustParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (rb.velocity.magnitude >= MaxSpeed * boostFactor) _anim.SetBool("Dash", true);
        else _anim.SetBool("Dash", false);
    }

    private void FlipSprite()
    {
        Vector3 currentLocalScale = transform.localScale;
        transform.localScale = Vector3.Scale(currentLocalScale, new Vector3(-1, 1, 1));
        isFacingRight = !isFacingRight;
    }

    public void SetAnimation(AnimatorOverrideController aoc)
    {
        if (aoc != null)
        _anim.runtimeAnimatorController = aoc;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || !gameManager.IsRunningGame) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior eb = collision.gameObject.GetComponent<EnemyBehavior>();
            if (gameManager.Stage >= eb.Stage)
            {
                _anim.SetTrigger("Attack");
                eb.gameObject.SetActive(false);
                gameManager.AddProgress(eb.value);
                int score = gameManager.AddScore(eb.scoreValue);
                GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", transform.position);
                if (gameManager.Stage == eb.Stage)
                {
                    CinemachineShake.Instance.ShakeCamera(3f, .2f);
                } 
                SpawnFloatingPoint(score, eb.scoreValue);
                
            }
            else if (!isInvincible)
            {
                bool isEaten = gameManager.AddLive(-1);
                if (isEaten)
                {
                    GFX.SetActive(false);
                    gameManager.ResetProgress();
                    rb.velocity = Vector2.zero;
                    SpawnEaten();
                    GameManager_.Instance.ParticlePlayer.PlayEffect("BloodSplatter", collision.transform.position);
                    if (!gameManager.Lost)
                    {
                        StartCoroutine(Respawn());
                        StartCoroutine(DamagedCooldown());
                    }
                } else
                {
                    StartCoroutine(DamagedCooldown());
                }
            }
            int rand = Random.Range(1, 7);
            GameManager_.Instance.SoundPlayer.PlayClip("Eat" + rand.ToString(), 1f);
        }
        if (collision.gameObject.CompareTag("Live"))
        {
            collision.gameObject.SetActive(false);
            gameManager.AddLive(1);
            GameManager_.Instance.SoundPlayer.PlayClip("Mushroom", 0.5f);
            GameManager_.Instance.ParticlePlayer.PlayEffect("Mushroom", collision.transform.position);
        }
    }

    private void SpawnEaten()
    {
        GameObject point = Instantiate(Point, transform.position, Quaternion.identity);
        TextMesh tm = point.GetComponentInChildren<TextMesh>();
        tm.text = "Eaten";
        tm.color = Color.red;
        // tm.fontStyle = FontStyle.Italic;
        tm.fontSize = 300;
        point.GetComponentInChildren<MeshRenderer>().sortingOrder = 12;
        Destroy(point, 2f);
    }

    private void SpawnFloatingPoint(float score, float originalScore)
    {
        GameObject point = Instantiate(Point, transform.position, Quaternion.identity);
        TextMesh tm = point.GetComponentInChildren<TextMesh>();
        float multiplier = Mathf.Round(score/originalScore * 2) / 2;
        tm.text = "+" + score.ToString() + " (x" + multiplier.ToString("R") + ")";
        if (score > originalScore)
        {
            tm.fontSize = 50 + (int)(score / originalScore) * 5;
        } else tm.fontSize = 50;

        SpawnTag(multiplier);

        point.GetComponentInChildren<MeshRenderer>().sortingOrder = 10;
        Destroy(point, 2f);
    }

    private void SpawnTag(float mult)
    {
        if (mult < 3f) return;
        int type = 0;
        if (mult >= 3f && gameManager.comboState == 0)
        {
            gameManager.comboState++;
            type = 1;
        } else if (mult >= 6f && gameManager.comboState == 1)
        {
            gameManager.comboState++;
            type = 2;
        } else if (mult >= 9f && gameManager.comboState == 2)
        {
            gameManager.comboState++;
            type = 3;
        } else if (mult >= 12f && gameManager.comboState == 3)
        {
            gameManager.comboState++;
            type = 4;
        } else if (mult >= 15f && gameManager.comboState == 4)
        {
            gameManager.comboState++;
            type = 5;
        }
        if (type == 0) return;

        GameObject point = Instantiate(Point, transform.position, Quaternion.identity);
        TextMesh tm = point.GetComponentInChildren<TextMesh>();
        tm.fontSize = 200;
        
        switch (type)
        {
            case 1:
                tm.text = "GREAT!";
                tm.color = Color.green;
                break;
            case 2:
                tm.text = "AMAZING!";
                tm.color = Color.cyan;
                break;
            case 3:
                tm.text = "SUPERSTAR!";
                tm.color = Color.yellow;
                break;
            case 4:
                tm.text = "LEGENDARY!";
                tm.color = Color.blue;
                break;
            case 5:
                tm.text = "FRENZY!";
                tm.color = Color.magenta;
                break;
            default:
                break;

        }
        point.GetComponentInChildren<MeshRenderer>().sortingOrder = 11;
        Destroy(point, 2f);
    }

    IEnumerator DamagedCooldown()
    {
        isInvincible = true;
        yield return new WaitForSeconds(4f);
        isInvincible = false;
    }

    public void SetBoosting(bool state)
    {
        if (Application.isMobilePlatform)
            boosting = state;
    }

    IEnumerator Respawn()
    {
        isDead = true;
        
        yield return new WaitForSeconds(2f);
        GFX.SetActive(true);
        isDead = false;
        transform.position = StartPosition;
        GrowParticle.Play();
    }
}
