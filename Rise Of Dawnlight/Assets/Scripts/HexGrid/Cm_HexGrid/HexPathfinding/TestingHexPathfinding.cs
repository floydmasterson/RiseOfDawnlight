using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CM_Pathfinding;

public class TestingHexPathfinding : MonoBehaviour {


    [SerializeField] private Transform pfSquare;
    [SerializeField] private Transform pfHex;
    [SerializeField] private Transform pfUnwalkable;
    //[SerializeField] private MovePositionHexPathfinding movePositionHexPathfinding;


    private GridHexXZ<GridObject> gridHexXZ;
    private PathfindingHexXZ pathfindingHexXZ;


    private class GridObject {
        public Transform visualTransform;

        public void Show() {
            visualTransform.Find("Selected").gameObject.SetActive(true);
        }

        public void Hide() {
            visualTransform.Find("Selected").gameObject.SetActive(false);
        }

    }


    private void Awake() {
        int width = 30;
        int height = 18;
        float cellSize = 1f;
        gridHexXZ =
            new GridHexXZ<GridObject>(width, height, cellSize, Vector3.zero, (GridHexXZ<GridObject> g, int x, int y) => new GridObject());

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                Transform visualTransform = Instantiate(pfHex, gridHexXZ.GetWorldPosition(x, z), Quaternion.identity);
                gridHexXZ.GetGridObject(x, z).visualTransform = visualTransform;
                gridHexXZ.GetGridObject(x, z).Hide();
            }
        }

        pathfindingHexXZ = new PathfindingHexXZ(width, height, cellSize);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            List<Vector3> pathList = pathfindingHexXZ.FindPath(Vector3.zero, Mouse3D.GetMouseWorldPosition());
            for (int i = 0; i < pathList.Count - 1; i++) {
                Debug.DrawLine(pathList[i], pathList[i+1], Color.green, 3f);
            }
            //Debug.Log(movePositionHexPathfinding);
            //movePositionHexPathfinding.SetMovePosition(Mouse3D.GetMouseWorldPosition(), () => { });
        }
        if (Input.GetMouseButtonDown(1)) {
            pathfindingHexXZ.GetGrid().GetGridObject(Mouse3D.GetMouseWorldPosition()).SetIsWalkable(false);
            pathfindingHexXZ.GetGrid().GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);
            Instantiate(pfUnwalkable, pathfindingHexXZ.GetGrid().GetWorldPosition(x, z), Quaternion.identity);
        }
    }

}