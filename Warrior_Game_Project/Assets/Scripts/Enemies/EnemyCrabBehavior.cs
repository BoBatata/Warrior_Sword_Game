using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrabBehavior : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float velocity;
    [SerializeField] private float distanceThreshold;
    [SerializeField] private AudioClip[] enemySounds;

    private Rigidbody2D rigidbodyEnemy;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private Vector2 playerPosition;

    private void Awake()
    {
        rigidbodyEnemy = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, PlayerBehavior.instance.GetPlayerPosition());
        if (distanceFromPlayer <= distanceThreshold)
        {
            SetIsMovingParameter(true);
            playerPosition = PlayerBehavior.instance.GetPlayerPosition();
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, velocity * Time.deltaTime);
        }

        if (rigidbodyEnemy.velocity.magnitude == 0)
        {
            SetIsMovingParameter(false);
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

    private void SetIsMovingParameter(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }

    private void PlayWalkSound()
    {
        audioSource.Stop();
        audioSource.clip = enemySounds[0];
        audioSource.Play();
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
