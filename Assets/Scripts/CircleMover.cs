using UnityEngine;

public class CircleMover : MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float speed = 1f; // Revolutions per second

    private Vector3 center;
    private float angle;

    private void Start()
    {
        center = transform.position;
    }

    private void Update()
    {
        angle += speed * Mathf.PI * 2f * Time.deltaTime;

        transform.position = center + new Vector3(
            Mathf.Cos(angle),
            Mathf.Sin(angle),
            0f
        ) * radius;
    }
}