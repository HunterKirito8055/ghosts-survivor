using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private float height;
    private float width;

    public Camera ortho;
    public Vector2 up, down, left, right;


    private float widthLeft, widthRight, heightUp, heightDown;

    private bool upClone, downClone, rightClone, leftClone;
    private void Awake()
    {
        height = ortho.orthographicSize;
        width = ortho.orthographicSize * ortho.aspect / 2;

    }
    [SerializeField] private float groundOffsetSize = 48;

    private void LateUpdate()
    {

        widthRight = ortho.transform.position.x + width;
        widthLeft = ortho.transform.position.x - width;
        heightUp = ortho.transform.position.y + height;
        heightDown = ortho.transform.position.x - height;


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (up.y < heightUp &&  !upClone)
        {
            GameObject newGround = Instantiate(gameObject);
            newGround.transform.SetParent(gameObject.transform.parent);
            newGround.transform.position = gameObject.transform.position + new Vector3(0, groundOffsetSize, 0);
            upClone = true;
        }
        if (down.y > heightDown && !downClone)
        {
            GameObject newGround = Instantiate(gameObject);
            newGround.transform.SetParent(gameObject.transform.parent);
            newGround.transform.position = gameObject.transform.position - new Vector3(0, groundOffsetSize, 0);
            downClone = true;
        }
        if (right.x < widthRight && !rightClone)
        {
            GameObject newGround = Instantiate(gameObject);
            newGround.transform.SetParent(gameObject.transform.parent);
            newGround.transform.position = gameObject.transform.position + new Vector3(groundOffsetSize, 0, 0);
            rightClone = true;
        }
        if (left.x > widthLeft && !leftClone)
        {
            GameObject newGround = Instantiate(gameObject);
            newGround.transform.SetParent(gameObject.transform.parent);
            newGround.transform.position = gameObject.transform.position - new Vector3(groundOffsetSize, 0, 0);
            leftClone = true;
        }


    }
}
