using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] points; // Points to patrol between
    public float speed = 2f;
    public int startPointIndex = 0;
    private int currentPointIndex;
    public float _waitTime;
    bool _pointReach;

    [Header("Platform Types")]
    [SerializeField] bool _isActivatedOnPlayerEnter;
    bool _isplatformActivated;

    [SerializeField]List<MovingPlatform> allPlatforms = new List<MovingPlatform>();

    private void OnEnable()
    {
        allPlatforms.Add(this);
    }

    private void OnDisable()
    {
        allPlatforms.Remove(this);
    }
    void Start()
    {
        if (points.Length == 0) return;
        currentPointIndex = startPointIndex % points.Length;
        transform.position = points[currentPointIndex].position;

        // If the platform should move by default, activate immediately
        if (!_isActivatedOnPlayerEnter)
        {
            _isplatformActivated = true;
        }
    }

    void Update()
    {
        if (points.Length == 0 || !_isplatformActivated) return; // <-- Check here

        // Move towards the current target point
        transform.position = Vector2.MoveTowards(
            transform.position,
            points[currentPointIndex].position,
            speed * Time.deltaTime
        );

        // If reached the target point, move to the next one
        if (Vector2.Distance(transform.position, points[currentPointIndex].position) < 0.05f && !_pointReach)
        {
            StartCoroutine(NextPoint());
        }
    }

    IEnumerator NextPoint()
    {
        _pointReach = true;
        yield return new WaitForSeconds(_waitTime);
        currentPointIndex = (currentPointIndex + 1) % points.Length;
        _pointReach = false;
    }
    private void ActivateLinkedPlatforms()
    {
        foreach (var platform in allPlatforms)
        {
            if (platform != this &&
                platform._isActivatedOnPlayerEnter &&
                !platform._isplatformActivated)
            {
                platform._isplatformActivated = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isActivatedOnPlayerEnter && !_isplatformActivated)
            {
                _isplatformActivated = true; // Activate when touched
                ActivateLinkedPlatforms();
            }
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
