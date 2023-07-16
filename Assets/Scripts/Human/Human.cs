using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Human : MonoBehaviour
{
    enum State { Idle, Walk, Carry}
    State _state;

    [SerializeField] GameObject _target;
    [SerializeField] GameResourceSO _resourceSO;
    [SerializeField] bool _busy = false;

    NavMeshAgent _agent;
    Animator _animator;

    // Lists of buildings that can be visited
    [SerializeField] List<GameObject> _extractBuildings = new List<GameObject>();
    [SerializeField] List<GameObject> _productBuildings;
    [SerializeField] List<GameObject> _warehouseBuildings;

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
        _state = State.Idle;
        _busy = false;

        _agent = GetComponent<NavMeshAgent>();
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
            FindJob();

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

    void FindJob()
    {
        _target = FindNearestExtraction(_extractBuildings);
        _busy = _target != null;
        MoveToTarget();
    }
    void SetTargetProduction()
    {
        //_target = FindNearestBuildingOfType(_productBuildings);
    }
    void SetTargetWarehouse()
    {
        //_target = FindNearestBuildingOfType(_warehouseBuildings);
    }


    private GameObject FindNearestExtraction(List<GameObject> buildingList)
    {
        GameObject nearestBuilding = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject building in buildingList)
        {
            float distance = Vector3.Distance(transform.position, building.transform.position);

            if (distance < shortestDistance && building.GetComponent<GameResourcesList>().resources[0].amount > 0)
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
            _agent.SetDestination(_target.transform.position);
        //transform.LookAt(_target.position);
    }


    void ChangeAnimation(State state)
    {
        if (_state != state)
        {
            switch(state)
            {
                case State.Idle:
                    _animator.SetTrigger("Idle");
                    break;
                case State.Walk:
                    _animator.SetTrigger("Walk");
                    break;
                case State.Carry:
                    _animator.SetTrigger("Carry");
                    break;
            }
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
