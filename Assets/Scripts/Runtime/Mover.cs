using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    public void Move(Vector3 direction) =>
        transform.position += direction.normalized * speed * Time.deltaTime;
}
