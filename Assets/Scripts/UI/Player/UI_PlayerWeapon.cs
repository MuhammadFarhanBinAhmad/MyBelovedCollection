using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UI_PlayerWeapon : MonoBehaviour , I_UIObserver
{
    Player_Weapon _PlayerWeapon;
    UIManager _UIManager;

    GameObject[] _AmmoImage;

    [Header("PlayerUI")]
    [SerializeField] TextMeshProUGUI _AmmoLeftText;
    [SerializeField] GameObject _BulletImage;
    [SerializeField] Transform _BulletImageTransform;

    /// <summary>
    /// Update player's weapon UI stats
    /// </summary>
    public void UpdatePlayerUI()
    {
        //update ammo text value
        _AmmoLeftText.text = _PlayerWeapon.magazine_LeftAmount.ToString() + " / " + _PlayerWeapon.ammoreserve_CurrentAmount.ToString();
        
        //Update ammo image
        if (_PlayerWeapon.magazine_LeftAmount < _PlayerWeapon.magazine_CurrentAmount)
        {
            //Hide image
            _AmmoImage[_PlayerWeapon.magazine_LeftAmount].SetActive(false);
        }
        if(_PlayerWeapon.weapon_IsReloading)
        {
            //Unhide bullet image
            for (int i = 0; i < _PlayerWeapon.magazine_BaseAmount; i++)
            {
                _AmmoImage[i].SetActive(true);
            }

        }
    }

    void Start()
    {
        _UIManager = FindAnyObjectByType<UIManager>();
        _PlayerWeapon = FindAnyObjectByType <Player_Weapon>();

        _UIManager.AddObserver(this);

        _AmmoImage = new GameObject[_PlayerWeapon.magazine_BaseAmount];

        RenderBulletImage();
    }

    void RenderBulletImage()
    {
        for (int i = 0; i < _PlayerWeapon.magazine_BaseAmount; i++)
        {
            //Set offset and position
            float offset = Mathf.Abs(_BulletImage.GetComponent<RectTransform>().sizeDelta.x) * i;
            Vector3 spawnpos = _BulletImageTransform.position;
            spawnpos.x -= offset;

            //Spawn image with offest position
            GameObject go = Instantiate(_BulletImage, spawnpos, _BulletImageTransform.rotation);
            _AmmoImage[i] = go;
            go.transform.parent = this.transform;
            go.transform.localScale = Vector3.one;
        }
    }

}
