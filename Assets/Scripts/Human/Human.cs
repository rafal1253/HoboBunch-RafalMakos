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
    [SerializeField] bool _busy = false;

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
    void Start()
    {
        InitHuman();
    }

    private void InitHuman()
    {
        _state = States.Idle;
        ChangeAnimation(_state);
        _busy = false;

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        SaveAsGameobjectsList(FindObjectsOfType<ExtractionBuilding>(), out _extractBuildings);
        SaveAsGameobjectsList(FindObjectsOfType<ProductionBuilding>(), out _productBuildings);
        SaveAsGameobjectsList(FindObjectsOfType<WarehouseBuilding>(), out _warehouseBuildings);
    }

    void SaveAsGameobjectsList<T>(T[] array, out List<GameObject> buildingList)
    {
        buildingList = new List<GameObject>();
        foreach (T item in array)
        {
            buildingList.Add(item as GameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!_busy)
            SetTargetExtraction();


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
                            _state = States.Carry;
                            ChangeAnimation(_state);
                            if (_resourceSO.resourceName == "Wood")
                                SetTargetProduction();
                            else
                                SetTargetWarehouse();
                        }
                        else
                            _busy = false;
                    }
                    else
                    {
                        targetResources.Add(_resourceSO, 1);
                        _resourceSO = null;
                        _state = States.Idle;
                    }
                }
            }
        }

        


        /*
        State state = _state;
        if (_target == null)
            _state = State.Idle;
        else
        {
            if (_resourceSO == null)
            {
                _state = State.Walk;
                //SetTargetExtraction();
            }
            else
            {
                _state = State.Carry;
                if (_resourceSO.name == "Wood")
                    SetTargetProduction();
                else if (_resourceSO.name == "Chairs")
                    SetTargetWarehouse();
            }

        }
        ChangeAnimation(state);
        */

    }

    void SetTargetExtraction()
    {
        _target = FindNearestBuildingOfType(_extractBuildings);
        _busy = _target != null;
        _state = States.Walk;
        ChangeAnimation(_state);
        MoveToTarget();
    }
    void SetTargetProduction()
    {
        _target = FindNearestBuildingOfType(_productBuildings);
        MoveToTarget();
    }
    void SetTargetWarehouse()
    {
        _target = FindNearestBuildingOfType(_warehouseBuildings);
        MoveToTarget();
    }


    private GameObject FindNearestBuildingOfType(List<GameObject> buildingList)
    {
        GameObject nearestBuilding = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject building in buildingList)
        {
            float distance = Vector3.Distance(transform.position, building.transform.position);

            if (distance < shortestDistance /*&& building.GetComponent<GameResourcesList>().resources[0].amount > 0*/)
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


    void ChangeAnimation(States state)
    {
        switch(state)
        {
            case States.Idle:
                _animator.SetTrigger("Idle");
                break;
            case States.Walk:
                _animator.SetTrigger("Walk");
                break;
            case States.Carry:
                _animator.SetTrigger("Carry");
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
