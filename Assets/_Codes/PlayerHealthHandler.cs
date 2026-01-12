using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthHandler : MonoBehaviour
{
    [SerializeField]
    public int PlayerMaxHealth;
    [SerializeField]
    public int PlayerCurrentHealth;

    private PlayerController _PlayerController;
    private ConstantForce spinForce;   // 🔹 ĐỔI TÊN (FIX WARNING)

    public GameObject pickupParticle;
    public Transform topPlayerHead;

    public Image HeartIcon;
    Transform HeartPanel;
    public Color FillColor, EmptyColor;

    public GameObject DeathParticle;

    Respawner spawner;

    private bool hasRespawned = false;

    // ===== CONSTANT FORCE SETTING =====
    public float spinDuration = 1.0f;
    // ==================================

    void Start()
    {
        _PlayerController = GetComponent<PlayerController>();

        spinForce = GetComponent<ConstantForce>(); // 🔹 FIX
        if (spinForce != null)
            spinForce.enabled = false;

        spawner = GameObject.FindGameObjectWithTag("Respawn")
            .GetComponent<Respawner>();
        spawner.SetPosition(transform.position);

        HeartPanel = GameObject.Find("HeartPanel").transform;

        for (int i = 0; i < PlayerMaxHealth; i++)
        {
            Image Icon = Instantiate(HeartIcon, HeartPanel);
            Icon.color = FillColor;
        }

        PlayerCurrentHealth = PlayerMaxHealth;
    }

    void Update()
    {
        if (PlayerCurrentHealth <= 0)
        {
            if (spinForce != null)
                spinForce.enabled = false;

            spawner.playerIsDead();
            Instantiate(DeathParticle, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        if (transform.position.y < -40 && !hasRespawned)
        {
            PlayerCurrentHealth = 0;
            Debug.Log("Player position below -40!");
        }

        UpdateHearts();
    }

    public void GainMaxHealth()
    {
        GameObject clone = Instantiate(pickupParticle, topPlayerHead.position, topPlayerHead.rotation);
        clone.transform.SetParent(topPlayerHead);
        PlayerMaxHealth += 1;
        ResetHealth();
    }

    public void GainHealth(int amount)
    {
        PlayerCurrentHealth += amount;
        GameObject clone = Instantiate(pickupParticle, topPlayerHead.position, topPlayerHead.rotation);
        clone.transform.SetParent(topPlayerHead);
        if (PlayerCurrentHealth > PlayerMaxHealth)
        {
            PlayerCurrentHealth = PlayerMaxHealth;
        }
        SavePlayerHealth();
    }

    public void LoseHealth(int amount, Vector3 push)
    {
        PlayerCurrentHealth -= amount;
        if (PlayerCurrentHealth < 0)
        {
            PlayerCurrentHealth = 0;
        }

        _PlayerController.controller.Move(-push);
        _PlayerController.flashRed();

        // ===== BẬT QUAY KHI MẤT MÁU =====
        if (spinForce != null)
        {
            StopCoroutine(nameof(SpinRoutine));
            StartCoroutine(SpinRoutine());
        }
        // =================================

        SavePlayerHealth();
    }

    IEnumerator SpinRoutine()
    {
        spinForce.enabled = true;
        yield return new WaitForSeconds(spinDuration);
        spinForce.enabled = false;
    }

    public void ResetHealth()
    {
        PlayerCurrentHealth = PlayerMaxHealth;
        Image Icon = Instantiate(HeartIcon, HeartPanel);
        Icon.color = FillColor;
    }

    void UpdateHearts()
    {
        Image[] icons = HeartPanel.GetComponentsInChildren<Image>();
        int numIcons = Mathf.Min(PlayerMaxHealth, icons.Length - 1);

        for (int n = 0; n < PlayerMaxHealth; n++)
        {
            if (n < PlayerCurrentHealth)
            {
                icons[n + 1].color = FillColor;
            }
            else
            {
                icons[n + 1].color = EmptyColor;
            }
        }
    }

    private void SavePlayerHealth()
    {
        PlayerPrefs.SetInt("PlayerMaxHealth", PlayerMaxHealth);
        PlayerPrefs.SetInt("PlayerCurrentHealth", PlayerCurrentHealth);
        PlayerPrefs.Save();
    }

    private void LoadPlayerHealth()
    {
        PlayerMaxHealth = PlayerPrefs.GetInt("PlayerMaxHealth", PlayerMaxHealth);
        PlayerCurrentHealth = PlayerPrefs.GetInt("PlayerCurrentHealth", PlayerMaxHealth);
    }
}
