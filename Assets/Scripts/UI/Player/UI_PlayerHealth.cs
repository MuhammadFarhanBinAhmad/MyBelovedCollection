using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PlayerHealth : MonoBehaviour, I_UIObserver
{

    UIManager _UIManager;

    [Header("PlayerUI")]
    [SerializeField] Image _HealthBar;
    [SerializeField] TextMeshProUGUI _HealthText;

    public void UpdatePlayerUI()
    {
        _HealthText.text = PlayerManager.Instance._Health.ToString() + " / " + PlayerManager.Instance._BaseHealth.ToString();
        _HealthBar.fillAmount = (float)((float)PlayerManager.Instance._Health / (float)PlayerManager.Instance._BaseHealth);
    }

    void OnEnable()
    {
        _UIManager = FindAnyObjectByType<UIManager>();
        _UIManager.AddObserver(this);
    }
    private void OnDisable()
    {
        _UIManager.RemoveObserver(this);
    }

}
