using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        float width = GetScreenToWorldWidth();
        transform.localScale = Vector3.one * (width*0.8f); //set the scale of the board to 80% of the screen width
    }

    float GetScreenToWorldHeight() {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        var height = edgeVector.y * 2;
        return height;
    }

    float GetScreenToWorldWidth() {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        var width = edgeVector.x * 2;
        return width;
    }
}
