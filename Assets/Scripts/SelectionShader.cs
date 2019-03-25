using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionShader : MonoBehaviour
{

    public Material Shader;

    public MouseController mouseController;

    void Start()
    {
        mouseController = MouseController.FindObjectOfType<MouseController>();
        Shader.SetFloat("_SecondOutlineWidth", 0f);
    }


    void Update()
    {
        if (mouseController.SelectedUnit != null)
        {
            Shader.SetFloat("_SecondOutlineWidth", 0.04f);
        }
        else
        {
            Shader.SetFloat("_SecondOutlineWidth", 0f);
        }
    }
}
