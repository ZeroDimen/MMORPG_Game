using System;
using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    public string playerName;
    public int level;
    public int gold;
    public float speed = 5f;

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(h, 0, v).normalized;
        transform.Translate(moveDir * speed * Time.deltaTime);
    }
}
