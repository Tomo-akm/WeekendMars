using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    private Transform[] waypoints;
    public float speed = 2f;
    public int currentWaypointIndex = 0;

    Rigidbody2D rbody;

    public Vector2 LastMoveDirection { get; private set; } = Vector2.zero;

    public void SetWaypoints(Transform[] points)
    {
        waypoints = points;
    }

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || currentWaypointIndex >= waypoints.Length)
            return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        LastMoveDirection = direction;

        // ✅ Waypoint 3〜4〜5の間だけ左向きに、それ以外は右向きに
        Vector3 scale = transform.localScale;
        if (currentWaypointIndex >= 3 && currentWaypointIndex <= 4)
        {
            scale.x = -Mathf.Abs(scale.x); // 左向き（Xを負に）
        }
        else
        {
            scale.x = Mathf.Abs(scale.x); // 右向き（Xを正に）
        }
        transform.localScale = scale;

        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;
        }
    }
}
