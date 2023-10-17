using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOctuBehavior : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float distanceThreshold;
    [SerializeField] private AudioClip[] enemySounds;

    private Rigidbody2D enemyRigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private Vector2 playerPosition;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, PlayerBehavior.instance.GetPlayerPosition());
        if (distanceFromPlayer <= distanceThreshold)
        {
            playerPosition = PlayerBehavior.instance.GetPlayerPosition();
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, velocity * Time.deltaTime);
        }

        CheckEnemySprite();
    }
    private void CheckEnemySprite()
    {
        if (transform.position.x - playerPosition.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (transform.position.x - playerPosition.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void PlayDeathSound()
    {
        audioSource.Stop();
        audioSource.clip = enemySounds[1];
        audioSource.Play();
        StartCoroutine(Die());
        StartDeathAnim();
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    private void StartDeathAnim()
    {
        animator.SetTrigger("onDeath");
    }

}
