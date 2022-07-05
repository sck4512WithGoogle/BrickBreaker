using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

[DisallowMultipleComponent]
public class AnimationPlayer : MonoBehaviour
{
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float keyFrame = 0.0416f;

    private void OnEnable()
    {
        StartCoroutine(PlayAnimation());
    }
    private IEnumerator PlayAnimation()
    {
        for (int i = 0; i < atlas.spriteCount; i++)
        {
            spriteRenderer.sprite = atlas.GetSprite(i.ToString());
            yield return new WaitForSecondsRealtime(keyFrame);
        }


        spriteRenderer.sprite = null;
    }
}
