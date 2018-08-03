using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float force = 100;
    private new Rigidbody rigidbody;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(100 * transform.forward, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }


}
