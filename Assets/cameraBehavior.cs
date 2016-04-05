using UnityEngine;
using System.Collections;

public class cameraBehavior : MonoBehaviour {

    public Transform camTransform;
    public Camera camera1;
    private GameObject currentTile;
    public float maxCameraDistance;
    public float minCameraDistance;
    public float cameraDistance;
    public float scrollSpeed;

    private bool moveFlag = false;
    private Vector3 moveDirection = new Vector3();
    private float maxPositionX;
    private float maxPositionY;

    public CreateGameBoard gb;
    private Vector3 home;

    private int tileSizeX, tileSizeY;

    // Use this for initialization
    void Start () {
        gb = (CreateGameBoard)FindObjectOfType(typeof(CreateGameBoard));
        home = gb.home;
        tileSizeX = gb.tileSizeX;
        tileSizeY = gb.tileSizeY;        
        camera1.transform.position = new Vector3(tileSizeX / 2, tileSizeY / 2, -1);
        camera1.orthographicSize = cameraDistance;
    }

    // Update is called once per frame
    void adjustCamera()
    {
        float height = (camera1.orthographicSize * 2.0f);
        float width = height * Screen.width / Screen.height;

        float maxX = tileSizeX - width / 2 + 7;
        float minX = width / 2;

        float maxY = tileSizeY - height / 2;
        float minY = height / 2;


        if (camera1.transform.position.x > maxX)
            camera1.transform.position = new Vector3(maxX, camera1.transform.position.y, camera1.transform.position.z);

        if (camera1.transform.position.y > maxY)
            camera1.transform.position = new Vector3(camera1.transform.position.x, maxY, camera1.transform.position.z);

        if (camera1.transform.position.x < minX)
            camera1.transform.position = new Vector3(minX, camera1.transform.position.y, camera1.transform.position.z);

        if (camera1.transform.position.y < minY)
            camera1.transform.position = new Vector3(camera1.transform.position.x, minY, camera1.transform.position.z);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            moveFlag = true;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            moveFlag = false;
        }

        if (Input.GetKey(KeyCode.Home))
        {
            camera1.transform.position = home;
            adjustCamera();
        }

        if (Input.GetKey(KeyCode.PageUp))
        {
            cameraDistance += 1;
            cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
            camera1.orthographicSize = cameraDistance;
            adjustCamera();
        }

        if (Input.GetKey(KeyCode.PageDown))
        {
            cameraDistance -= 1;
            cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
            camera1.orthographicSize = cameraDistance;
            adjustCamera();
        }

        if (Input.GetKey("up"))
        {
            moveDirection = new Vector3(0, 1, 0) * scrollSpeed * cameraDistance / 20;
            camera1.transform.position += moveDirection;
            adjustCamera();
        }
        if (Input.GetKey("down"))
        {
            moveDirection = new Vector3(0, -1, 0) * scrollSpeed * cameraDistance / 20;
            camera1.transform.position += moveDirection;
            adjustCamera();
        }
        if (Input.GetKey("left"))
        {
            moveDirection = new Vector3(-1, 0, 0) * scrollSpeed * cameraDistance / 20;
            camera1.transform.position += moveDirection;
            adjustCamera();
        }
        if (Input.GetKey("right"))
        {
            moveDirection = new Vector3(1, 0, 0) * scrollSpeed * cameraDistance / 20;
            camera1.transform.position += moveDirection;
            adjustCamera();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            cameraDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
            camera1.orthographicSize = cameraDistance;
            adjustCamera();
        }

        moveDirection = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * cameraDistance / 20;
        if (moveFlag)
        {
            camera1.transform.position -= moveDirection;
            adjustCamera();
        }
    }
}
