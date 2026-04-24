using UnityEngine;

public class GlassTarget : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;

    private float startX;
    private float targetX;

    [Header("Hit Window")]
    [SerializeField] private float hitOffset = 1.5f;

    private bool canBeHit;
    private bool finished;

    public void Init(float moveSpeed, float spawnX, float hitX)
    {
        speed = moveSpeed;
        startX = spawnX;
        targetX = hitX;

        canBeHit = false;
        finished = false;
    }

    private void Update()
    {
        if (finished) return;

        Vector3 pos = transform.position;

        // ALWAYS MOVE FROM START → HIT + OFFSET (safe direction)
        float moveTarget = targetX + hitOffset;

        float newX = Mathf.MoveTowards(pos.x, moveTarget, speed * Time.deltaTime);

        transform.position = new Vector3(newX, pos.y, pos.z);

        // HIT WINDOW (CENTERED AROUND TARGET)
        float dist = Mathf.Abs(newX - targetX);

        if (dist <= hitOffset)
            canBeHit = true;

        // MISS AFTER PASSING FULL ZONE
        if (newX >= targetX + hitOffset)
            Fail();
    }

    public void OnGlassHit()
    {
        if (!canBeHit || finished) return;

        finished = true;

        Destroy(gameObject);
    }

    private void Fail()
    {
        if (finished) return;

        finished = true;

        Destroy(gameObject);
    }
}