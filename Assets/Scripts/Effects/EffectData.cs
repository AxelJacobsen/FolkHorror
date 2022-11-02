using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base of an effect, defining what functions are the bare minimum.
/// </summary>
public abstract class EffectData : ScriptableObject
{
    [Header("Stats")]
    public float        BaseIntensity = 1f,
                        BaseDuration  = 1f;

    [Header("Particle effects")]
    public EffectEmitter effectEmitter;

    [Header("Set by scripts")]
    public GameObject   _Creator,
                        _Target;
    public float        _Intensity,
                        _Duration;

    /// <summary>
    /// How the effect acts when first applied.
    /// (Re-applying the effect while it's already running won't trigger this.)
    /// </summary>
    public virtual void OnBegin() 
    {
        // Fetch hitbox
        foreach (Transform child in _Target.transform) {
            if (child.gameObject.tag != "Hitbox") continue;
            BoxCollider childBoxCollider = child.gameObject.GetComponent<BoxCollider>();
            if (childBoxCollider == null) continue;
            effectEmitter._Hitbox = childBoxCollider;
            break;
        }
        if (effectEmitter._Hitbox == null) effectEmitter._Hitbox = _Target.GetComponent<BoxCollider>();
        if (effectEmitter._Hitbox == null) Debug.LogError("Effect could not find its target's hitbox!");

        // Set active
        effectEmitter._Active = true;
    }

    /// <summary>
    /// How the effect acts when reapplied.
    /// </summary>
    public virtual void OnReapply()
    {

    }

    /// <summary>
    /// How the effect acts between start and end.
    /// </summary>
    /// <param name="deltaTime">Deltatime in seconds.</param>
    public virtual void During(float deltaTime) 
    {
        effectEmitter.Emit(deltaTime);
    }

    /// <summary>
    /// How the effect acts when it ends.
    /// </summary>
    public virtual void OnEnd() 
    {
        effectEmitter._Active = false;
    }
}
