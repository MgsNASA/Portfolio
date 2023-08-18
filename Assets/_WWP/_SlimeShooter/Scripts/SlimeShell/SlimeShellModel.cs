using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WWP.Game;

public class SlimeShellModel
{
    private SlimeConfig _config;
    public MyTransform Transform { get; private set; }

    private Unit _target;
    private SlimeShellGameObject _slimeObject;

    private SlimeShellView _view;

    public bool Active { get; private set; }

    public SlimeShellModel (Unit target, MyTransform initialTransform, HitConfig hitConfig)
    {
        Active = true;

        Transform = new MyTransform(initialTransform);
        _target = target;

        _config = SlimeShooterManager.Instance.slimeConfig;
        _config.hitConfig = hitConfig;

        _view = new(this);

        _slimeObject = new GameObject().AddComponent<SlimeShellGameObject>();
        _slimeObject.StartCoroutine(Update());
        _target.Died += Recycle;
        SlimeShooterManager.Instance.RestartEvent += Recycle;
    }

    public SlimeShellModel()
    {
        Active = false;
    }

    private IEnumerator Update()
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _view.Update();
            if (MoveTo())
            {
                _target.TakeHit(_config.hitConfig);
                break;
            }
        }

        Recycle();
    }

    private bool MoveTo()
    {
        float deltaMovement = _config.speed * Time.deltaTime;

        if (_target == null) return false;

        if (Vector3.Distance(this.Transform.position, _target.Transform.position) < 0.05f)
        {
            return true;
        }

        this.Transform.position = Vector3.MoveTowards(this.Transform.position, _target.Transform.position, deltaMovement);

        return false;
    }

    private void Recycle(Unit model, bool inProcess)
    {
        Recycle();
    }

    private void Recycle()
    {
        Active = false;
        if (_slimeObject != null && _slimeObject.gameObject != null) GameObject.Destroy(_slimeObject.gameObject);
        _view.Update();
    }

    private class SlimeShellGameObject : MonoBehaviour
    {

    }
}


[Serializable]
public class SlimeConfig
{
    public HitConfig hitConfig;
    public float speed;
}