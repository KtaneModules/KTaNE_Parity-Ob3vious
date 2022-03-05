using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;

public abstract class Clickable
{
    private KMSelectable _selectable;
    private ParityScript _parent;

    public Clickable(KMSelectable selectable, ParityScript parent)
    {
        _selectable = selectable;
        _parent = parent;

        _selectable.OnInteract += delegate { OnPress(); return false; };
        _selectable.OnInteractEnded += delegate { OnRelease(); };
    }

    protected void UpdateParent()
    {
        _parent.UpdateModule();
    }

    protected void PlaySound(string sound, Transform transform)
    {
        _parent.Audio.PlaySoundAtTransform(sound, transform);
    }

    protected abstract void OnPress();

    protected abstract void OnRelease();

    public abstract void Update(float deltaTime);
}
