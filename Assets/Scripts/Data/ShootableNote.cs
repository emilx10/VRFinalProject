using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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
    [SerializeField] private float hitWindow = 1.2f;
    [SerializeField] private float missBuffer = 1.0f;

    [Header("Visuals")]
    [SerializeField] private Renderer rend;
    [SerializeField] private Color perfectColor = Color.green;
    [SerializeField] private Color earlyColor = Color.yellow;
    [SerializeField] private float flashDuration = 0.15f;

    private Color originalColor;

    public void Init(float moveSpeed, float targetPosX)
    {
        speed = moveSpeed;
        targetX = targetPosX;
        startX = transform.position.x;

        active = true;
        canHit = false;
        finished = false;

        ResetVisual();
    }

    private void Awake()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();

        rend.material = new Material(rend.material);

        originalColor = rend.material.color;
    }

    private void Update()
    {
        if (!active || finished) return;

        float direction = (targetX > startX) ? 1 : -1;
        float moveAmount = speed * Time.deltaTime * direction;

        transform.Translate(Vector3.right * moveAmount, Space.World);

        float distToTarget = Mathf.Abs(transform.position.x - targetX);
        canHit = (distToTarget <= hitWindow);

        if (startX < targetX)
        {
            if (transform.position.x > targetX + missBuffer) Miss();
        }
        else
        {
            if (transform.position.x < targetX - missBuffer) Miss();
        }
    }

    public bool TryHit()
    {
        if (finished) return false;

        if (canHit)
        {
            Success();
            return true;
        }

        ShowColor(earlyColor);

        return false;
    }

    private void Success()
    {
        finished = true;
        active = false;

        ShowColor(perfectColor);

        OnNoteHit?.Invoke(transform.position);

        StartCoroutine(ReturnAfterFlash());
    }

    private void Miss()
    {
        if (finished) return;

        finished = true;
        active = false;

        OnNoteMiss?.Invoke(transform.position);

        NotePool.Instance.ReturnNote(gameObject);
    }

    private void ShowColor(Color color)
    {
        StopAllCoroutines();
        StartCoroutine(FlashColor(color));
    }

    private IEnumerator FlashColor(Color color)
    {
        rend.material.color = color;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = originalColor;
    }

    private IEnumerator ReturnAfterFlash()
    {
        yield return new WaitForSeconds(flashDuration);
        NotePool.Instance.ReturnNote(gameObject);
    }

    private void ResetVisual()
    {
        if (rend != null)
            rend.material.color = originalColor;
    }
}