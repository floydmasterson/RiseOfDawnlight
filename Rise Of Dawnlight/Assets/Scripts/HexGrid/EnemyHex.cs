using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHex : MonoBehaviour
{
    public Transform enemySpawnLocation;
	public GameObject selcetedEnemy;
	public GameMaster gameMaster;
	[SerializeField] private WeightedRandomList<GameObject> enemys;

	public GameObject SelectEnemy()
	{
		GameObject selcetedMonster = enemys.GetRandom();
		selcetedEnemy = selcetedMonster;
		return selcetedMonster;
	}

	public void TriggerFight()
	{
		gameMaster.TrasitionToFight(selcetedEnemy.GetComponent<EnemyData>().tableEnemy);
	}
}
