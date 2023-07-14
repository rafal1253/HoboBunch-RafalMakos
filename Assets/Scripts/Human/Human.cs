using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    enum State { Idle, Walk, Carry}
    State _state;

    Transform _target;

    NavMeshAgent _agent;


    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveByClick();
    }


    void MoveByClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
                _agent.SetDestination(hit.point); 
        }
    }
}
