using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMoving : MonoBehaviour {

    // Scroll main texture based on time

    public float scrollSpeed = 0.5f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        float offset = rend.material.GetTextureOffset("_MainTex").x + (Time.deltaTime * scrollSpeed);
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
