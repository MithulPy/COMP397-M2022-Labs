using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerBehaviour : NetworkBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 10.0f;
    public float gravity = -30.0f;
    public float jumpHeight = 3.0f;
    public Vector3 velocity;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundRadius = 0.5f;
    public LayerMask groundMask;
    public bool isGrounded;

    // network variables for input
    private NetworkVariable<float> remoteVerticalInput = new NetworkVariable<float>();
    private NetworkVariable<float> remoteHorizontalInput = new NetworkVariable<float>();
    private NetworkVariable<bool> remoteJumpInput = new NetworkVariable<bool>();

    // local variables that are used to detect changes in input
    private float localHorizontalInput;
    private float localVerticalInput;
    private bool localJumpInput;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer)
        {
            GetComponentInChildren<NetworkCameraController>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
        }

        if (IsServer)
        {
            RandomSpawnPosition();
        }
    }

    private void LateUpdate()
    {
        if (IsLocalPlayer)
        {
            UpdateRotationServerRpc(transform.eulerAngles.y);
        }
    }

    [ServerRpc]
    void UpdateRotationServerRpc(float newRotationY)
    {
        transform.rotation = Quaternion.Euler(0.0f, newRotationY, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            // server update
            ServerUpdate();
        }

        if (IsClient && IsOwner)
        {
            // client update
            ClientUpdate();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void RandomSpawnPosition()
    {
        GetComponent<CharacterController>().enabled = false;
        var x = Random.Range(-3.0f, 3.0f);
        var z = Random.Range(-3.0f, 3.0f);
        transform.position = new Vector3(x, 1.0f, z);
        GetComponent<CharacterController>().enabled = true;
    }

    public Color SetRandomMaterialColour()
    {
        var r = Random.Range(0.0f, 1.0f);
        var g = Random.Range(0.0f, 1.0f);
        var b = Random.Range(0.0f, 1.0f);
        return new Color(r, g, b);
    }

    private void ServerUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = -2.0f;
        }

        Vector3 move = transform.right * remoteHorizontalInput.Value + transform.forward * remoteVerticalInput.Value;
        GetComponent<CharacterController>().Move(move * maxSpeed * Time.deltaTime);

        if (remoteJumpInput.Value && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
    }

    public void ClientUpdate()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");
        bool isJumping = Input.GetButton("Jump");

        // network update
        if (localHorizontalInput != x || localVerticalInput != z || localJumpInput != isJumping)
        {
            localHorizontalInput = x;
            localVerticalInput = z;
            localJumpInput = isJumping;

            // update the Client position on the network
            UpdateClientPositionServerRpc(x, z, isJumping);
        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float horizontal, float vertical, bool isJumping)
    {
        // set the network variables for horizontal and vertical input
        remoteHorizontalInput.Value = horizontal;
        remoteVerticalInput.Value = vertical;
        remoteJumpInput.Value = isJumping;
    }

}