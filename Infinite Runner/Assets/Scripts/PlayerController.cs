using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Lanes")]
    [SerializeField] private float laneOffset = 2f;
    [SerializeField, Min(1)] private int laneCount = 3;
    [SerializeField] private float laneSwitchSpeed = 14f;

    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private float gravity = -25f;

    [Header("Ground / Walkable")]
    [SerializeField] private string walkableTag = "Walkable";
    [SerializeField] private float groundCheckStartHeight = 0.6f;
    [SerializeField] private float groundCheckDistance = 4f;
    [SerializeField] private float maxSnapUpHeight = 0.35f;
    [SerializeField] private float standHeightOffset = 0f;

    [Header("Obstacle")]
    [SerializeField] private string obstacleTag = "Obstacle";

    private int _laneIndex;
    private float _y;
    private float _yVel;
    private float _groundY;
    private bool _isGrounded = true;
    private Vector2 _prevMove;

    void Awake()
    {
        // We drive position directly, so any rigidbody on this object must be kinematic.
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        _y = transform.position.y;
        _groundY = 0f;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        Vector2 v = ctx.ReadValue<Vector2>();

        if (v.x > 0.5f && _prevMove.x <= 0.5f)
        {
            ChangeLane(+1);
        }
        else if (v.x < -0.5f && _prevMove.x >= -0.5f)
        {
            ChangeLane(-1);
        }

        if (v.y > 0.5f && _prevMove.y <= 0.5f && _isGrounded)
        {
            _yVel = jumpVelocity;
            _isGrounded = false;

            MusicManager.Instance?.JumpSound();
        }

        _prevMove = v;
    }

    private void ChangeLane(int delta)
    {
        int half = laneCount / 2;
        _laneIndex = Mathf.Clamp(_laneIndex + delta, -half, half);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        _groundY = GetCurrentGroundY();

        _yVel += gravity * Time.deltaTime;
        _y += _yVel * Time.deltaTime;

        if (_y <= _groundY)
        {
            _y = _groundY;
            _yVel = 0f;
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, _laneIndex * laneOffset, laneSwitchSpeed * Time.deltaTime);
        pos.y = _y;
        pos.z = 0f;
        transform.position = pos;
    }

    private float GetCurrentGroundY()
    {
        float bestGroundY = 0f;

        Vector3 rayOrigin = transform.position + Vector3.up * groundCheckStartHeight;

        RaycastHit[] hits = Physics.RaycastAll(
            rayOrigin,
            Vector3.down,
            groundCheckDistance,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit hit in hits)
        {
            if (!HasTagInParents(hit.collider.transform, walkableTag)) continue;

            float possibleGroundY = hit.point.y + standHeightOffset;

            // Prevent snapping upward onto a high train unless the player actually jumped high enough.
            if (possibleGroundY <= _y + maxSnapUpHeight && possibleGroundY > bestGroundY)
            {
                bestGroundY = possibleGroundY;
            }
        }

        return bestGroundY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasTagInParents(other.transform, obstacleTag))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private bool HasTagInParents(Transform obj, string tagToCheck)
    {
        Transform current = obj;

        while (current != null)
        {
            if (current.CompareTag(tagToCheck))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}