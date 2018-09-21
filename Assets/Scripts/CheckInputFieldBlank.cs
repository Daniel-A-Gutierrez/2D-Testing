using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CheckInputFieldBlank : MonoBehaviour
{
    public Button target;
    // Start is called before the first frame update
    void Start()
    {
        target.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<InputField>().text.Trim().Length ==0 )
        {
            target.interactable = false;
        }
        else
        {
            target.interactable = true;
        }
    }
}
