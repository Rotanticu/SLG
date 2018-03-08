using UnityEngine;

public class HexMapCamera : MonoBehaviour
{

   // Transform swivel, stick;
    public new  Camera camera;
    float zoom = 0.4f;
    public float stickMinZoom, stickMaxZoom;
    public float moveSpeed;
    void Awake()
    {
        //swivel = transform.GetChild(0);
       // stick = swivel.GetChild(0);
        
    }
    void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }
        float xDelta = -Input.GetAxis("Vertical");
        float zDelta = Input.GetAxis("Horizontal");
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);
        float distance = Mathf.Lerp(stickMaxZoom, stickMinZoom, zoom);
        //stick.localPosition = new Vector3(0f, 0f, distance);
        camera.orthographicSize = distance;
    }
    void AdjustPosition (float xDelta, float zDelta)
    {
        float distance = moveSpeed * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += new Vector3(xDelta, 0f, zDelta) * distance;
        transform.localPosition = position;
    }
}
