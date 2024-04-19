using UnityEngine;

public class HexTile : MonoBehaviour
{
	public HexTileHenerationSettings settings;

	public HexTileHenerationSettings.TileType tileType;

	public GameObject tile;

	public bool isWalkable = false;

	public bool isDirty = false;

	public int x;
	public int z;






	public GameObject AddTile(Vector3 position, Transform parent, int x, int z)
	{
		tile = Instantiate(settings.GetTile(out HexTileHenerationSettings.TileType type), position, Quaternion.identity);
		tileType = type;
		isWalkable = settings.IsWalkable(tileType);
		tile.transform.SetParent(parent);
		this.x = x;
		this.z = z;
		return tile;

	}

	public GameObject ReplaceTileWithEnemy(Vector3 position, Transform parent)
	{
		Destroy(tile);
		tile = null;
		tile = Instantiate(settings.Enemy, position, Quaternion.identity);
		tileType = HexTileHenerationSettings.TileType.Enemy;
		isWalkable = settings.IsWalkable(tileType);
		tile.transform.SetParent(parent);
		return tile;
	}
}
