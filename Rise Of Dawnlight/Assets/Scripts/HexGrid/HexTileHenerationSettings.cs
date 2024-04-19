using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TileGen/GenerationSettings")]
public class HexTileHenerationSettings : ScriptableObject
{
   public enum TileType
	{
		Grass,
		Water,
		Sand,
		Swamp,
		Rocks,
		Enemy,

	}

	public GameObject Grass;
	public GameObject Water;
	public GameObject Sand;
	public GameObject Swamp;
	public GameObject Rocks;
	public GameObject Enemy;

	public WeightedRandomList<TileType> tiles;

	

	public GameObject GetTile(out TileType type, bool replace = false)
	{
		TileType tileType = tiles.GetRandom();
		switch (tileType)
		{
			case TileType.Grass:
				type = TileType.Grass;
				return Grass;
			case TileType.Water:
				type = TileType.Water;
				return Water;
			case TileType.Sand:
				type = TileType.Sand;
				return Sand;
			case TileType.Swamp:
				type = TileType.Swamp;
				return Swamp;
			case TileType.Rocks:
				type = TileType.Rocks;
				return Rocks;
			case TileType.Enemy:
				type = TileType.Enemy;
				return Enemy;

		}
		type = 0;
		return null;
	}


	public bool IsWalkable(TileType type)
	{
		if(TileType.Grass == type || TileType.Sand == type || TileType.Swamp == type|| TileType.Enemy == type) return true;
		return false;
	}
}
