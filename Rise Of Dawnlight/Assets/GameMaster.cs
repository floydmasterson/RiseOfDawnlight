using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
	[SerializeField] private ComabtManager comabtManager;
	[SerializeField] private List<GameObject> players;
	[SerializeField] private EnemySpanwer enemySpanwer;
	[SerializeField] private HexGrid hexGid;
	[SerializeField] private GameObject table;
	[SerializeField] private GameObject board;

	private void Awake()
	{
		comabtManager.gameMaster = this;
		hexGid.gameMaster = this;
		enemySpanwer.gameMaster = this;
	}
	public void TrasitionToFight(EnemyHex enemy)
	{
		Debug.Log($"Trigger fight with {enemy.name}");
		board.SetActive(false);
		table.SetActive(true);
		comabtManager.BeginFight(enemy);
	}

	public void TranistionOutOfFight()
	{
		table.SetActive(false);
		board.SetActive(true);
	}
}
