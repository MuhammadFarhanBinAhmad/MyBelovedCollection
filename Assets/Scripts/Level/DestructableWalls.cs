using System.Collections;
using UnityEngine;

public class DestructableWalls : MonoBehaviour, IResettable
{
    [SerializeField] private float rayDistance = 0.1f; // small buffer beyond the edge
    [SerializeField] private float rayOffset = 0.1f; // small buffer beyond the edge

    private bool isBeingDestroyed = false;
    private BoxCollider2D col;

    private void OnEnable()
    {
        RoomManager room = GetComponentInParent<RoomManager>();
        if (room != null)
            room.RegisterResettable(this);
    }

    private void OnDisable()
    {
        RoomManager room = GetComponentInParent<RoomManager>();
        if (room != null)
            room.UnregisterResettable(this);
    }

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        if (col == null)
            Debug.LogWarning($"{name} is missing a BoxCollider2D!");
    }

    private void CheckAllDirections()
    {
        if (col == null) return;

        // Get the collider bounds
        Bounds bounds = col.bounds;

        // Define ray start points at the edges
        Vector2 originUp = new Vector2(bounds.center.x, bounds.max.y + rayOffset);
        Vector2 originDown = new Vector2(bounds.center.x, bounds.min.y - rayOffset);
        Vector2 originLeft = new Vector2(bounds.min.x - rayOffset, bounds.center.y);
        Vector2 originRight = new Vector2(bounds.max.x + rayOffset, bounds.center.y);

        // Define directions and corresponding origins
        (Vector2 dir, Vector2 origin)[] rays = {
            (Vector2.up, originUp),
            (Vector2.down, originDown),
            (Vector2.left, originLeft),
            (Vector2.right, originRight)
        };

        foreach (var ray in rays)
        {
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.dir, rayDistance);

            Debug.DrawRay(ray.origin, ray.dir * rayDistance, hit ? Color.red : Color.green);

            if (hit.collider != null)
            {
                print($"{name} hit {hit.collider.name}");

                DestructableWalls otherWall = hit.collider.GetComponent<DestructableWalls>();

                // Make sure it's another wall and not already being destroyed
                if (otherWall != null && !otherWall.isBeingDestroyed)
                {
                    otherWall.DelayDestroy();
                }
            }
        }

        gameObject.SetActive(false);
    }
    void DelayDestroy()
    {
        StartCoroutine(WaitForDestroy());
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(.1f);
        DestroyWall();
    }

    public void DestroyWall()
    {
        if (isBeingDestroyed) return; // prevent infinite recursion
        isBeingDestroyed = true;

        CheckAllDirections();

        // Optional: Add VFX/SFX here
    }

    public void ResetObject()
    {
        isBeingDestroyed= false;
        gameObject.SetActive(true);
    }
}
