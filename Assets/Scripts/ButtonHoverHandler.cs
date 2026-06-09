using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    private InfoPanelController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<InfoPanelController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData) => controller.SetMessage(description);
    public void OnPointerExit(PointerEventData eventData) => controller.ResetMessage();
}
