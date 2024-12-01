using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3 startSize;
    public int scale = 1;
    public float size = 1;
    public BoxCollider collider;
    public bool isSelected;


    void Update()
    {
        if (isSelected && Input.GetKeyDown(KeyCode.Equals))
        {
            scale += 1;
            if (scale > 5) scale = 5;
            SetSize(size);
        }
        else if (isSelected && Input.GetKeyUp(KeyCode.Minus))
        {
            scale -= 1;
            if (scale < 1) scale = 1;
            SetSize(size);
        }
    }
    public void SetSize(float _size)
    {
        size = _size;
        this.transform.localScale = startSize * scale * (size + 1);
    }
}
