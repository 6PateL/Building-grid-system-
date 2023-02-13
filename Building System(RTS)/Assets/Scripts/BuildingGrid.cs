using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int _gridSize = new Vector2Int(10,10);
    private Building[,] _grid;
    private Building _flyingBuilding;

    [SerializeField] private Building[] _buildingPrefabs; 
    private int currentIndex = 0;
    
    [SerializeField] private Text currentTypeInfo; 

    private Camera mainCamera; 
    
    //create Grid 
    private void Awake()
    {
        _grid = new Building[_gridSize.x,_gridSize.y];
        mainCamera = Camera.main;
        currentTypeInfo.text = "" + _buildingPrefabs[currentIndex];
    }
    
    public void StartPlacingBuilding()
    {
        if(_flyingBuilding != null){Destroy(_flyingBuilding);}
        _flyingBuilding = Instantiate(_buildingPrefabs[currentIndex]);   
    }
    
    private void Update()
    {
        if (_flyingBuilding != null)
        {
            var groundPlane = new Plane(Vector3.up,Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worlPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worlPosition.x);
                int y = Mathf.RoundToInt(worlPosition.z);

                bool available = true;
                if (x < 0 || x > _gridSize.x - _flyingBuilding.Size.x) available = false; 
                if (y < 0 || y > _gridSize.y - _flyingBuilding.Size.y) available = false;
                if (available && IsPlaceTaken(x, y)) available = false; 

                _flyingBuilding.transform.position = new Vector3(x, 0f,y);
                _flyingBuilding.SetTransparent(available);

                if (available && Input.GetMouseButtonDown(0))
                {
                    PlaceFlyingBuilding(x,y);
                }
                
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    _flyingBuilding.transform.Rotate(0f,90f,0f);
                }
            }
        }
        ChangeBuildingType(); 
    }

    //change type of building
    private void ChangeBuildingType()
    {
        if (currentIndex >= 3){currentIndex = 0;}
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentIndex++;
            currentTypeInfo.text = "" + _buildingPrefabs[currentIndex];
        }
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < _flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < _flyingBuilding.Size.y; y++)
            {
                if(_grid[placeX + x, placeY + y] != null) return true; 
            }
        }
        return false; 
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        for (int x = 0; x < _flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < _flyingBuilding.Size.y; y++)
            {
                _grid[placeX + x, placeY] = _flyingBuilding; 
            }
        }
        _flyingBuilding.SetNormal();
        _flyingBuilding = null; 
    }

    //check plane,optional 
    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(transform.position + new Vector3(x,0f,y), new Vector3(1,.1f,1));
            }
        }
    }
}

