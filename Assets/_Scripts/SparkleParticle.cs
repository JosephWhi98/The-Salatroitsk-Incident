using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkleParticle : MonoBehaviour
{
    public void Update()
    {
        float distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position) / 2f; 

        distance = Mathf.Clamp(distance, 0, 20f);

        transform.localScale = Vector3.one * distance; 
    }
}
