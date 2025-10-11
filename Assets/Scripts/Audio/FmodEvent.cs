using UnityEngine;
using FMODUnity;

public class FmodEvent : MonoBehaviour
{
    [field: Header("Environment")]
    [field: SerializeField] public EventReference sfx_GunShot { get; private set; }
    [field: SerializeField] public EventReference sfx_DeflectProjectile { get; private set; }


    [field: Header("Traps")]
    [field: SerializeField] public EventReference sfx_TrapDoor { get; private set; }
    [field: SerializeField] public EventReference sfx_PopUpSpikeTrap { get; private set; }


    [field: Header("Player")]
    [field: SerializeField] public EventReference sfx_PlayerFootStep { get; private set; }
    [field: SerializeField] public EventReference sfx_PlayerDashing { get; private set; }
    [field: SerializeField] public EventReference sfx_PlayerDeath { get; private set; }
    [field: SerializeField] public EventReference sfx_PlayerJump { get; private set; }


    [field: Header("Enemy")]
    [field: SerializeField] public EventReference sfx_EnemyHit { get; private set; }
    [field: SerializeField] public EventReference sfx_EnemyDeflectHit { get; private set; }


    public static FmodEvent Instance {  get; private set; }

    private void Awake()
    {
        if (Instance != null)
            print("more than one Fmod Event instance in the scene");

        Instance = this;
    }
}
