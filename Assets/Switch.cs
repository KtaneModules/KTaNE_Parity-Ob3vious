using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Clickable
{
    private float _visualState = 0.5f;
    private int _state = 0;

    private Transform _switch;
    private MeshRenderer _led;

    public Switch(KMSelectable selectable, Transform transform, MeshRenderer led, ParityScript parent) : base(selectable, parent)
    {
        _switch = transform;
        _led = led;

        Update(0);
    }

    public int GetState()
    {
        return _state;
    }

    protected override void OnPress()
    {
        _state = 1 - _state;
    }

    protected override void OnRelease()
    {
        return;
    }

    public override void Update(float deltaTime)
    {
        if (_visualState == _state)
            return;

        // set the animation speed
        deltaTime /= 0.0625f;

        bool isOn = _visualState > 0.5f;

        _visualState = Mathf.Clamp(_visualState + Mathf.Sign(_state - _visualState) * deltaTime, 0, 1);
        
        _switch.localEulerAngles = new Vector3(0, -90, (_visualState * 2 - 1) * 7.5f);

        // when a toggle happens
        if (isOn != _visualState > 0.5f)
        {
            _led.material.color = new Color(_state * 0.4375f, _state * 0.875f, 0);
            PlaySound("Switch" + (2 - _state), _switch);
        }
    }
}
