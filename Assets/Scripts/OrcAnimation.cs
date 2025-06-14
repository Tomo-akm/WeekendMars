using UnityEngine;

public class OrcAnimation : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    private float stillTime = 0f;
    public float stopThreshold = 0.01f;  // 動かなくなったと判定する距離のしきい値
    public float timeToAttack = 0.5f;    // 0.5秒以上止まっていたら攻撃開始

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsAttacking", false);
        lastPosition = transform.position;
    }

    void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved < stopThreshold)
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
