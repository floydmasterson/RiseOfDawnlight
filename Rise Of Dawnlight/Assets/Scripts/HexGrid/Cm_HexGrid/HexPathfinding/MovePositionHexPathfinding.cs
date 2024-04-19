using CM_Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePositionHexPathfinding : MonoBehaviour
{
    [SerializeField] private float reachedPositionDistance = 1f;

    private List<Vector3> pathVectorList;
    private int pathIndex = -1;

    public void SetMovePostion(Vector3 movePostion)
    {
      
        pathVectorList = CM_Pathfinding.PathfindingHexXZ.Instance.FindPath(transform.position, movePostion);
        if(pathVectorList.Count > 0)
        {
            pathVectorList.RemoveAt(0);
        }
        if (pathVectorList.Count > 0)
            pathIndex = 0;
        else
        {
            pathIndex = -1;
        }

    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown(0))
        {
			SetMovePostion(Mouse3D.GetMouseWorldPosition());
			List<Vector3> pathList = CM_Pathfinding.PathfindingHexXZ.Instance.FindPath(transform.position, Mouse3D.GetMouseWorldPosition());
			for (int i = 0; i < pathList.Count - 1; i++)
			{
				Debug.DrawLine(pathList[i], pathList[i + 1], Color.green, 3f);
			}

		}
		if (pathIndex != -1)
        {
            Vector3 nextPathPostion = pathVectorList[pathIndex];
            Vector3 moveVelocity = (nextPathPostion - transform.position).normalized;
            GetComponent<IMoveVelocity>().SetVelocity(moveVelocity);

            if(Vector3.Distance(transform.position, nextPathPostion) < reachedPositionDistance)
            {
                pathIndex++;
              
                if(pathIndex >= pathVectorList.Count)
                {
                    pathIndex = -1;
                }
            }
        }
        else
        {
			GetComponent<IMoveVelocity>().SetVelocity(Vector3.zero);
		}
        
    }

	private void OnTriggerEnter(Collider other)
	{
        HexTileHenerationSettings.TileType tileType = other.GetComponentInParent<HexTile>().tileType;
        if(tileType == HexTileHenerationSettings.TileType.Enemy)
        {
            pathIndex = -1;
        }
	}
}
