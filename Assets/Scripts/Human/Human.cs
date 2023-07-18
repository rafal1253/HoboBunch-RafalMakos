using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Human : MonoBehaviour
{
    enum States { Idle, Walk, Carry}
    States _state;


    [SerializeField] GameObject _target;
    [SerializeField] GameResourceSO _resourceSO;
    bool _targetingInProcess = false;

    NavMeshAgent _navMeshAgent;
    Animator _animator;

    // Lists of buildings that can be visited
    List<GameObject> _extractBuildings;
    List<GameObject> _productBuildings;
    List<GameObject> _warehouseBuildings;

    private void OnEnable()
    {
        EventManager.OnNewExtractionBuilding += UpdateExtractionBuildings;
        EventManager.OnNewProductionBuilding += UpdateProductionBuildings;
        EventManager.OnNewWarehouseBuilding += UpdateWarehouseBuildings;
    }
    private void OnDisable()
    {
        EventManager.OnNewExtractionBuilding -= UpdateExtractionBuildings;
        EventManager.OnNewProductionBuilding -= UpdateProductionBuildings;
        EventManager.OnNewWarehouseBuilding -= UpdateWarehouseBuildings;
    }

    // Start is called before the first frame update
    void Awake()
    {
        InitHuman();
    }

    private void InitHuman()
    {
        _state = States.Idle;

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        SaveAsGameobjectsList(FindObjectsOfType<ExtractionBuilding>(), out _extractBuildings);
        SaveAsGameobjectsList(FindObjectsOfType<ProductionBuilding>(), out _productBuildings);
        SaveAsGameobjectsList(FindObjectsOfType<WarehouseBuilding>(), out _warehouseBuildings);
    }

    void SaveAsGameobjectsList<T>(T[] array, out List<GameObject> buildingList) where T : Component
    {
        buildingList = new List<GameObject>();
        foreach (T item in array)
        {
            buildingList.Add(item.gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        // set target
        if (_target == null && !_targetingInProcess)
        {
            _state = States.Idle;
            _targetingInProcess = true;
            if (_resourceSO == null)
            {
                _state = States.Walk;

                if (_warehouseBuildings.Count > 0 && _productBuildings.Count > 0)
                    SetTargetToNearest(_productBuildings);
                else if (_productBuildings.Count > 0 && _extractBuildings.Count > 0)
                    SetTargetToNearest(_extractBuildings);
            }
            else
            {
                _state = States.Carry;

                if (_resourceSO.resourceName == "Wood")
                    SetTargetToNearest(_productBuildings, false);
                else
                    SetTargetToNearest(_warehouseBuildings, false);
            }
            _targetingInProcess = false;
            ChangeAnimation();
        }
        
        // take resource when reach destination and eq is empty
        if (_target != null && !_navMeshAgent.pathPending)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    GameResourcesList targetResources = _target.GetComponent<GameResourcesList>();
                    if (_resourceSO == null)
                    {
                        GameResourceSO resourceNeeded = targetResources.resourceSOs.Last();
                        if (targetResources.TryUse(resourceNeeded, 1))
                        {
                            _resourceSO = resourceNeeded;
                        }
                    }
                    else
                    {
                        targetResources.Add(_resourceSO, 1);
                        _resourceSO = null;
                    }
                    _target = null;
                }
            }
        }
    }

    void SetTargetToNearest(List<GameObject> buildingList, bool takingItem = true)
    {
        _target = FindNearestBuildingOfType(buildingList, takingItem);
        MoveToTarget();
    }


    private GameObject FindNearestBuildingOfType(List<GameObject> buildingList, bool takingItem = true)
    {
        GameObject nearestBuilding = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject building in buildingList)
        {
            float distance = Vector3.Distance(transform.position, building.transform.position);

            if (takingItem && (building.GetComponent<GameResourcesList>().resources.Last().amount <= 0))
                continue;

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestBuilding = building.gameObject;
            }
        }
        return nearestBuilding;
    }
    


    void MoveToTarget()
    {
        if (_target != null)
            _navMeshAgent.SetDestination(_target.transform.position);
        //transform.LookAt(_target.position);
    }


    void ChangeAnimation()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Carry", false);
        switch (_state)
        {
            case States.Idle:
                break;
            case States.Walk:
                _animator.SetBool("Walk", true);
                _navMeshAgent.speed = 0.5f;
                break;
            case States.Carry:
                _animator.SetBool("Carry", true);
                _navMeshAgent.speed = 0.3f;
                break;
        }
    }
    void UpdateExtractionBuildings(GameObject building)
    {
        _extractBuildings.Add(building);
    }
    void UpdateProductionBuildings(GameObject building)
    {
        _productBuildings.Add(building);
    }
    void UpdateWarehouseBuildings(GameObject building)
    {
        _warehouseBuildings.Add(building);
    }
}
