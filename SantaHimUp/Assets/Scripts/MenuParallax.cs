using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [SerializeField] private float offsetMultiplier = 1f;
    [SerializeField] private float smoothTime = .3f;

    private Vector2 startPosition;
    private Vector3 velocity;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 offset = new Vector2(mousePos.x / Screen.width - 0.5f, mousePos.y / Screen.height - 0.5f);
        transform.position = Vector3.SmoothDamp(transform.position, startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);
    }
}
