using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private Vector2 movePosition;
    [SerializeField, Range(0f, 10f)] private float velocity;
    [SerializeField] private float distanceThreshold;

    private Rigidbody2D rigidbody;
    private Animator animator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, PlayerBehavior.instance.GetPlayerPosition()); 
        if (distanceFromPlayer <= distanceThreshold)
        {
            Vector2 playerPosition = PlayerBehavior.instance.GetPlayerPosition();
            Vector2.MoveTowards(transform.position, playerPosition, 0.5f);
            transform.Translate(new Vector2(playerPosition.x, 0).normalized * velocity * Time.deltaTime * -1);
        }
    }

    private void SetIsMovingParameter(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }
}
