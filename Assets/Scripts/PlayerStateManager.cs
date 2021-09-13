using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerStateManager : MonoBehaviour
{
    private PlayerState currentPlayerState;
    public static float speed;
    [SerializeField]private GameObject Player;
    [SerializeField] private GameObject Camera;
    private Vector3 cameraSlerpStartLocation;
    private Vector3 cameraOffset;
    private float speedLastFrame;
    private float acc;
    
    [SerializeField] private float dashSpeed;
    public float playerRotation;
    public float playerRotationGap;
    [SerializeField] private ParticleSystem backGroundParticle;
    [SerializeField] private ParticleSystem stopParticle;
    private Quaternion startQuaternion;
    private Quaternion endQuaternion;
    public float cameraRotationSpeed;
    private IEnumerator dashCoroutine;
    public IEnumerator cameraFollowCoroutine;
    private IEnumerator playerRotate;
    public GameObject currentPlatform;

    [SerializeField]private AnimationCurve cameraRotationSpeedCurve;

    [SerializeField]private GameObject playerChild;
    public Color ColorWhenNotHitPlatform;
    public Color ColorWhenHitPlatform;
    public Color ColorWhenPerfectlyHitPlatform;
    public Color LerpColor;
    [SerializeField] private AnimationCurve dashCurve;
    [SerializeField] private AnimationCurve CameraCurve;
    private Vector2[] trianglePoints;
    [SerializeField]private PolygonCollider2D polygonCollider;
    [HideInInspector]public static Collider2D platformCollider;
    public List<GameObject> inactivePlatformList;
    public static float minSpawnRange = 3;
    public static float maxSpawnRange = 6;

    public static float intensity = 0.7f;
    private float timer = 0;
    [SerializeField]private GameObject GameoverScreen;
    public static int hp;
    [SerializeField]private AudioSource audioSource;
    [SerializeField]private AudioClip snare;
    [SerializeField] private AudioSource secondaryAudioSource;
    [SerializeField] private AudioClip drum;

    [SerializeField]private Material ParticleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        ParticleMaterial.SetColor("_EmissionColor", Color.white);
        cameraOffset = Camera.transform.position - Player.transform.position;
        currentPlatform.GetComponentInParent<PlatformController>().platformGetHit();
        intensity = 0.6f;
        trianglePoints = new Vector2[3];
        ChangeState(new NoInput(this));
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayerState.stateBehavior();
        cameraRotationSpeed = (cameraRotationSpeedCurve.Evaluate(timer - Mathf.Floor(timer)) - 0.5f)*intensity*30;
        
        timer += (Time.deltaTime * Random.Range(0.6f, 2.0f))*0.03f;

        intensity += Time.deltaTime * 0.03f;
        intensity = Mathf.Min(2.5f, intensity);
        ColorWhenHitPlatform = ColorManager.HitPlatform;
        ColorWhenPerfectlyHitPlatform = ColorManager.PerfectlyHitPlatform;
    }


    public void ChangeState(PlayerState newPlayerState)
    {
        if (currentPlayerState != null) currentPlayerState.Leave();
        currentPlayerState = newPlayerState;
        currentPlayerState.Enter();
    }

    public void ObjectScale()
    {
        acc = speed - speedLastFrame;
        transform.localScale = new Vector3(Mathf.Min(2.0f, Mathf.Max(0.5f, 1 - acc * 0.3f)), Mathf.Max(0.5f, Mathf.Min(1 + acc * 1.0f, 2.5f)), 1);
        //Debug.Log("Scalea");
        speedLastFrame = speed;
    }
    IEnumerator PlayerRotate()
    {
        startQuaternion = Quaternion.Euler( transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        endQuaternion = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, (Mathf.Round(transform.rotation.eulerAngles.z / 120) + 1) * 120);
        for (float t = 0; t <= 1.1; t += Time.deltaTime * playerRotation*intensity)
        {
            transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, t);
            yield return null;
        }

    }

    IEnumerator PlayerRotateFre()
    {
        while (true)
        {
            StartCoroutine(PlayerRotate());
            playerRotationGap = (2.5f - intensity) * 0.6f + 0.5f;
            yield return new WaitForSeconds(playerRotationGap);
            
        }
    }

    public void PlayerStartRotate()
    {
        playerRotate = PlayerRotateFre();
        StartCoroutine(playerRotate);
    }

    public void PlayerStopRotate()
    {
        
        StopCoroutine(playerRotate);
        playerRotate = null;
    }
    IEnumerator ShortDashMovement()
    {

        for (float t = 0; t <= 1; t += Time.deltaTime * 2)
        {
            speed = dashCurve.Evaluate(Mathf.Min(t, 1)) * dashSpeed;
            transform.position += transform.up * speed * Time.deltaTime;
            
            yield return null;
        }
        speed = 0;
    }

    public void ShortDash()
    {

        dashCoroutine = ShortDashMovement();
        StartCoroutine(dashCoroutine);
    }

    public void TransformBackToNormal()
    {
        transform.localScale= new Vector3(1,1,1);
       // Debug.Log("scale");
    }

    public void StopDash()
    {
        speed = 0;
        StopCoroutine(dashCoroutine);
        
        //stopParticle.Emit(1);
    }

    public void BackGroundParticleRotate()
    {
        var rotationOverLifetimeBackGroundParticle = backGroundParticle.rotationOverLifetime;
        rotationOverLifetimeBackGroundParticle.z = Mathf.Min(speed * 3 + 1, 20) * 1;
    }

    public void BackGroundParticleStop()
    {
        var rotationOverLifetimeBackGroundParticle = backGroundParticle.rotationOverLifetime;
        rotationOverLifetimeBackGroundParticle.z = 0;
    }

    public void BackGroundParticleTurnColor(Color targetColor)
    {
        StartCoroutine(ParticleColorChange(targetColor));
    }

    public void BackGroundParticleTurnColorBack()
    {
       // ParticleMaterial.EnableKeyword("_EMISSION");
    }

    IEnumerator ParticleColorChange(Color targetColor)
    {
        
        for (float t = 0; t <=2.2; t += Time.deltaTime*3)
        {
            
            if (t<=1)
            {
                LerpColor = Color.Lerp(Color.white,targetColor,t);
                //Debug.Log("aaa");
            }
            else
            {
                LerpColor = Color.Lerp(targetColor, Color.white, Mathf.Max(0,(t-1.2f)));
            }
            ParticleMaterial.SetColor("_EmissionColor", LerpColor);
            ParticleMaterial.EnableKeyword("_EMISSION");
            yield return null;
        }
    }


    public void CameraStartFollow()
    {
        cameraSlerpStartLocation = Camera.transform.position;
        cameraFollowCoroutine = CameraFollow();
        StartCoroutine(cameraFollowCoroutine);
    }
    IEnumerator CameraFollow()
    {
        for (float t = 0;t<=1.1f;t+=Time.deltaTime*1.7f*intensity)
        {

            Camera.transform.position = new Vector3(Vector3.SlerpUnclamped(cameraSlerpStartLocation,Player.transform.position+cameraOffset,CameraCurve.Evaluate(Mathf.Min(t,1))).x,
                Vector3.LerpUnclamped(cameraSlerpStartLocation, Player.transform.position + cameraOffset, CameraCurve.Evaluate(Mathf.Min(t, 1))).y,-10);
            yield return null;

        }
        cameraFollowCoroutine = null;
    }


    public void CameraRotate()
    {
        Camera.transform.Rotate(0, 0, Time.deltaTime * cameraRotationSpeed, Space.Self);
    }

    public int CheckCollision()
    {
        if (platformCollider != null&&platformCollider.gameObject!=currentPlatform)
        {
            //trianglePoints = polygonCollider.points;
            for (int i =0; i <3; i++)
            {
                trianglePoints[i] = polygonCollider.points[i];
               // Debug.Log(trianglePoints[i]);
            }
            Vector2 offset = new Vector2(polygonCollider.gameObject.transform.position.x, polygonCollider.gameObject.transform.position.y);
            currentPlatform.GetComponentInParent<PlatformController>().DisablePlatform(platformCollider.gameObject.transform.parent.gameObject);
            currentPlatform = platformCollider.gameObject;
            if (platformCollider.bounds.Contains(trianglePoints[0] + offset) && platformCollider.bounds.Contains(trianglePoints[1] + offset) && platformCollider.bounds.Contains(trianglePoints[2] + offset))
            {
                //Debug.Log("fully contained");
                audioSource.PlayOneShot(snare);
                return 2;
            }

            else
            {
                secondaryAudioSource.PlayOneShot(drum);
                return 1;
            }
        }
        else
        {
            return 0;
        }
    }

    public void ActivatePlatform()
    {
        platformCollider.gameObject.GetComponentInParent<PlatformController>().platformGetHit();
    }

    public void StartPlatformDisappear()
    {
        currentPlatform.GetComponentInParent<PlatformController>().PlatformStartDisappear();
    }
   
    public void Gameover()
    {
        GameoverScreen.SetActive(true);
        playerChild.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}