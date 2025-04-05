using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public string Tag { get; }
    public float Health { set; get; }
    public bool Targetable { set; get; }
    public bool Invincible { set; get; }
    public void OnHit(float damage, Vector3 knockback);
    public void OnHit(float damage, float knockback);
    public void OnHit(float damage);
    public void OnObjectDestroyed();
}
