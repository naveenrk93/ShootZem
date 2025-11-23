using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float xParallaxValue;
    [SerializeField] private float yParallaxValue;
    private float spriteLength;
    private Camera cam;
    private Vector3 deltaMovement;
    private Vector3 lastCameraPosition;

    void Start()
    {
        cam = Camera.main;
        lastCameraPosition = cam.transform.position;
        spriteLength = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        deltaMovement = cam.transform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * xParallaxValue, deltaMovement.y * yParallaxValue);
        lastCameraPosition = cam.transform.position;

        if(cam.transform.position.x - transform.position.x >= spriteLength)
        {
            transform.position = new Vector3(cam.transform.position.x + spriteLength, transform.position.y);
        }
        else if(transform.position.x - cam.transform.position.x >= spriteLength)
        {
            transform.position = new Vector3(cam.transform.position.x - spriteLength, transform.position.y);
        }
    }
}
