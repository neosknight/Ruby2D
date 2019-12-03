using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    #region Properties

    #region Public Properties
    public int MaxHealth = 5;
    public float Speed = 3.0f;
    public float BulletLaunchForce = 300f;
    public int Health { get { return this.currentHealth; } }
    public GameObject ProjectilePrefab;
    public GameObject HitEffectPrefab;
    public AudioClip hitAudio;
    public AudioClip throwAudio;
    #endregion

    #region Private Properties
    int currentHealth;
    Rigidbody2D rigidbody2d;
    Animator animator;
    AudioSource audioSource;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    float deltaX;
    float deltaY;
    Vector2 lookDirection = new Vector2(1, 0);
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = MaxHealth;
    }

    void Animate(Vector2 move)
    {
        // Update animation for Ruby
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        this.Animate(move);

        Vector2 position = rigidbody2d.position;

        position = position + move * Speed * Time.deltaTime;

        rigidbody2d.MovePosition(position);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayableCharacter character = hit.collider.GetComponent<NonPlayableCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            GameObject hitEffectObject = Instantiate(HitEffectPrefab, rigidbody2d.position + Vector2.up * 2f, Quaternion.identity);

            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            this.PlaySound(hitAudio);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)MaxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(ProjectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, BulletLaunchForce);

        animator.SetTrigger("Launch");
        this.PlaySound(throwAudio);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
