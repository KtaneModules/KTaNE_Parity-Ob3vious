using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Clickable
{
    private float _visualState = 0.5f;
    private int _state = 0;

    private Transform _button;

    public Button(KMSelectable selectable, Transform transform, ParityScript parent) : base(selectable, parent)
    {
        _button = transform;

        Update(0);
    }

    protected override void OnPress()
    {
        _state = 1;
    }

    protected override void OnRelease()
    {
        _state = 0;
    }

    public override void Update(float deltaTime)
    {
        if (_visualState == _state)
            return;

        // set the animation speed
        deltaTime /= 0.0625f;

        bool isOn = _visualState > 0.5f;

        _visualState = Mathf.Clamp(_visualState + Mathf.Sign(_state - _visualState) * deltaTime, 0, 1);

        _button.localPosition = new Vector3(0, 0, (_visualState) * -0.005f);

        // when a toggle happens
        if (isOn != _visualState > 0.5f)
        {
            PlaySound("Button" + (2 - _state), _button);

            if (isOn)
                UpdateParent();
        }
    }
}
