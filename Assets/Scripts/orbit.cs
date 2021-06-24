using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbit : MonoBehaviour
{
    protected Transform cameraTransform;
    protected Transform parentTransform;

    Vector3 localRotation;
    float cameraDistance = 10f;

    public float mouseSensitivity = 4f;
    public float scrollSensitivity = 2f;
    public float orbitSpeed = 10f;
    public float scrollSpeed = 6f;

    public bool cameraDisable = false;
    // Start is called before the first frame update
    void Start()
    {
        this.cameraTransform = this.transform;
        parentTransform = this.transform.parent;
        localRotation.x = 90f;
        localRotation.y = 30f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            cameraDisable = !cameraDisable;
        }

        if (!cameraDisable)
        {
            if(Input.GetAxis("Mouse X") !=0 || Input.GetAxis("Mouse Y") != 0)
            {
                localRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity;
                localRotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;

                localRotation.y = Mathf.Clamp(localRotation.y, 0f, 90f);
            }

            if(Input.GetAxis("Mouse ScrollWheel") != 0f){
                float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
                scroll *= (cameraDistance * 0.3f);

                cameraDistance += scroll * -1f;

                cameraDistance = Mathf.Clamp(cameraDistance, 1.5f, 100f);
            }
        }

        Quaternion q = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        parentTransform.rotation = Quaternion.Lerp(parentTransform.rotation, q, Time.deltaTime * orbitSpeed);

        if(cameraTransform.localPosition.z != cameraDistance * -1f)
        {
            cameraTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(cameraTransform.localPosition.z, cameraDistance * -1f, Time.deltaTime * scrollSensitivity));
        }
    }
}
