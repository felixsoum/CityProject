using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject aimReticle;
    [SerializeField] private Transform gun;
    [SerializeField] private Transform rayPoint;

    public float sheathedMoveForce = 3;
    public float unsheathedMoveForce = 2;
    public float meshLerpSpeed = 0.333f;
    public float aimLerpSpeed = 0.667f;
    public float groundDistance = 0.1f;
    public float extraGravity = 100;
    public GameObject mesh;

    private const float ShootRayDistance = 100;
    private Animator animator;
    private bool isEquipSheathed = true;
    private bool isMovementReady = true;
    private bool isAiming;
    private new Rigidbody rigidbody;
    private PlayerCamera playerCamera;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = mesh.GetComponent<Animator>();
        playerCamera = Camera.main.GetComponent<PlayerCamera>();
    }

    void Start()
    {
    }

    private void OnMovementReady()
    {
        isMovementReady = true;
    }

    void Update()
    {
        isAiming = Input.GetMouseButton(1);

        aimReticle.SetActive(isAiming);

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        if (isMovementReady)
        {
            ApplyMovementInput();
        }

        if (isAiming)
        {
            ApplyAim();
        }
        else
        {
            gun.localRotation = Quaternion.identity;
        }

        ApplyFakeGravity();

    }

    void ApplyMovementInput()
    {
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;

        Vector3 move = cameraRight.normalized * Input.GetAxis("Horizontal") + cameraForward.normalized * Input.GetAxis("Vertical");
        float moveForce = isEquipSheathed ? sheathedMoveForce : unsheathedMoveForce;
        rigidbody.AddForce(move * moveForce, ForceMode.VelocityChange);


        if (!isAiming && move.magnitude > 0.1)
        {
            mesh.transform.forward = Vector3.Lerp(mesh.transform.forward, move.normalized, meshLerpSpeed);
        }
    }

    private void ApplyAim()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 groundForward = cameraForward;
        groundForward.y = 0;
        mesh.transform.forward = Vector3.Lerp(mesh.transform.forward, groundForward.normalized, aimLerpSpeed);

        bool hasFoundTarget = false;
        RaycastHit[] hits = Physics.RaycastAll(playerCamera.transform.position, cameraForward, ShootRayDistance);
        Vector3 target = new Vector3(-9999f, -9999f, -9999f);

        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Player")
            {
                continue;
            }

            if (Vector3.Distance(playerCamera.transform.position, hit.point) < Vector3.Distance(playerCamera.transform.position, target))
            {
                hasFoundTarget = true;
                target = hit.point;
            }
            break;
        }

        if (!hasFoundTarget)
        {
            target = playerCamera.transform.position + cameraForward * ShootRayDistance;
        }
        rayPoint.transform.position = target;
        gun.transform.forward = (target - gun.transform.position).normalized;
    }

    void ApplyFakeGravity()
    {
        if (!IsGrounded())
        {
            rigidbody.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
    }

    bool IsGrounded()
    {
        Vector3 groundOrigin = transform.position + Vector3.up * groundDistance;
        Vector3 ray = Vector3.down *  groundDistance *2;
        var hits = Physics.RaycastAll(groundOrigin, Vector3.down, groundDistance * 2);
        Debug.DrawRay(groundOrigin, ray, Color.red);
        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Player")
            {
                continue;
            }
            return true;
        }
        return false;
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }
}
