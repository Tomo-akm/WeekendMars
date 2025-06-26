using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    private float stillTime = 0f;
    public float stopThresholdPerSecond = 0.01f; // 毎秒の動きがこの値未満なら停止と判断
    public float timeToAttack = 0.5f; // 停止と判断してから攻撃開始までの時間

    void Start()

    {
        animator = GetComponentInChildren<Animator>();  
        animator.SetBool("IsAttacking", false);
    }

    void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        float movementPerSecond = distanceMoved / Time.deltaTime;

        if (movementPerSecond < stopThresholdPerSecond)
        {
            stillTime += Time.deltaTime;

            if (stillTime >= timeToAttack)
            {
                animator.SetBool("IsAttacking", true);
            }
        }
        else
        {
            stillTime = 0f;
        }

        lastPosition = transform.position;
    }
}
