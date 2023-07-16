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

    [SerializeField] Transform _target;
    [SerializeField] GameResourceSO _resourceSO;

    NavMeshAgent _agent;
    Animator _animator;

    // Lists of buildings that can be visited
    [SerializeField] List<ExtractionBuilding> _extractBuildings;
    [SerializeField] List<ProductionBuilding> _productBuildings;
    [SerializeField] List<WarehouseBuilding> _warehouseBuildings;

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

        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        _extractBuildings = FindObjectsOfType<ExtractionBuilding>().ToList();
        _productBuildings = FindObjectsOfType<ProductionBuilding>().ToList();
        _warehouseBuildings = FindObjectsOfType<WarehouseBuilding>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        State state = _state;
        if (_target == null)
            _state = State.Idle;
        else
        {
            if (_resourceSO == null)
            {
                _state = State.Walk;
                SetTargetExtraction();
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

        MoveToTarget();

    }

    void SetTargetExtraction()
    {
        _target = FindNearestBuildingOfType(typeof(ExtractionBuilding));
    }
    void SetTargetProduction()
    {
        _target = FindNearestBuildingOfType(typeof(ProductionBuilding));
    }
    void SetTargetWarehouse()
    {
        _target = FindNearestBuildingOfType(typeof(WarehouseBuilding));
    }


    private Transform FindNearestBuildingOfType(Type buildingType)
    {
        Transform nearestBuilding = null;
        float shortestDistance = Mathf.Infinity;
        dynamic[] targetBuildings = FindObjectsOfType(buildingType);

        foreach (dynamic building in targetBuildings)
        {
            Transform buildingTransform = building as Transform;
            float distance = Vector3.Distance(transform.position, building.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestBuilding = buildingTransform;
            }
        }

        return nearestBuilding;
    }

    void MoveToTarget()
    {
        if (_target != null)
            _agent.SetDestination(_target.position);
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
    void UpdateExtractionBuildings(ExtractionBuilding building)
    {
        _extractBuildings.Add(building);
    }
    void UpdateProductionBuildings(ProductionBuilding building)
    {
        _productBuildings.Add(building);
    }
    void UpdateWarehouseBuildings(WarehouseBuilding building)
    {
        _warehouseBuildings.Add(building);
    }
}
