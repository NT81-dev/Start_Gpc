using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoPanelController : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    private string defaultMessage = "";
    // Start is called before the first frame update
    void Start()
    {
        infoText.text = defaultMessage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMessage(string message)
    {
        infoText.text = message;
    }

    public void ResetMessage()
    {
        infoText.text = defaultMessage;
    }
}
