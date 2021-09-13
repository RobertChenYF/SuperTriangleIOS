using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed;
    private float speedLastFrame;
    private float acc;
    private bool ifDashing = false;
    [SerializeField] private float dashSpeed;
    public float playerRotation;
    public float playerRotationGap;
    [SerializeField]private ParticleSystem backGroundParticle;
    [SerializeField] private ParticleSystem stopParticle;
    private Quaternion startQuaternion;
    private Quaternion endQuaternion;

    private IEnumerator dashCoroutine;
    [SerializeField]private AnimationCurve dashCurve;

    // Start is called before the first frame update
    void Start()
    {
       
        StartCoroutine(PlayerRotateFre(playerRotationGap));
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space)&&ifDashing == false)
        {
            transform.SetParent(null);
            ShortDash();
        }
        else if (Input.GetKeyUp(KeyCode.Space) && ifDashing == true)
        {
            StopDash();
        }

        if (speed == 0)
        {
            ifDashing = false;
        }
        ObjectScale();
        var rotationOverLifetimeBackGroundParticle = backGroundParticle.rotationOverLifetime;
        rotationOverLifetimeBackGroundParticle.z = Mathf.Min(speed*3 + 1, 20) * 1;
    }



    private void ShortDash()
    {
        
        dashCoroutine =  ShortDashMovement();
        StartCoroutine(dashCoroutine);
    }


    private void StopDash()
    {
        speed = 0;
        StopCoroutine(dashCoroutine);
        ifDashing = false;
        stopParticle.Emit(1);
    }
    IEnumerator ShortDashMovement()
    {
        
        for(float t = 0; t <=1; t+= Time.deltaTime*2)
        {
            speed = dashCurve.Evaluate(Mathf.Min(t,1))*dashSpeed;
            transform.position += transform.up * speed * Time.deltaTime;
            ifDashing = true;
            yield return null;
        }
        speed = 0;
    }

    private void ObjectScale()
    {
        acc = speed - speedLastFrame;
        transform.localScale = new Vector3(Mathf.Min(3.0f,Mathf.Max(0.5f, 1 - acc * 0.3f)), Mathf.Max(0.3f,Mathf.Min(1 + acc * 1.0f, 2.5f)), 1);
        speedLastFrame = speed;
    }

    IEnumerator PlayerRotate()
    {
        startQuaternion = transform.rotation;
        endQuaternion = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 120);
        for (float t = 0; t <=1.1; t+=Time.deltaTime*playerRotation)
        {
            transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, t);
            yield return null;
        }
        
    }

    IEnumerator PlayerRotateFre(float gap)
    {
        while (true)
        {
            StartCoroutine(PlayerRotate());
            yield return new WaitForSeconds(gap);
            //Debug.Log("Sending");
        }
    }
}
