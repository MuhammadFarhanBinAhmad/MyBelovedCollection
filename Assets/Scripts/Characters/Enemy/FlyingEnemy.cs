using System.Collections;
using UnityEngine;

public class FlyingEnemy : BaseEnemy
{
    [Header("Flying Enemy Settings")]
    [SerializeField] private float homingSpeed = 4f;        // Speed during homing phase
    [SerializeField] private float dashSpeed = 12f;         // Speed during dash
    [SerializeField] private float homingDuration = 1.0f;   // How long it homes before dashing
    [SerializeField] private float dashDuration = 0.3f;     // How long the dash lasts
    [SerializeField] private float attackCooldown = 1.5f;   // Delay before another attack

    private bool _isAttacking = false;
    private bool _canAttack = true;

    public override void DetectPlayer()
    {
        if (_target == null)
            _target = PlayerManager.Instance.gameObject;

        float distanceToPlayer = Vector2.Distance(transform.position, _target.transform.position);

        // Detection purely by distance
        if (distanceToPlayer <= _viewRange)
        {
            _state = STATE.ATTACKING;
        }
    }

    public override void ChasePlayer()
    {
        if (_isAttacking || !_canAttack)
            return;

        StartCoroutine(HomingAndDashRoutine());
    }

    private IEnumerator HomingAndDashRoutine()
    {
        _isAttacking = true;
        _canAttack = false;

        if (_target == null)
            _target = PlayerManager.Instance.gameObject;

        float timer = 0f;

        // --- Homing Phase ---
        while (timer < homingDuration)
        {
            if (_target == null) break;

            Vector2 direction = (_target.transform.position - transform.position).normalized;
            _rigidbody.linearVelocity = direction * homingSpeed;

            // Flip sprite to face direction
            if (direction.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // --- Dash Phase ---
        Vector2 dashTargetPos = _target.transform.position;
        Vector2 dashDir = (dashTargetPos - (Vector2)transform.position).normalized;

        _rigidbody.linearVelocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        // Stop after dash
        _rigidbody.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(attackCooldown);

        _isAttacking = false;
        _canAttack = true;
    }

    public new void ResetObject()
    {
        base.ResetObject();

        StopAllCoroutines();
        _isAttacking = false;
        _canAttack = true;
        _rigidbody.linearVelocity = Vector2.zero;
    }
}
