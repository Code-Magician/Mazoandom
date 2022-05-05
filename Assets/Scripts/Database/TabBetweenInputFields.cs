using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class TabBetweenInputFields : MonoBehaviour
{
    [SerializeField] InputField nextInputField;
    [SerializeField] Button enterButton;
    InputField thisInputField;

    private void Start()
    {
        if (nextInputField == null)
        {
            Destroy(this);
        }

        thisInputField = GetComponent<InputField>();
    }

    private void Update()
    {
        if (thisInputField.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                nextInputField.ActivateInputField();
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                thisInputField.DeactivateInputField();
            }
        }
    }
}
