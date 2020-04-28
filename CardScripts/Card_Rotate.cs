using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Rotate : MonoBehaviour
{
    private float speedRotate = 295f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RotateForSeconds());
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public IEnumerator RotateForSeconds()
        {
            float time = .6f;
            float speed = .17f;
            yield return new WaitForSeconds(.1f);

            while (time > 0)
            {
                
                transform.Rotate(Vector3.forward, speedRotate * Time.deltaTime);
            transform.localPosition = Vector3.MoveTowards(
               transform.localPosition,
               new Vector3(transform.localPosition.x, .1f, transform.localPosition.z),
               speed * Time.deltaTime);
                time -= Time.deltaTime;

                yield return null;
            }
            transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        transform.localPosition = new Vector3(transform.localPosition.x, .1f, transform.localPosition.z);
    }


}


