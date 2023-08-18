using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPrefabComponent : MonoBehaviour
{
    public Image healthbar;

    public Transform healthLoseTextPoint;

    public TextMeshPro healthLoseText;

    private GameObject _healthLoseObject;

    public void SpawnHealthText(float value)
    {
        var text = Instantiate(healthLoseText);
        Destroy(text.gameObject, 1f);
        text.gameObject.transform.position = healthLoseTextPoint.position;
        text.text = value.ToString("#0");
        _healthLoseObject = text.gameObject;
    }

    private void FixedUpdate()
    {
        if (_healthLoseObject != null)
        {
            _healthLoseObject.transform.position += new Vector3(0, 0.5f * Time.deltaTime, 0);
        }
    }
}
