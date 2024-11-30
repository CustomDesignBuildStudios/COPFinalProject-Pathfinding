using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3 startSize;
    public BoxCollider collider;
    public void SetSize(float size)
    {
        this.transform.localScale = startSize * (size + 1);
    }
}
