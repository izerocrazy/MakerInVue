/****************************************************************************
Copyright (c) 2014-2016 Beijing TianRuiDiAn Network Technology Co.,Ltd.
Copyright (c) 2014-2016 ShenZhen Redbird Network Polytron Technologies Inc.
 
http://www.hotniao.com

All of the content of the software, including code, pictures, 
resources, are original. For unauthorized users, the company 
reserves the right to pursue its legal liability.
****************************************************************************/

using UnityEngine;
using System.Collections;

public class InputAccVal : MonoBehaviour
{

    private Vector3 currAcc = Vector3.zero;
    private Vector3 preAcc = Vector3.zero;
    private Vector3 sumAcc = Vector3.zero;
    private long count;
    private Vector3 deltaVec = Vector3.zero;
    private Vector3 moveVec = Vector3.zero;


    private float moveSpeed = 16.18f;
    private float maxXval = 0.3814f;
    private float maxYval = 0.618f;
    private float maxRotX = 2.618f;
    private float maxRotY = 1f;
    private Vector3 RotTo;
    private void Start()
    {

    }
    public void EndPos()
    {
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
    }
    public void UpdatePos()
    {
        Vector3 vector = this.transform.localPosition;
        this.currAcc = Input.acceleration;//重力感应
        this.deltaVec = this.currAcc - this.preAcc;
        if (this.currAcc.z < 0f)
        {
            this.moveVec.x = this.deltaVec.x;
            this.moveVec.y = this.deltaVec.y;
            this.moveVec.z = 0f;
        }
        else
        {
            this.moveVec.x = -this.deltaVec.x;
            this.moveVec.y = -this.deltaVec.y;
            this.moveVec.z = 0f;
        }

        this.sumAcc += this.currAcc;
        this.count += 1L;
        this.preAcc = this.sumAcc / (float)this.count;
        vector -= this.moveVec * Time.deltaTime * this.moveSpeed;
        if (vector.x < -this.maxXval)
        {
            vector.x = -this.maxXval;
        }
        if (vector.x > this.maxXval)
        {
            vector.x = this.maxXval;
        }
        if (vector.y < -this.maxYval)
        {
            vector.y = -this.maxYval;
        }
        if (vector.y > this.maxYval)
        {
            vector.y = this.maxYval;
        }
        this.transform.localPosition = vector;
        float num = Mathf.InverseLerp(-this.maxXval, this.maxXval, vector.x);
        num = -Mathf.Lerp(-this.maxRotY, this.maxRotY, num);
        float num2 = Mathf.InverseLerp(-this.maxYval, this.maxYval, vector.y);
        num2 = Mathf.Lerp(-this.maxRotX, this.maxRotX, num2);
        this.transform.localRotation = Quaternion.Euler(num2, num, 0f);
    }
}
