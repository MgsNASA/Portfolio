using UnityEngine;

public class ScalingManager : MonoBehaviour {
    public int defaultWidth = 1040;
    public int defaultHeight = 2080;

    private void Start ( ) {
        ScaleSprites ();
    }

    private void ScaleSprites ( ) {
        SpriteRenderer [] spriteRenderers = FindObjectsOfType<SpriteRenderer> ();
        Vector3 scale = new Vector3 (Screen.width * 1.0f / defaultWidth, Screen.height * 1.0f / defaultHeight, 1f);

        foreach ( SpriteRenderer spriteRenderer in spriteRenderers ) {
            spriteRenderer.transform.localScale = Vector3.Scale (spriteRenderer.transform.localScale, scale);
            spriteRenderer.transform.position = Vector3.Scale (spriteRenderer.transform.position, scale);
        }
    }
}
