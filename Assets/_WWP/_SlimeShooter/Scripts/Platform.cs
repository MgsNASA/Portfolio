using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : Singleton<Platform>
{
    [Header("Environment Materials")]
    [SerializeField] private Material _backgroundMat;
    [SerializeField] private Material _platformMat;

    public void MoveGround(float movementAmount)
    {
        MoveBackground(-movementAmount / 8);
        MovePlatform(movementAmount);
    }

    private void MoveBackground(float movementAmount)
    {
        MoveTexture(movementAmount, _backgroundMat);
    }

    private void MovePlatform(float movementAmount)
    {
        MoveTexture(movementAmount, _platformMat);
    }

    private void MoveTexture(float movementAmount, Material material)
    {
        Vector2 offset = material.GetTextureOffset("_MainTex");
        offset.x += movementAmount;
        material.SetTextureOffset("_MainTex", offset);

        if (offset.x >= 1f)
            ResetOffset(material);
    }

    public void Restart()
    {
        ResetOffset();
    }

    private void ResetOffset()
    {
        ResetOffset(_backgroundMat);
        ResetOffset(_platformMat);
    }

    private void ResetOffset(Material material)
    {
        material.SetTextureOffset("_MainTex", Vector2.zero);
    }

    public void EndGame()
    {

    }
}
