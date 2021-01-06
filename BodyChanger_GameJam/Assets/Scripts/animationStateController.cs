using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{

    //private Rigidbody _rigidbody;
    public float moveSpeed;
    Animator anim;
    private CharacterController _controller;
    private float tDist = 5;

    int isWalkingHash;
    int isRunningHash;
    int isWalkingBackHash;
    int isAttackHash;
    int isDeadHash;

    Camera camera;

    void Start()
    {
        //_rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        _controller = GetComponent<CharacterController>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isWalkingBackHash = Animator.StringToHash("isWalkingBack");
        isAttackHash = Animator.StringToHash("isAttack");
        isDeadHash = Animator.StringToHash("isDead");

    }

    void Update()
    {
        Move();
        CaptureBody();
    }

    void Move()
    {

        float ver = Input.GetAxis("Vertical");
        Vector3 playerMovement = transform.forward * ver * moveSpeed * Time.deltaTime;
        //_rigidbody.MovePosition(_rigidbody.position+playerMovement); 
        _controller.Move(playerMovement+((_controller.isGrounded)?Vector3.zero:Vector3.down*100*Time.deltaTime));

        bool isRunning = anim.GetBool(isRunningHash);
        bool isWalking = anim.GetBool(isWalkingHash);
        bool isWalkingBack = anim.GetBool(isWalkingBackHash);
        bool isAttack = anim.GetBool(isAttackHash);

        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");
        bool backwardPressed = Input.GetKey("s");
        bool attackPressed = Input.GetKeyDown(KeyCode.Mouse0);


        if (!isWalking && forwardPressed)
        {
            anim.SetBool(isWalkingHash, true);
            moveSpeed = 25;
        }

        if (isWalking && !forwardPressed)
        {
            anim.SetBool(isWalkingHash, false);
        }

        if (!isRunning && (forwardPressed && runPressed))
        {
            anim.SetBool(isRunningHash, true);
            moveSpeed = 50;
        }

        if (isRunning && !forwardPressed || !runPressed)
        {
            anim.SetBool(isRunningHash, false);
            moveSpeed = 25;
        }

        if (!isWalkingBack && backwardPressed)
        {
            anim.SetBool(isWalkingBackHash, true);
            moveSpeed = 10; // çalışmıyor?? geri giderken hızı düşürmüyor.
        }

        if (isWalkingBack && !backwardPressed)
        {
            anim.SetBool(isWalkingBackHash, false);
        }

        if (!isAttack && attackPressed)
        {
            anim.SetBool(isAttackHash, true);
        }

        if (isAttack && !attackPressed)
        {
            anim.SetBool(isAttackHash, false);
        }


    }

    void CaptureBody()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject nearBody = FindClosestEnemy();

            if (nearBody != null && nearBody.CompareTag("Body"))
            {
                Vector3 velocity = Vector3.zero;
                nearBody.GetComponent<animationStateController>().enabled = true;
                nearBody.GetComponent<ThirdPersonCameraController>().enabled = true;

                nearBody.gameObject.tag = "Player";

                nearBody.transform.Find("Camera").gameObject.SetActive(true);
                this.transform.Find("Camera").gameObject.SetActive(false);

                this.gameObject.tag = "Dead";
                this.GetComponent<animationStateController>().enabled = false;
                this.GetComponent<ThirdPersonCameraController>().enabled = false;
                this.anim.SetBool(isDeadHash, true);

            }
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Body");
        GameObject closest = null;
        float distance = tDist;
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
            if (go == this.gameObject)
                continue;

            float curDistance = Vector3.Distance(go.transform.position, position);
            curDistance = Mathf.Abs(curDistance);

            if (curDistance > tDist)
                continue;

            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }


}
