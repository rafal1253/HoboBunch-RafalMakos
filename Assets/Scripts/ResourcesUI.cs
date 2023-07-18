using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    public Canvas buildingCanvas;
    [SerializeField] bool _showOnClick = false;
    bool _clicked = true;


    private void Start()
    {
        _clicked = !_showOnClick;
    }
    void Update()
    {
        if (_showOnClick)
        {
            _clicked = Input.GetMouseButton(0);
            if (_clicked)
                ShowUI();
        }
        else
            ShowUI();
    }

    private void ShowUI()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.gameObject == gameObject)
            {
                buildingCanvas.gameObject.SetActive(true);
            }
            else
            {
                buildingCanvas.gameObject.SetActive(false);
            }
        }
        else
        {
            buildingCanvas.gameObject.SetActive(false);
        }
    }
}
