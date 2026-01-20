using UnityEngine;

public class MoveTowardPlayer : MonoBehaviour
{
    public float speed = 2.5f;
    public float destroyDistance = 0.25f;

    private Transform target;

    void Start()
    {
        GameObject t = GameObject.Find("PlayerAttractPoint");
        if (t != null)
            target = t.transform;
        else
            Debug.LogError("PlayerAttractPoint not found");
    }

    void Update()
    {
        if (!target)
            return;

        Vector3 dir = target.position - transform.position;

        if (dir.magnitude <= destroyDistance)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += dir.normalized * speed * Time.deltaTime;
        transform.LookAt(target);
    }
}