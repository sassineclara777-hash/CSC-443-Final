using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private Collider _coinCollider;
    private Renderer[] _renderers;
    private bool _collected;

    void Awake()
    {
        _coinCollider = GetComponent<Collider>();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    void OnEnable()
    {
        ResetCoin();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;

        if (other.CompareTag(playerTag))
        {
            PlayerCoinVFX playerVFX = other.GetComponentInParent<PlayerCoinVFX>();

            if (playerVFX != null)
            {
                playerVFX.PlayCoinCollectVFX();
            }

            Collect();
        }
    }

    private void Collect()
    {
        _collected = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin();
        }

        if (_coinCollider != null)
        {
            _coinCollider.enabled = false;
        }

        foreach (Renderer renderer in _renderers)
        {
            renderer.enabled = false;
        }
    }

    private void ResetCoin()
    {
        _collected = false;

        if (_coinCollider != null)
        {
            _coinCollider.enabled = true;
        }

        foreach (Renderer renderer in _renderers)
        {
            renderer.enabled = true;
        }
    }
}