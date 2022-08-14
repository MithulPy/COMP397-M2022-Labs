using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerBehaviour : NetworkBehaviour
{
    [Header("Player Movement Properties")]
    public float speed;

    [Header("Player Colour Properties")]
    public MeshRenderer meshRenderer;

    // network variables for input
    private NetworkVariable<float> verticalPosition = new NetworkVariable<float>();
    private NetworkVariable<float> horizontalPosition = new NetworkVariable<float>();

    // network variable for color
    private NetworkVariable<Color> materialColor = new NetworkVariable<Color>();

    // local variables that are used to detect changes in input
    private float localHorizontal;
    private float localVertical;

    // local variable used to detect changes in colour
    private Color localColor;
    private Color randomColor;

    void Awake()
    {
        // adds an event listener("OnValueChanged") to the materialColor and then 
        // when it triggers it invokes the ColorOnChange event Handler function
        materialColor.OnValueChanged += ColorOnChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // spawn each player in a random position
        RandomSpawnPosition();
        randomColor = SetRandomMaterialColour();

        meshRenderer.material.SetColor("_Color", materialColor.Value);
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
    }

    public void RandomSpawnPosition()
    {
        var x = Random.Range(-3.0f, 3.0f);
        var z = Random.Range(-3.0f, 3.0f);
        transform.position = new Vector3(x, 1.0f, z);
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
        transform.position = new Vector3(transform.position.x + horizontalPosition.Value, transform.position.y,
            transform.position.z + verticalPosition.Value);

        meshRenderer.material.SetColor("_Color", materialColor.Value);
    }

    public void ClientUpdate()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
        var verticalInput = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;

        // network update
        if (localHorizontal != horizontalInput || localVertical != verticalInput)
        {
            localHorizontal = horizontalInput;
            localVertical = verticalInput;

            // update the Client position on the network
            UpdateClientPositionServerRpc(horizontalInput, verticalInput);
        }

        if (localColor != randomColor)
        {
            localColor = randomColor;

            SetClientColorServerRpc(randomColor);
        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float horizontal, float vertical)
    {
        // set the network variables for horizontal and vertical input
        horizontalPosition.Value = horizontal;
        verticalPosition.Value = vertical;
    }

    [ServerRpc]
    public void SetClientColorServerRpc(Color color)
    {
        materialColor.Value = color;
    }

    void ColorOnChange(Color oldColor, Color newColor)
    {
        GetComponent<MeshRenderer>().material.color = materialColor.Value;
    }
}