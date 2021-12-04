using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;


public class Hand : MonoBehaviour
{
    List<InputDevice> devices;
    InputDevice targetDevice;

    [SerializeField] GameObject OtherHand;
    [SerializeField] Rigidbody PlayerRB;
    [SerializeField] Transform PlayerT;
    [SerializeField] Material defaultMat;
    [SerializeField] Material grabbedMat;
    [SerializeField] GameObject Gun;
    [SerializeField] GameObject ThrusterParticles;
    [SerializeField] GameObject Projectile;
    [SerializeField] int handNum = -1;
    [SerializeField] bool lastTriggerDown = false;
    public bool grabOverridden = false;

    [SerializeField] AudioSource grab;
    [SerializeField] AudioSource pistolShoot;
    [SerializeField] AudioSource pistolEquip;
    [SerializeField] AudioSource pistolUnequip;
    [SerializeField] AudioSource thruster;
    [SerializeField] AudioSource thrusterIgnite;
    [SerializeField] AudioSource thrusterOut;

    public Menu menu { get { return GameObject.Find("Menu").GetComponent<Menu>(); } }

    Vector3 lastPosition;
    Vector3 lastPlayerPosition;
    float triggerAxis = 0;
    float gripAxis = 0;
    bool thrusterOutPlayed;

    float moveSpeed = 1.5f;
    int histLength = 0;
    float thrusterSpeed = 1.5f;
    float vibDurationRemaining = 0f;
    const float vibOneFrame = 0.01f;

    int touchingBranches = 0;
    bool grabbed = false;
    int grabbedSampleCount = 0;
    bool interior = false;
    bool jumpSaving = false;
    bool alreadyCol = false;
    bool gunMode = false;

    Head head = null;

    Vector3[] velHist;

    Vector3 grabPos;
    Vector3 grabMove;

    bool[] handAssigned = new bool[3] { false, false, false };

    GameProgress gameProgress { get { return GameObject.Find("GameProgress").GetComponent<GameProgress>(); } }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(handNum == 2 || handNum == 1);
        Debug.Assert(handAssigned[handNum] == false);
        handAssigned[handNum] = true;
        //Debug.Assert(histLength == 0);
        histLength = (int)(1 / Time.fixedDeltaTime);
        //Debug.Assert(histLength == 50);
        velHist = new Vector3[histLength];
        devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        head = GameObject.Find("Head").GetComponent<Head>();
        PlayerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Vibrate(float strength, float duration)
    {
        OVRInput.SetControllerVibration(1f, strength, (handNum == 1 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
        vibDurationRemaining = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (devices==null || devices.Count < 3)
        {
            return;
        }
        //Debug.Log(devices.Count);

        alreadyCol = false;

        try
        {
            targetDevice = devices[handNum];
        }
        catch (Exception ex)
        {
            Debug.Log("Failed to index into hand " + handNum);
            return;
        }


        //targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripAxis);
        targetDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerAxis);
        targetDevice.TryGetFeatureValue(CommonUsages.grip, out gripAxis);
        targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 myPos);
        targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion myRot);
        //targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool unusedButton);
        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton);
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton);
        targetDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButton);

        Debug.Assert(menu != null);
        menu.ReportControllerState(handNum, primaryButton, secondaryButton, menuButton);

        if (handNum == 1) {
            head.levitation = secondaryButton;
        }

        if (handNum == 2)
        {
            if (secondaryButton && !jumpSaving)
            {
                head.JumpSave();
            }
            jumpSaving = secondaryButton;
        }

        if((primaryButton && !head.thrustRanOut) != thruster.isPlaying)
        {
            if (primaryButton)
            {
                thruster.Play();
                thrusterIgnite.Play();
                thruster.time = head.thrustMax-head.thrustRemaining;
            }
            else
            {
                thruster.Stop();
            }
        }

        ThrusterParticles.SetActive(primaryButton && !head.thrustRanOut);

        if (primaryButton)
        {
            if (!head.thrustRanOut)
            {
                Vector3 impulseDirection = transform.rotation * Vector3.forward;

                head.Impulse(impulseDirection * thrusterSpeed * Time.deltaTime);

                Vibrate(0.1f, vibOneFrame);
                thrusterOutPlayed = false;
            }
            else
            {
                if (!thrusterOutPlayed)
                {
                    thrusterOut.Play();
                    thrusterOutPlayed = true;
                }
            }

            head.usingThrust = true;
        }

        transform.localPosition = myPos;
        transform.localRotation = myRot;

        

        if (grabbed && !grabOverridden)
        {
            grabMove = grabPos - transform.position;
            PlayerT.position += grabMove;
        }

        
        interior = CheckInterior();

        if (gunMode != GripDown())
        {
            gunMode = GripDown();

            Gun.SetActive(gunMode);
            GetComponent<Renderer>().enabled = !gunMode;

            if (gunMode)
            {
                pistolEquip.Play();
            }
            else
            {
                pistolUnequip.Play();
            }
        }

        if (TriggerDown() && !lastTriggerDown && gunMode)
        {
            Instantiate(Projectile, transform.position, transform.rotation);
            pistolShoot.Play();
            Vibrate(0.25f, 0.1f);
        }

        //GRABBING
        if (TriggerDown() && !gunMode && (touchingBranches > 0 || interior) && !grabOverridden)
        {
            if (!grabbed) {

                //LogText.text = ""+PlayerRB.velocity.magnitude;

                grab.Play();

                Vibrate(PlayerRB.velocity.magnitude/20, 0.05f);

                GetComponent<Renderer>().material = grabbedMat;
                
                if (grabPos == new Vector3(0, 0, 0))
                {
                    grabPos = transform.position;
                    //Debug.Log("GrabPos " + grabPos);
                }
                
                OtherHand.GetComponent<Hand>().grabOverridden = true;
            }


            grabbed = true;

            PlayerRB.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            grabPos = new Vector3(0, 0, 0);
            //Debug.Log("GrabPos " + grabPos);

            PlayerRB.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);

            if (grabbed)
            {
                GetComponent<Renderer>().material = defaultMat;

                PlayerRB.velocity = SwingVel();
            }

            grabbed = false;

             
        }

        if (TriggerDown())
        {
            head.gravGrace = false;
        }

        //if (!(TriggerDown() && (touchingBranches > 0 || interior)))
        if(!TriggerDown())
        {
            grabOverridden = false;
        }

        if (vibDurationRemaining <= 0f)
        {
            OVRInput.SetControllerVibration(0, 0, (handNum == 1 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
            vibDurationRemaining = 0f;
        }

        if (vibDurationRemaining > 0f)
        {
            vibDurationRemaining -= Time.deltaTime;
        }

        lastPosition = transform.position;
        lastPlayerPosition = PlayerT.position;
        lastTriggerDown = TriggerDown();
    }

    bool TriggerDown()
    {
        return triggerAxis > 0.75;
    }

    bool GripDown()
    {
        return gripAxis > 0.75;
    }

    bool CheckInterior()
    {
        Vector3 castDirection = GetComponent<Transform>().position - head.GetComponent<Transform>().position;

        float length = castDirection.magnitude;

        int layerMask = 1 << 9;

        return Physics.Raycast(head.GetComponent<Transform>().position, castDirection.normalized, length, layerMask);

        //return Physics.Raycast(new Vector3(-1, 10, 0), new Vector3(1, 10, 0), 100);
    }
    
    Vector3 SwingVel()
    {
        Debug.Assert(histLength != 0);

        //collects dotproducts of hand movement vectors

        //OHHHH SHIT DAWG! Higher dotproducts mean straighter angles between two vectors. This makes an array that start only once the dotproducts are above
        //the validThreshold (straight enough) and ends only once they're below the validThreshold.

        bool straightEnough = false;
        float validThreshold = 0.8f; //~35 degrees
        List <Vector3> validHist = new List<Vector3>();
        for (int i = 0; i < grabbedSampleCount-1; i++)
        {
            float dotProduct = Vector3.Dot(velHist[i].normalized, velHist[i + 1].normalized);
            if (dotProduct > validThreshold) { straightEnough = true; }
            if (straightEnough)
            {
                validHist.Add(velHist[i]);
                if (dotProduct <= validThreshold ) { break; }
            }
        }

        Vector3 AvgVel = new Vector3(0, 0, 0);

        if (validHist.Count == 0)
        {
            return AvgVel;
        }


        //Use angle of whole swing but only velocity of last few because we want the release velocity.
        for (int i = 0; i < validHist.Count; i++)
        {
            AvgVel += validHist[i] / validHist.Count;
        }

        Vector3 angle = AvgVel.normalized;


        float velocity = 0.0f;
        int velSamplesTaken = Mathf.Min(validHist.Count, 3);
        for (int i = 0; i < velSamplesTaken; i++)
        {
            velocity += validHist[i].magnitude / velSamplesTaken;
        }

        AvgVel = angle * Mathf.Min(velocity * moveSpeed, head.moveMaxSpeed);


        return AvgVel;
    }

    void setLastStable(GameObject terrain)
    {
        if (terrain.layer == LayerMask.NameToLayer("GrabbableTerrain") && terrain.tag != "NoJumpSave" && grabbed && PlayerT.position.y > head.abyssY+10)
        {
            head.setLastStable(PlayerT.position);
        }
    }

    void FixedUpdate()
    {
        for (int i = histLength - 2; i >= 0; i -= 1)
        {
            velHist[i + 1] = velHist[i];
                
        }

        if (grabbed)
        {
            velHist[0] = grabMove / Time.deltaTime;
            grabbedSampleCount = Mathf.Min(grabbedSampleCount+1,histLength);
            //Debug.Log("velHist[0]="+ velHist[0]);
        }
        else {
            velHist[0] = new Vector3(0, 0, 0);
            grabbedSampleCount = 0;
            //Debug.Log("clearing velHist[0]");
        }
    }
        
    //HAND IS GRABBING WHILE ALREADY GRABBED CREATING SKITTER
    void OnCollisionEnter(Collision collision)
    {
        setLastStable(collision.collider.gameObject);
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            //Debug.Log("ColEnt");
            if(TriggerDown() && !alreadyCol)
            {
                if (!grabbed) { grabPos = collision.contacts[0].point; }
                //Instantiate(Indicator, collision.contacts[0].point, Quaternion.Euler(0, 0, 0));

                alreadyCol = true;
                //touched = true;

                //PlayerRB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                PlayerRB.velocity = new Vector3(0, 0, 0);
            }
            if(touchingBranches == 0 && !interior)
            {
                Vibrate(0.01f, vibOneFrame);
            }
            touchingBranches += 1;
        }

        if (collision.collider.gameObject.tag == "Vase")
        {
            head.Deposit(collision.collider.gameObject.GetComponent<Vase>());
        }

        Item item = collision.collider.gameObject.GetComponent<Item>();
        if (item != null)
        {
            Debug.Log("Hand touched " + item.gameObject.name);
            bool basketFull = head.heldScore >= head.maxHeldScore;
            if (!item.allowCollectWhenBasketFull && basketFull)
            {
                Debug.Log("Basket full " + head.heldScore);
                return;
            }
            if (!basketFull)
            {
                Debug.Log("Collected " + item.points);
                head.GetPoint(item.points);
            }
            //Debug.Log("item");
            item.OnHandCollide();
            Vibrate(item.vibDur, 0.5f);
            
        }
    }

    void OnCollisionExit(Collision collision)
    {
        setLastStable(collision.collider.gameObject);
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            //Debug.Log("ColEx");
            touchingBranches -= 1;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        setLastStable(collision.collider.gameObject);
    }
}