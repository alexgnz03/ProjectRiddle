using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightUIController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject miniGamePrefab;          // El prefab del minijuego
    public Transform miniGameParent;           // Dónde se instancia (Canvas u otro panel)

    public Transform ButtonsPanel;

    public static FightUIController Instance;

    public GameObject FightElements;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SpawnMiniGame()
    {
        Instantiate(miniGamePrefab, miniGameParent);
    }

    public void IgnoreDialog()
    {
        Enemy.Instance.AdvanceToNextPhase();
    }

}
