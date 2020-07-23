using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    LayerMask groundMask;

    new Rigidbody2D rigidbody;
    new BoxCollider2D collider;
    Animator animator;
    AudioSource audioSource;

    [SerializeField]
    AudioClip jumpSound;

    [SerializeField]
    AudioClip coinSound;

    [SerializeField]
    AudioClip hurtSound;

    public bool isHuring;

    public delegate void Callback();
    public event Callback OnCoinCollected;
    public event Callback OnHurted;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHuring)
            return;

        if (collision.collider.tag == "Obstacle")
        {
            OnHurted();

            isHuring = true;

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = hurtSound;
            audioSource.Play();
        }

        if (collision.collider.tag == "Coin")
        {
            OnCoinCollected();

            Destroy(collision.collider.gameObject);

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = coinSound;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("IsJumping", !isGrounded());
        animator.SetBool("IsHurting", isHuring);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            float jumpVelocity = 12.0f;
            rigidbody.velocity = Vector2.up * jumpVelocity;
            rigidbody.gravityScale = 2.5f;

            audioSource.clip = jumpSound;
            audioSource.Play();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            rigidbody.gravityScale = 7.0f;
        }
    }

    bool isGrounded()
    {
        var hit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0.0f, Vector2.down, 0.1f, groundMask);
        return hit.collider != null;
    }
}
