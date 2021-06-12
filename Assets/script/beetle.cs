using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beetle : MonoBehaviour
{
    public float speed = 4f;
    bool isWait = false;
    bool isHidden = false;
    public float WaitTime = 4f;
    public Transform Point;
    // Start is called before the first frame update
    void Start()
    {
        Point.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (isWait == false)
            transform.position = Vector3.MoveTowards(transform.position, Point.position, speed * Time.deltaTime);
        if (transform.position == Point.position)
        {
            if (isHidden)
            {
                Point.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                isHidden = false;
            } else
            {
                Point.transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
                isHidden = true;
            }
            isWait = true;
            StartCoroutine(Waiting());
              
            
        }
    }
    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(WaitTime);
        isWait = false;
    }
}
