using UnityEngine;
using System.Collections;

public class cameraBehavior : MonoBehaviour {

    public Transform camTransform;
    public Camera camera1;
    //private GameObject currentTile;
    public float maxCameraDistance;
    public float minCameraDistance;
    public float cameraDistance;
    public float scrollSpeed;

    private bool moveFlag = false;
    private Vector3 moveDirection = new Vector3();


    public gameBoard gb;
    private Vector3 home;

    private int tileSizeX, tileSizeY;

    // Use this for initialization
    void Start () {
        gb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        home = gb.home;
        tileSizeX = gameBoard.tileSizeX;
        tileSizeY = gameBoard.tileSizeY;        
        camera1.orthographicSize = cameraDistance;
        camera1.transform.position = home;
        adjustCamera();
    }

    // Update is called once per frame
    void adjustCamera()
    {
        float height = (camera1.orthographicSize * 1.8f);
        float width = height * Screen.width / Screen.height;

        float maxX = tileSizeX - width / 2 + 7;
        float minX = width / 2;

        float maxY = tileSizeY - height / 2;
        float minY = (height*.9f) / 2;


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
        
        if (Input.GetButtonDown("Fire2"))
        {
            moveFlag = true;
        }

        if (Input.GetButtonUp("Fire2"))
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

        if (Input.GetKey("up") || (Input.GetKey(KeyCode.W)))
        {
            moveDirection = new Vector3(0, 1, 0) * scrollSpeed * cameraDistance / 20;
            camera1.transform.position += moveDirection;
            adjustCamera();
        }
        if (Input.GetKey("down") || (Input.GetKey(KeyCode.S)))
        {
            moveDirection = new Vector3(0, -1, 0) * scrollSpeed * cameraDistance / 20;
            camera1.transform.position += moveDirection;
            adjustCamera();
        }
        if (Input.GetKey("left") || (Input.GetKey(KeyCode.A)))
        {
            moveDirection = new Vector3(-1, 0, 0) * scrollSpeed * cameraDistance / 20;
            camera1.transform.position += moveDirection;
            adjustCamera();
        }
        if (Input.GetKey("right") || (Input.GetKey(KeyCode.D)))
        {
            moveDirection = new Vector3(1, 0, 0) * scrollSpeed * cameraDistance / 20;
            camera1.transform.position += moveDirection;
            adjustCamera();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            cameraDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed *10;
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
