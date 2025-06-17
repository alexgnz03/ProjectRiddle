using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameController : MonoBehaviour
{
    [Header("Referencias UI")]
    public RectTransform miniCanvasRect;    // El contenedor Image donde sucede todo
    public RectTransform playerRect;        // El personaje (imagen UI)
    public RectTransform bulletPrefab;      // Prefab bala (imagen UI)
    public RectTransform enemyPrefab;       // Prefab enemigo (imagen UI)

    [Header("Configuración")]
    public float playerSpeed = 300f;        // px/s
    public float bulletSpeed = 500f;        // px/s
    public int enemyCount = 3;

    private int trueAnswerID;

    private List<MinigameEnemy> enemies = new List<MinigameEnemy>();
    private List<RectTransform> bullets = new List<RectTransform>();

    private float enemyYPos;  // altura fija donde están los enemigos dentro del miniCanvas

    public GameObject wrongObject;

    void Start()
    {
        //Desactivar Dialogue
        Enemy.Instance.DialogueEnabled(false);

        // Crear enemigos y posicionarlos al inicio con espaciado en X y Y
        float xSpacing = miniCanvasRect.rect.width / (enemyCount + 1);
        float yStart = miniCanvasRect.rect.height * 0.6f;  // altura base
        float ySpacing = 30f; // espacio vertical entre enemigos

        //Elegir de forma random cual de los enemies va a ser el correcto
        trueAnswerID = Random.Range(1, enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            RectTransform enemy = Instantiate(enemyPrefab, miniCanvasRect);

            float x = xSpacing * (i + 1) - miniCanvasRect.rect.width / 2f;
            float y = yStart + i * ySpacing - miniCanvasRect.rect.height / 2f;

            enemy.anchoredPosition = new Vector2(x, y);
            enemies.Add(new MinigameEnemy(enemy));
        }

        //Cambiar color de la imagen
        Image enemyImage;
        enemyImage = enemies[trueAnswerID].rect.GetComponent<Image>();
        enemyImage.color = Color.green;

        // Iniciar cambio random velocidad enemigo cada 2 segundos
        StartCoroutine(ChangeEnemySpeedsRoutine());
    }

    void Update()
    {
        HandlePlayerMovement();
        HandleShooting();
        UpdateBullets();
        UpdateEnemies();
        CheckBulletEnemyCollisions();
    }

    // ---- PLAYER ----
    void HandlePlayerMovement()
    {
        float inputX = Input.GetAxis("Horizontal"); // A/D o flechas
        Vector2 pos = playerRect.anchoredPosition;
        pos.x += inputX * playerSpeed * Time.deltaTime;

        // Limitar al ancho del miniCanvas
        float halfWidth = miniCanvasRect.rect.width / 2f;
        float playerHalfWidth = playerRect.rect.width / 2f;
        pos.x = Mathf.Clamp(pos.x, -halfWidth + playerHalfWidth, halfWidth - playerHalfWidth);

        playerRect.anchoredPosition = pos;
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShootBullet();
        }
    }

    void ShootBullet()
    {
        RectTransform bullet = Instantiate(bulletPrefab, miniCanvasRect);
        // Posicionar bala encima del jugador (y un poco arriba)
        Vector2 bulletPos = playerRect.anchoredPosition;
        bulletPos.y += playerRect.rect.height / 2f + bullet.rect.height / 2f;
        bullet.anchoredPosition = bulletPos;
        bullets.Add(bullet);
    }

    // ---- BULLETS ----
    void UpdateBullets()
    {
        float halfHeight = miniCanvasRect.rect.height / 2f;
        float bulletHalfHeight = bulletPrefab.rect.height / 2f;

        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            RectTransform b = bullets[i];
            Vector2 pos = b.anchoredPosition;
            pos.y += bulletSpeed * Time.deltaTime;
            b.anchoredPosition = pos;

            // Si sale del miniCanvas hacia arriba, destruir
            if (pos.y > halfHeight - bulletHalfHeight)
            {
                Destroy(b.gameObject);
                bullets.RemoveAt(i);
            }
        }
    }

    // ---- ENEMIES ----
    void UpdateEnemies()
    {
        float halfWidth = miniCanvasRect.rect.width / 2f;

        foreach (MinigameEnemy e in enemies)
        {
            Vector2 pos = e.rect.anchoredPosition;
            pos.x += e.speed * Time.deltaTime;

            float enemyHalfWidth = e.rect.rect.width / 2f;
            // Rebotar en los bordes del miniCanvas
            if (pos.x > halfWidth - enemyHalfWidth)
            {
                pos.x = halfWidth - enemyHalfWidth;
                e.speed = -e.speed;
            }
            else if (pos.x < -halfWidth + enemyHalfWidth)
            {
                pos.x = -halfWidth + enemyHalfWidth;
                e.speed = -e.speed;
            }

            e.rect.anchoredPosition = pos;
        }
    }

    IEnumerator ChangeEnemySpeedsRoutine()
    {
        while (true)
        {
            foreach (MinigameEnemy e in enemies)
            {
                // Cambiar velocidad random entre -200 y 200, evitando valores muy bajos (para que siempre se muevan)
                float newSpeed = Random.Range(200f, 400f);
                // Alternar dirección para variar más
                newSpeed *= (Random.value > 0.5f) ? 1f : -1f;
                e.speed = newSpeed;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    // ---- COLISIONES ----
    void CheckBulletEnemyCollisions()
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            RectTransform b = bullets[i];
            Rect bulletRect = GetWorldRect(b);

            bool bulletDestroyed = false;

            for (int j = enemies.Count - 1; j >= 0; j--)
            {
                MinigameEnemy e = enemies[j];
                Rect enemyRect = GetWorldRect(e.rect);

                if (bulletRect.Overlaps(enemyRect))
                {
                    // Impacto: destruir bala y enemigo
                    Destroy(b.gameObject);
                    bullets.RemoveAt(i);
                    int currentPhaseIndex = Enemy.Instance.currentPhaseIndex;

                    if (e.rect.GetComponent<Image>().color == Color.green)
                    {
                        FightStateController.Instance.AddProgress(10);
                        Enemy.Instance.ShowObject(Enemy.Instance.dialogueData.phases[currentPhaseIndex].correctObject);
                        Enemy.Instance.DialogueEnabled(true);                       
                    }
                    else
                    {
                        FightStateController.Instance.AddProgress(-10);
                        Enemy.Instance.ShowObject(wrongObject);
                        Enemy.Instance.DialogueEnabled(true);
                    }

                    Destroy(e.rect.gameObject);
                    enemies.RemoveAt(j);

                    Destroy(gameObject);

                    bulletDestroyed = true;
                    break;
                }
            }

            if (bulletDestroyed)
                break;
        }
    }

    // Convierte RectTransform a rectángulo en coordenadas del mundo para chequeo de colisión
    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];
        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    // Clase para controlar enemigos
    class MinigameEnemy
    {
        public RectTransform rect;
        public float speed;

        public MinigameEnemy(RectTransform r)
        {
            rect = r;
            speed = Random.Range(200f, 400f);
            speed *= (Random.value > 0.5f) ? 1f : -1f;
        }
    }
}