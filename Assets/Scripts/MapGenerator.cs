using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MapGenerator : MonoBehaviour
{
	public Vector2Int MapSize = new Vector2Int(50, 20);
	public float TileSize = 1f;
	public GameObject TilePrefab;
	public GameObject RiverPrefab;
	public GameObject ObstaclePrefab;
	public GameObject HideoutPrefab;
	public GameObject SpawnPrefab;
	public int ObstacleChunkNumber = 10;
	public int RiverChunkNumber = 10;
	public int SpawnPositionNumber = 4;
	public int HideoutNumber = 10;
	private static System.Random _rng = new System.Random(); 

	private string _mapHolderName = "Map Holder";

	public void GenerateMap()
	{
		if (TilePrefab == null) throw new ArgumentNullException();

		var oldHolder = transform.Find(_mapHolderName);
		if (oldHolder != null) DestroyImmediate(oldHolder.gameObject);

		var holder = new GameObject(_mapHolderName).transform;
		holder.parent = transform;

		// Create and shuffle coords
		var coords = new List<Vector2Int>(MapSize.x * MapSize.y);
		for (var x = 0; x < MapSize.x; x++)
		{
			for (var y = 0; y < MapSize.y; y++)
			{
				coords.Add(new Vector2Int(x, y));
			}
		}
		FisherYatesShuffle(coords);
		
		// Borders coords
		var borderCoords = new List<Vector2Int>();
		var horizontalBorderCoords = Enumerable.Range(0, MapSize.x);
		var verticalBorderCoords = Enumerable.Range(-1, MapSize.y + 2);
		borderCoords.AddRange(horizontalBorderCoords.Select(x => new Vector2Int(x, -1)));
		borderCoords.AddRange(horizontalBorderCoords.Select(x => new Vector2Int(x, MapSize.y)));
		borderCoords.AddRange(verticalBorderCoords.Select(y => new Vector2Int(-1, y)));
		borderCoords.AddRange(verticalBorderCoords.Select(y => new Vector2Int(MapSize.x, y)));

		void InstantiatePrefab(IEnumerable<Vector2Int> _coords, GameObject prefab)
		{
			foreach(var c in _coords)
			{
				var newTile = GameObject.Instantiate(prefab, new Vector3(c.x * TileSize, 0, c.y * TileSize) + prefab.transform.localPosition, prefab.transform.rotation);
				newTile.transform.parent = holder;
				NetworkServer.Spawn(newTile);
			}
		}

		// Instantiate stuff
		InstantiatePrefab(borderCoords, ObstaclePrefab);
		var toInstantiate = new [] 
		{ 
			(SpawnPositionNumber, SpawnPrefab), 
			(ObstacleChunkNumber, ObstaclePrefab),
			(RiverChunkNumber, RiverPrefab),
			(HideoutNumber, HideoutPrefab),
			(coords.Count, TilePrefab)
		};
		IEnumerable<Vector2Int> coordIterator = coords;
		foreach (var (count, prefab) in toInstantiate)
		{
			InstantiatePrefab(coordIterator.Take(count), prefab);
			coordIterator = coordIterator.Skip(count);
		}
	}

	private static void FisherYatesShuffle<T>(IList<T> source)
	{
		var n = source.Count();
		while (n-- > 1)
		{
			var next = _rng.Next(n);
			var t = source[n];
			source[n] = source[next];
			source[next] = t;
		}
	}
}
