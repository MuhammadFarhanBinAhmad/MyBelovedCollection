using System.Collections;
using UnityEngine;

public class GroundSpike : MonoBehaviour
{

    [SerializeField]int _damage;
    [SerializeField] bool _isPopUp;

    [Header("Pop-Up Settings")]
    [SerializeField] float _downTime = 1f;       // How long spike stays hidden
    [SerializeField] float _upTime = 1f;         // How long spike stays active
    [SerializeField] float _popSpeed = 5f;       // Speed of popping up/down
    [SerializeField] Transform _spikeVisual;     // Reference to the visual part (child sprite/mesh)
    [SerializeField] Vector3 _hiddenOffset = new Vector3(0, -1f, 0); // Where spike hides

    private Vector3 _initialPos;
    private Vector3 _hiddenPos;
    private bool _cycleStarted = false; // Prevent starting multiple times
    bool inView;

    private Camera _playerCam;
    private Coroutine _popUpRoutine; // Keep track of coroutine
    private void Start()
    {
        if (_isPopUp)
        {
            _initialPos = _spikeVisual.localPosition;
            _hiddenPos = _initialPos + _hiddenOffset;
            _playerCam = Camera.main; // Assuming player uses main camera
        }
    }

    private void Update()
    {
        if (!_isPopUp) return;

        inView = IsInCameraView();

        if (inView && !_cycleStarted)
        {
            _popUpRoutine = StartCoroutine(PopUpCycle());
            _cycleStarted = true;
        }
        else if (!inView && _cycleStarted)
        {
            StopCoroutine(_popUpRoutine);
            _cycleStarted = false;
            // Optionally reset spike to hidden when leaving view
            _spikeVisual.localPosition = _hiddenPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            player.TakeDamage();
        }
    }

    private IEnumerator PopUpCycle()
    {
        while (true)
        {
            // Hide
            yield return MoveSpike(_hiddenPos);
            yield return new WaitForSeconds(_downTime);

            // Pop up
            yield return MoveSpike(_initialPos);
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_PopUpSpikeTrap, transform.position);
            yield return new WaitForSeconds(_upTime);
        }
    }

    private IEnumerator MoveSpike(Vector3 targetPos)
    {
        while (Vector3.Distance(_spikeVisual.localPosition, targetPos) > 0.01f)
        {
            _spikeVisual.localPosition = Vector3.MoveTowards(
                _spikeVisual.localPosition,
                targetPos,
                _popSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private bool IsInCameraView()
    {
        if (_playerCam == null) return false;

        Vector3 viewPos = _playerCam.WorldToViewportPoint(transform.position);

        return (viewPos.z > 0 &&
                viewPos.x > 0 && viewPos.x < 1 &&
                viewPos.y > 0 && viewPos.y < 1);
    }
}
