using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("PauseBG")]
    public GameObject _pauseMenu;

    private void Start()
    {
        _pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!_pauseMenu.activeInHierarchy);
        }
    }

    void SetPause(bool ispause)
    {
        if(!ispause)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;

        _pauseMenu.SetActive(ispause);
    }
    public void ResumeGame()
    {
        SetPause(false);
    }
}
