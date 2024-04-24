using CM_Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{


	[SerializeField] private Transform pfHex;
	[SerializeField] private Transform pfUnwalkable;
	[SerializeField] public int height;
	[SerializeField] public int width;


	[SerializeField] private float cellSize = 1f;
	public HexTileHenerationSettings settings;


	public GridHexXZ<GridObject> gridHexXZ;
	private GridObject lastGridObject;
	private PathfindingHexXZ pathfindingHexXZ;

	public List<GameObject> hexs = new List<GameObject>();

	[SerializeField] private EnemySpanwer enemySpanwer;

	public GameMaster gameMaster;

	public Transform gridHolder;

	

	public class GridObject
	{
		public Transform visualTransform;

		public void Show()
		{
			visualTransform.Find("Selected").gameObject.SetActive(true);
		}

		public void Hide()
		{
			visualTransform.Find("Selected").gameObject.SetActive(false);
		}

	}

	private void Start()
	{
		enemySpanwer.gridSpawner = this;
		gridHexXZ = new GridHexXZ<GridObject>(width, height, cellSize, Vector3.zero, (GridHexXZ<GridObject> g, int x, int y) => new GridObject());
		pathfindingHexXZ = new PathfindingHexXZ(width, height, cellSize);
		Generate();

	}
	private void Generate()
	{
		Clear();

		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z < height; z++)
			{
				GameObject tile = new GameObject($"Hex C{x},R{z}");
				HexTile hexTile = tile.AddComponent<HexTile>();
				hexTile.settings = settings;
				GameObject hex = hexTile.AddTile(gridHexXZ.GetWorldPosition(x, z), tile.transform, x, z);
				hexs.Add(tile);
				pathfindingHexXZ.GetNode(x, z).SetIsWalkable(hexTile.isWalkable);
				Transform visualTransform = hex.transform;


				gridHexXZ.GetGridObject(x, z).visualTransform = visualTransform;
				gridHexXZ.GetGridObject(x, z).Hide();
				tile.transform.SetParent(gridHolder);
			}
		}
		enemySpanwer.SpawnEnemy();
	}

	public void Clear()
	{
		List<GameObject> children = new List<GameObject>();

		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject child = transform.GetChild(i).gameObject;
			children.Add(child);
		}

		foreach (GameObject child in children)
		{
			DestroyImmediate(child, true);
		}
		lastGridObject = null;
	}

	private void Update()
	{
		if (lastGridObject != null)
		{
			lastGridObject.Hide();
		}

		lastGridObject = gridHexXZ.GetGridObject(Mouse3D.GetMouseWorldPosition());

		if (lastGridObject != null)
		{
			lastGridObject.Show();
		}
	}
}