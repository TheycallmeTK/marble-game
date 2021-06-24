using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class move : MonoBehaviour
{
    Vector3 lerpStart;
    Vector3 lerpEnd;
    public Text scoreText;
    public float score;
    public GameObject player;
    public Rigidbody rbody;
    public Camera cam;
    public GameObject pivot;
    public Vector3 offset;
    public float accel;
    public bool onFloor;
    GameObject currentCheckpoint;
    List<GameObject> checkpoints = new List<GameObject>();
    Vector3 forward;
    Vector3 prevForward;
    GameObject part;
    public GameObject boostBar;
    bool boostRecharge;
    RawImage boost;
    bool doLerp;
    float lerpFloat;
    // Start is called before the first frame update
    void Start()
    {
        doLerp = false;
        lerpFloat = 0.0f;
        score = 0;
        rbody = this.GetComponent<Rigidbody>();
        offset = transform.position;
        accel = 1;
        for (int i=0; i < GameObject.FindGameObjectsWithTag("checkpoint").Length; i++)
        {
            //checkpoints.Add(GameObject.FindGameObjectsWithTag("checkpoint")[i]);
        }
        part = transform.Find("Particle System").gameObject;
        boostRecharge = false;
        boost = boostBar.transform.Find("RawImage").GetComponent<RawImage>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        float mult = 7000;
        float torqueMult = 70000;
        
        //Debug.Log(rbody.velocity.magnitude);
        

        if (!onFloor) 
        { 
            mult = 14000;
            rbody.AddForce(new Vector3(0, -1500, 0));
        }
        
        if (Input.GetKey(KeyCode.W) && rbody.velocity.magnitude <= 25)
        {
            
            rbody.AddTorque(new Vector3(cam.transform.right.x * torqueMult * accel*100, 0f, cam.transform.right.z * torqueMult * accel*100));

            rbody.AddForce(forward.x * mult * accel/10, -10f, forward.z * mult * accel/10);
            if (onFloor) { accel *= 1.3f; } else { accel = 2.0f; }
                
        }
        if (Input.GetKey(KeyCode.S) && rbody.velocity.magnitude <= 25)
        {
            rbody.AddTorque(-cam.transform.right * torqueMult * accel*10);
            rbody.AddForce(new Vector3(-cam.transform.forward.x * mult * accel/10, -10f, -cam.transform.forward.z * mult * accel/10));
            if (onFloor) { accel += 1.0f; } else { accel = 2.0f; }
        }

        if (Input.GetKey(KeyCode.A) && rbody.velocity.magnitude <= 25)
        {
            rbody.AddTorque(cam.transform.forward * torqueMult * accel * 10);
            rbody.AddForce(new Vector3(-cam.transform.right.x * mult * accel / 10, -10f, -cam.transform.right.z * mult * accel / 10));
            if (onFloor) { accel += 1.05f; } else { accel = 2.0f; }
        }
        if (Input.GetKey(KeyCode.D) && rbody.velocity.magnitude <= 25)
        {
            rbody.AddTorque(-cam.transform.forward * torqueMult * accel * 10);
            rbody.AddForce(new Vector3(cam.transform.right.x * mult * accel / 10, -10f, cam.transform.right.z * mult * accel / 10));
            if (onFloor) { accel += 1.05f; } else { accel = 2.0f; }
        }

        if (!Input.GetKey(KeyCode.W)&&!Input.GetKey(KeyCode.S)&&!Input.GetKey(KeyCode.A)&&!Input.GetKey(KeyCode.D))
        {
            accel -= 0.1f;
            
        }
        accel = Mathf.Clamp(accel, 1, 50);
        if (boost.rectTransform.sizeDelta.x < 100f)
        {
            boost.rectTransform.sizeDelta = new Vector2(boost.rectTransform.sizeDelta.x + 0.7f, boost.rectTransform.sizeDelta.y);
            //Debug.Log(boostBar.transform.Find("RawImage").GetComponent<RawImage>().rectTransform.sizeDelta);
            if (boost.rectTransform.sizeDelta.x >= 100f) { boostRecharge = false; }
        }
        if(rbody.velocity.magnitude >= 25)
        {
            rbody.AddForce(-rbody.velocity/5);
            Debug.Log("drag");
        }

        pivot.transform.position += transform.position - offset;
        offset = transform.position;

        

        

        

        forward = cam.transform.forward;

        if(rbody.velocity.magnitude >= 20)
        {
            //part.SetActive(true);
            part.GetComponent<ParticleSystem>().Play();
        }
        else { //part.SetActive(false);
            part.GetComponent<ParticleSystem>().Stop();
        }

        if (doLerp)
        {
            float velX = Mathf.Abs(rbody.velocity.x);
            float velZ = Mathf.Abs(rbody.velocity.z);
            //transform.position = new Vector3(Mathf.Lerp(lerpStart.x, lerpEnd.x, lerpFloat+=(Time.deltaTime*0.5f)), transform.position.y, transform.position.z);
            rbody.AddForce(new Vector3(-1* velX*(Mathf.Abs(transform.position.x - lerpEnd.x)*mult/200), -1 * Mathf.Abs(transform.position.y - lerpEnd.y), -1 * velZ * (Mathf.Abs(transform.position.z - lerpEnd.z) * mult / 200)));
            
            accel = -1.0f;
            if(Mathf.Abs(transform.position.x - lerpEnd.x) < 3 &&Mathf.Abs(transform.position.z - lerpEnd.z) < 3)
            {
                doLerp = false;
                Debug.Log("lerp end");
            } else if(velX <5.0f && velZ < 5.0f)
            {
                doLerp = false;
                Debug.Log("lerp end");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onFloor)
        {
            rbody.AddForce(new Vector3(0f, 15000f, 0f), ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !boostRecharge)
        {
            rbody.AddForce(new Vector3(cam.transform.forward.x * 23000f, 0f, cam.transform.forward.z * 23000f), ForceMode.Impulse);
            boostBar.transform.Find("RawImage").GetComponent<RawImage>().rectTransform.sizeDelta = new Vector2(0, 100);
            boostRecharge = true;
        }
    }

    Vector3 calcForward()
    {
        float angle = Vector3.Angle(prevForward, cam.transform.forward);
        prevForward = forward;
        if (angle == 45) { return cam.transform.forward; }
        else { return forward; }
    }

    private void OnCollisionStay(Collision collision)
    {
        string col = collision.transform.name;
        if(!col.Contains("wall"))
        {
            onFloor = true;

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        

      /*  if (collision.transform.name.Contains("boostPad"))
        {
            speedPad speedScript = collision.transform.GetComponent<speedPad>();
            float downThrust = collision.gameObject.GetComponent<speedPad>().downThrust;
            float thrustMult = collision.gameObject.GetComponent<speedPad>().thrustMult;
            Vector3 boostFor = collision.transform.right;
            if (collision.transform.GetComponent<speedPad>().dir)
            {
                boostFor = (speedScript.dir.transform.position - transform.position).normalized;
            }
            Debug.Log(collision.transform.right);
            if (speedScript.x)
            {
                boostFor = new Vector3(boostFor.x, boostFor.y, 0);
            }
            if (speedScript.z)
            {
                boostFor = new Vector3(0, boostFor.y, boostFor.z);
            }
            rbody.AddForce(new Vector3(boostFor.x * thrustMult, boostFor.y * downThrust, boostFor.z * thrustMult), ForceMode.Impulse);
            

        }*/
        

        if (collision.transform.name.Contains("checkpoint"))
        {
            
            bool checkTrue = true;
            foreach (GameObject check in checkpoints)
            {
                if(check == collision.gameObject)
                {
                    checkTrue = false;
                }
            }

            if (checkTrue) 
            {
               
                    checkpoints.Add(collision.gameObject);
                    currentCheckpoint = collision.gameObject;
                
                
                
            }
        }

        if (collision.transform.name.Contains("water"))
        {
            transform.position = new Vector3(currentCheckpoint.transform.position.x, currentCheckpoint.transform.position.y +3, currentCheckpoint.transform.position.z);
            rbody.velocity = Vector3.zero;
        }

        string col = collision.transform.name;
        if (!col.Contains("wall"))
        {
            onFloor = true;
            Debug.Log(col);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent)
        {
            if (collision.transform.parent.name.Contains("checkpoint"))
            {
                bool checkTrue = true;
                foreach (GameObject check in checkpoints)
                {
                    if (check == collision.gameObject)
                    {
                        checkTrue = false;
                    }
                }

                if (checkTrue)
                {
                    checkpoints.Add(collision.transform.parent.gameObject);
                    currentCheckpoint = collision.transform.parent.gameObject;
                }
            }
        }

        if (collision.transform.name.Contains("coin"))
        {
            collision.gameObject.SetActive(false);
            score += 10;
            scoreText.text = score.ToString();
        }

        if (collision.transform.name.Contains("start"))
        {
            doLerp = true;
            Debug.Log("lerp start");
            lerpStart = collision.transform.position;
            lerpEnd = collision.transform.parent.Find("finish").position;
        }

        if (collision.transform.name.Contains("springPad"))
        {
            springPad spring = collision.transform.GetComponent<springPad>();
            if(spring.boostPower > 0f)
            {
                rbody.AddForce(collision.transform.up*spring.boostPower, ForceMode.Impulse);
            }
            else
            {
                rbody.AddForce(new Vector3(0f, 25000f, 0f), ForceMode.Impulse);

            }

        }

        if (collision.transform.name.Contains("boostPad"))
        {
            speedPad speedScript = collision.transform.GetComponent<speedPad>();
            float downThrust = collision.gameObject.GetComponent<speedPad>().downThrust;
            float thrustMult = collision.gameObject.GetComponent<speedPad>().thrustMult;
            Vector3 boostFor = collision.transform.right;
            if (collision.transform.GetComponent<speedPad>().dir)
            {
                boostFor = (speedScript.dir.transform.position - transform.position).normalized;
            }
            Debug.Log(collision.transform.right);
            if (speedScript.x)
            {
                boostFor = new Vector3(boostFor.x, boostFor.y, 0);
            }
            if (speedScript.z)
            {
                boostFor = new Vector3(0, boostFor.y, boostFor.z);
            }
            rbody.AddForce(new Vector3(boostFor.x * thrustMult, boostFor.y * downThrust, boostFor.z * thrustMult), ForceMode.Impulse);


        }

        if (collision.transform.name.Contains("ringBoost"))
        {
            speedPad speedScript = collision.transform.GetComponent<speedPad>();
            Vector3 boostFor = (speedScript.dir.transform.position - transform.position).normalized;
            if (speedScript.x)
            {
                boostFor = new Vector3(boostFor.x, boostFor.y, 0);
            }
            if (speedScript.z)
            {
                boostFor = new Vector3(0, boostFor.y, boostFor.z);
            }
            if (speedScript.thrustMult != 0) { rbody.AddForce(boostFor * speedScript.thrustMult, ForceMode.Impulse); }
            else { rbody.AddForce(boostFor * 30000, ForceMode.Impulse); }
            

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.transform.name.Contains("wall"))
        {
            onFloor = false;
            
        }
    }
}
