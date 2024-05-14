using System.Collections.Generic;
using UnityEngine;

public class EnemySpanwer : MonoBehaviour
{
    // Start is called before the first frame update
	public HexGrid gridSpawner;
	[SerializeField] int spawnAmount;
	public GameMaster gameMaster;


	private List<int> usedHexs = new List<int>();


	public void SpawnEnemy()
	{
		for (int i = 0; i < spawnAmount; i++)
		{
			int hex = Random.Range(0, gridSpawner.hexs.Count);

			if(HexHasBeenUsed(hex))
			{
				while(HexHasBeenUsed(hex))
				{
					hex = Random.Range(0, gridSpawner.hexs.Count);
				}
			}
			usedHexs.Add(hex);
			GameObject selctedHex = gridSpawner.hexs[hex];
			HexTile selctedHexTile = selctedHex.GetComponent<HexTile>();
			GameObject enemyHexTile = selctedHexTile.ReplaceTileWithEnemy(gridSpawner.gridHexXZ.GetWorldPosition(selctedHexTile.x, selctedHexTile.z), gridSpawner.hexs[hex].transform);
			Transform visualTransform = enemyHexTile.transform;
			gridSpawner.gridHexXZ.GetGridObject(selctedHexTile.x, selctedHexTile.z).visualTransform = visualTransform;
			gridSpawner.gridHexXZ.GetGridObject(selctedHexTile.x, selctedHexTile.z).Hide();
			selctedHex.transform.parent = gridSpawner.gridHolder;
			EnemyHex enemyHex = enemyHexTile.GetComponent<EnemyHex>();
			enemyHex.gameMaster = gameMaster;
			Vector3 spawnLocation = enemyHex.enemySpawnLocation.position;
			GameObject monster = Instantiate(enemyHex.SelectEnemy(), spawnLocation, Quaternion.identity);
			enemyHex.SpawnedEnemy = monster;
			monster.transform.localScale = new Vector3(.15f, .15f, .15f);
			monster.transform.SetParent(selctedHex.transform);
		}
	}

	private bool HexHasBeenUsed(int spawn)
	{
		if(usedHexs.Contains(spawn))
		{
			return true;
		}
		return false;
	}
}
