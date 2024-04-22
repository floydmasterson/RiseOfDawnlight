using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityManager : MonoBehaviour
{
	public abstract void SetBaseStats();
	public abstract void TakeDamage(int damage);
	public abstract void Heal(int healing);
	public abstract void ApplyStatus(StatusEffectSO statusEffect);
	public abstract void RemoveStatus(StatusEffectSO statusEffect);
}
