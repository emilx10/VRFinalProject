using UnityEngine;
using UnityEngine.Events;

public class ShootableNote : MonoBehaviour
{
    public static UnityAction<Vector3> OnNoteHit;
    public static UnityAction<Vector3> OnNoteMiss;

    private float speed;
    private float startX;
    private float targetX;

    private bool active;
    private bool canHit;
    private bool finished;

    [Header("Rhythm Window Settings")]
    [SerializeField] private float hitWindow = 1.2f; // The "Big Area" on X axis for success
    [SerializeField] private float missBuffer = 1.0f; // Distance past target to fail

    public void Init(float moveSpeed, float targetPosX)
    {
        speed = moveSpeed;
        targetX = targetPosX;
        startX = transform.position.x;

        active = true;
        canHit = false;
        finished = false;
    }

    private void Update()
    {
        if (!active || finished) return;

        // Move ONLY on X, preserving the random Z set by NoteManager
        float direction = (targetX > startX) ? 1 : -1;
        float moveAmount = speed * Time.deltaTime * direction;
        
        transform.Translate(Vector3.right * moveAmount, Space.World);

        // Check if we are inside the Success Window
        float distToTarget = Mathf.Abs(transform.position.x - targetX);
        canHit = (distToTarget <= hitWindow);

        // Check for Miss (Passing the target)
        if (startX < targetX) // Moving Right
        {
            if (transform.position.x > targetX + missBuffer) Miss();
        }
        else // Moving Left
        {
            if (transform.position.x < targetX - missBuffer) Miss();
        }
    }

    // This is called by the Raycaster
    public bool TryHit()
    {
        if (finished) return false;

        if (canHit)
        {
            Success();
            return true;
        }

        // If hit but canHit is false, it was too early or too late
        return false; 
    }

    private void Success()
    {
        finished = true;
        active = false;
        OnNoteHit?.Invoke(transform.position);
        NotePool.Instance.ReturnNote(gameObject);
    }

    private void Miss()
    {
        if (finished) return;
        finished = true;
        active = false;
        OnNoteMiss?.Invoke(transform.position);
        NotePool.Instance.ReturnNote(gameObject);
    }
}