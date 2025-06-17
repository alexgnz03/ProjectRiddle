using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightStateController : MonoBehaviour
{
    public static FightStateController Instance { get; private set; }

    private float progress;
    private float lerpTimer;
    [Header("VSBar")]
    public float maxProgress = 100;
    public float chipSpeed = 2f;
    public Image VSBar;

    //EnemySprite
    public Image enemyUIImage;
    public Sprite losingSprite;  
    public Sprite winningSprite;

    public bool onFight = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        progress = 50;
    }

    // Update is called once per frame
    void Update()
    {

        progress = Mathf.Clamp(progress, 0, maxProgress);
        UpdateProgressUI();
    }

    public void UpdateProgressUI()
    {
        float fillF = VSBar.fillAmount;
        float hFraction = progress / maxProgress;
        VSBar.fillAmount = hFraction;
    }

    public void AddProgress(float AddedProgress)
    {
        Debug.Log("Se ha añadido progreso");
        progress += AddedProgress;
        UpdateEnemyUIImage();
    }

    void UpdateEnemyUIImage()
    {
        if (progress <= 50)
        {
            enemyUIImage.sprite = losingSprite;
        }
        else
        {
            enemyUIImage.sprite = winningSprite;
        }
    }
}
