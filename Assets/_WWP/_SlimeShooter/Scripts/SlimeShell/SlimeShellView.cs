using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeShellView
{
    private SlimeShellModel _model;

    private GameObject _slimeObject;

    public SlimeShellView(SlimeShellModel model)
    {
        _model = model;

        _slimeObject = GameObject.Instantiate(SlimeShooterManager.Instance.slimePrefab, SlimeShooterManager.Instance.gameObjectContainer);

        _slimeObject.transform.position = model.Transform.position;
    }

    public void Update()
    {
        if (_model.Active)
            _slimeObject.transform.position = _model.Transform.position;
        else
        {
            if (_slimeObject != null)
                GameObject.Destroy(_slimeObject);
        }
    }
}
