using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField]private List<SpriteRenderer> smallTris;
    [SerializeField] private float emergeSpeed;
    private float emergeSlowSpeed = 3;
    public bool HitPerfect = false;
    private float currentEmergeSpeed;
    public float platformRotation;
    public float platformRotationGap;
    private Quaternion startQuaternion;
    private Quaternion endQuaternion;
    [SerializeField] private GameObject player;
    private List<GameObject> linkedPlatform;

    public Color OriginColor;
    public Color PlatformColor;
    [SerializeField]private SpriteRenderer platformSpriteRenderer;
    
    private Vector3 startPosition;
    [SerializeField] private Transform firstAngle;
    [SerializeField] private Transform secondAngle;
    [SerializeField] private Transform thirdAngle;
    public bool notOn = true;
    public bool platformDisappear = false;
    // Start is called before the first frame update
    void Start()
    {

        linkedPlatform = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (notOn)
        {
            transform.rotation = player.transform.rotation;
        }
    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        foreach (SpriteRenderer a in smallTris)
        {
            a.color = new Color(a.color.r, a.color.g, a.color.b, 0);
        }
        notOn = true;
        platformSpriteRenderer.color = OriginColor;
        if (player!=null&&gameObject!=null)
        {
            player.GetComponent<PlayerStateManager>().inactivePlatformList.Add(gameObject);
        }
        
        platformDisappear = false;
        HitPerfect = false;

    }
    IEnumerator PlatformEmerge()
    {
        PlatformColor = ColorManager.platformLightColor;
        foreach (SpriteRenderer sprite in smallTris)
        {
            sprite.color = ColorManager.platformHeavyColor;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
        }
        startQuaternion = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        endQuaternion = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, (Mathf.Round(transform.rotation.eulerAngles.z / 360) * 360 + 60));

        for (float t = 0; t <= 1.1; t+=Time.deltaTime*1.5f*PlayerStateManager.intensity)
        {
            platformSpriteRenderer.color = Color.Lerp(OriginColor,PlatformColor,t);
            foreach (SpriteRenderer sprite in smallTris)
            {
                sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,Mathf.Min(1,t));
            }
            transform.position =new Vector3( Vector3.Slerp(startPosition,player.transform.position,t).x, Vector3.Slerp(startPosition, player.transform.position, t).y,transform.position.z);

            transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, t);
            yield return null;
        }
        //change transform

        //change color

        //rotate

        
    }
    IEnumerator PlatformRotate()
    {
        startQuaternion = transform.rotation;
        endQuaternion = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 120);
        for (float t = 0; t <= 1.1; t += Time.deltaTime * platformRotation)
        {
            transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, t);
            yield return null;
        }

    }


    IEnumerator PlatformDisappear()
    {
        if (HitPerfect)
        {
            currentEmergeSpeed = emergeSlowSpeed;
        }
        else
        {
            currentEmergeSpeed = emergeSpeed;
        }
       
        for (float i =smallTris.Count-0.0001f ; i >= -0.2f; i-= Time.deltaTime*currentEmergeSpeed*PlayerStateManager.intensity)
        {
            if (Mathf.FloorToInt(i) + 1 < smallTris.Count)
            {
                smallTris[Mathf.FloorToInt(i) + 1].color = new Color(smallTris[Mathf.FloorToInt(i) +1].color.r,
                 smallTris[Mathf.FloorToInt(i) +1].color.g, smallTris[Mathf.FloorToInt(i) + 1].color.b,0);
            }
             smallTris[Mathf.Max(Mathf.FloorToInt(i),0)].color = new Color(smallTris[Mathf.Max(Mathf.FloorToInt(i), 0)].color.r, 
                 smallTris[Mathf.Max(Mathf.FloorToInt(i), 0)].color.g, smallTris[Mathf.Max(Mathf.FloorToInt(i), 0)].color.b, Mathf.Max(0,(i - Mathf.Floor(i)) * Mathf.Sign(i)));
            //Debug.Log("AAA");
             yield return null;
         }
        platformDisappear = true;
    }
    public void platformGetHit()
    {
        notOn = false;
        startPosition = transform.position;
        StartCoroutine(PlatformEmerge());
    }

    public void PlatformStartDisappear()
    {
        IEnumerator newCoroutine= (PlatformDisappear());
        StartCoroutine(newCoroutine);
    }
    IEnumerator PlatformRotateFre(float gap)
    {
        while (true)
        {
            StartCoroutine(PlatformRotate());
            yield return new WaitForSeconds(gap);
            //Debug.Log("Sending");
        }
    }


    public void SpawnPlatform(int a)
    {
        List<Transform> transformList = new List<Transform>();
        transformList.Add(firstAngle);
        transformList.Add(secondAngle);
        transformList.Add(thirdAngle);
        for (int i = 0; i < a; i++)
        {
            GameObject newplatform = player.GetComponent<PlayerStateManager>().inactivePlatformList[0];
            player.GetComponent<PlayerStateManager>().inactivePlatformList.RemoveAt(0);
            int b = Random.Range(0,transformList.Count);
            newplatform.transform.position = player.transform.position + transformList[b].transform.up * (Random.Range(PlayerStateManager.minSpawnRange, PlayerStateManager.maxSpawnRange));
            transformList.RemoveAt(b);
            newplatform.SetActive(true);
            linkedPlatform.Add(newplatform);
        }
    }

    public void DisablePlatform(GameObject currentPlatform)
    {
        foreach (GameObject platform in linkedPlatform)
        {
            if (platform!=currentPlatform)
            {
                platform.SetActive(false);
            }
        }
        linkedPlatform.Clear();
        gameObject.SetActive(false);
    }
}
