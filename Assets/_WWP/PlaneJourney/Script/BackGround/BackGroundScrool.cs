using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrool : MonoBehaviour {
    [SerializeField]
    private List<Material> materials;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float materialChangeInterval = 30f; // Интервал смены материалов (в секундах)

    private int currentMaterialIndex = 0;
    private Renderer _meshRenderer;
    private float timer = 0f;

    private float targetAlpha = 0f;
    private bool isTransitioning = false;
    private float transitionDuration = 1.0f; // Длительность плавного перехода (в секундах)

    private void Start ( ) {
        _meshRenderer = GetComponent<Renderer> ();
        targetAlpha = 0f;
    }

    private void Update ( ) {
        Vector2 offset = _meshRenderer.material.mainTextureOffset;
        offset = offset + new Vector2 (0, speed * Time.deltaTime);
        _meshRenderer.material.mainTextureOffset = offset;

        timer += Time.deltaTime;
        if ( timer >= materialChangeInterval ) {
            SwitchMaterial ();
            timer = 0f;
        }

        if ( isTransitioning ) {
            Color currentColor = _meshRenderer.material.color;
            float transitionProgress = Mathf.Min (1f, transitionDuration > 0 ? Time.deltaTime / transitionDuration : 1f);

            Color newColor = currentColor;
            newColor.a = Mathf.Lerp (currentColor.a, targetAlpha, transitionProgress);

            _meshRenderer.material.color = newColor;

            if ( Mathf.Abs (newColor.a - targetAlpha) < 0.05f ) // Примените окончательные настройки, когда альфа близка к целевому значению
            {
                _meshRenderer.material = materials [currentMaterialIndex];
                isTransitioning = false;
            }
        }
    }

    private void SwitchMaterial ( ) {
        currentMaterialIndex = ( currentMaterialIndex + 1 ) % materials.Count;
        targetAlpha = 0f;
        isTransitioning = true;
    }
}
