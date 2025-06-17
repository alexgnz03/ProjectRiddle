using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour, AnimationInterface
{

    [SerializeField] private Sprite[] UpAnimationFrames;
    [SerializeField] private Sprite[] DownAnimationFrames;
    [SerializeField] private Sprite[] LeftAnimationFrames;
    [SerializeField] private Sprite[] RightAnimationFrames;
    [SerializeField] private float frameRate = 10f;

    private int currentFrame;

    private SpriteRenderer spriteRenderer;

    private float timer;

    private Dictionary<Direction, Sprite[]> animationFramesMap;
    private Sprite[] currentAnimationFrames;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        animationFramesMap = new Dictionary<Direction, Sprite[]>
        {
            { Direction.Up, UpAnimationFrames },
            { Direction.Down, DownAnimationFrames },
            { Direction.Left, LeftAnimationFrames },
            { Direction.Right, RightAnimationFrames }
        };

        // Valor por defecto
        currentAnimationFrames = DownAnimationFrames;
    }

    public void AnimationStart(Direction direction)
    {
        if (!animationFramesMap.TryGetValue(direction, out currentAnimationFrames) || currentAnimationFrames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame++;
            if (currentFrame >= currentAnimationFrames.Length || currentFrame == 0)
            {
                currentFrame = 1;
            }
            spriteRenderer.sprite = currentAnimationFrames[currentFrame];
        }
    }

    public void AnimationStop() 
    {
        currentFrame = 0;
        spriteRenderer.sprite = currentAnimationFrames[currentFrame];
    }
}
