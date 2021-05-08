using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerBox : MonoBehaviour
{
    public List<GameObject> wallList;

    public void ColorSuccess()
    {
        foreach (GameObject obj in wallList)        
            obj.GetComponent<SpriteRenderer>().color = TrainingController.Instance.successColor;         
    }
    public void ColorFail()
    {
        foreach (GameObject obj in wallList)
            obj.GetComponent<SpriteRenderer>().color = TrainingController.Instance.failColor ;
    }
}
