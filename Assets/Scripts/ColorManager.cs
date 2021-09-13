using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public Color primaryColor;

    public static Color playerColor;
    public static Color platformLightColor;
    public static Color platformHeavyColor;

    public static Color HitPlatform;
    public static Color PerfectlyHitPlatform;
    private int situation = 0;
    [SerializeField]private SpriteRenderer sprite;
    private float currentValue;
    private IEnumerator colorChangeCoroutine = null;
    private float t = 0.01f;
    [SerializeField]
    private float colorChangeSpeed;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Camera cam;
    [SerializeField]
    public Gradient blackColor = new Gradient
    
    {
        alphaKeys = new[]
        {
            new GradientAlphaKey(1, 0f),
            new GradientAlphaKey(1, 1f)
        },

        colorKeys = new[]
        {
            new GradientColorKey(Color.white, 0f),
            
            new GradientColorKey(Color.red, 1f),
        }
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GradientColorKey[] gck = new GradientColorKey[2];
        
        gck[0].color = Color.white;
        gck[0].time = 0;
        gck[1].color = primaryColor;
        gck[1].time = 1.0f;
        

        blackColor.SetKeys(gck,blackColor.alphaKeys);
        if (colorChangeCoroutine == null)
        {
            if (primaryColor.r == 1 && primaryColor.g == 0 && primaryColor.b == 0)
            {
                //StopAllCoroutines();
                //Debug.Log("coroutine start");
                colorChangeCoroutine = ColorChange(1);
                StartCoroutine(colorChangeCoroutine);
                situation = 1;
            }
            else if (primaryColor.r == 0 && primaryColor.g == 1 && primaryColor.b == 0)
            {
                //StopAllCoroutines();
                colorChangeCoroutine = ColorChange(2);
                StartCoroutine(colorChangeCoroutine);
                //Debug.Log("coroutine start");
                situation = 2;
            }
            else if (primaryColor.r == 0 && primaryColor.g == 0 && primaryColor.b == 1)
            {
                //StopAllCoroutines();
                colorChangeCoroutine = ColorChange(3);
                StartCoroutine(colorChangeCoroutine);
                //Debug.Log("coroutine start");
                situation = 3;
            }
            else if (primaryColor.r == 1 && primaryColor.g == 1 && primaryColor.b == 0)
            {
                //StopAllCoroutines();
                colorChangeCoroutine = ColorChange(4);
                StartCoroutine(colorChangeCoroutine);
                //Debug.Log("coroutine start");
                situation = 4;
            }
            else if (primaryColor.r == 0 && primaryColor.g == 1 && primaryColor.b == 1)
            {
                //StopAllCoroutines();
                //Debug.Log("coroutine start");
                colorChangeCoroutine = ColorChange(5);
                StartCoroutine(colorChangeCoroutine);
                situation = 5;
            }
            else if (primaryColor.r == 1 && primaryColor.g == 0 && primaryColor.b == 1)
            {
                // StopAllCoroutines();
                colorChangeCoroutine = ColorChange(6);
                StartCoroutine(colorChangeCoroutine);
                //Debug.Log("coroutine start");
                situation = 6;
            }
        }
        
        situation = 0;
        sprite.color = blackColor.Evaluate(0.4f);
        //Debug.Log(primaryColor.r);

        t = Time.time*colorChangeSpeed - Mathf.Floor(Time.time * colorChangeSpeed);
           switch (situation)
            {
                case 1:
                    {
                        //primaryColor = new Color(1,Mathf.Min(t,1),0);
                        primaryColor = Color.Lerp(new Color(1, 0, 0), new Color(1, 1, 0), t);
                        break;
                    }

                case 2:
                    {
                        //primaryColor = new Color(0, 1, Mathf.Min(t, 1));
                        primaryColor = Color.Lerp(new Color(0, 1, 0), new Color(0, 1, 1), t);
                        break;
                    }

                case 3:
                    {
                        //primaryColor = new Color(Mathf.Min(t, 1), 0, 1);
                        primaryColor = Color.Lerp(new Color(0, 0, 1), new Color(1, 0, 1), t);
                        break;
                    }

                case 4:
                    {
                        // primaryColor = new Color(1 - Mathf.Max(1,t),1,0);
                        primaryColor = Color.Lerp(new Color(1, 1, 0), new Color(0, 1, 0), t);
                        break;
                    }

                case 5:
                    {
                        //primaryColor = new Color(0, 1 - Mathf.Max(1, t), 1);
                        primaryColor = Color.Lerp(new Color(0, 1, 1), new Color(0, 0, 1), t);
                        break;
                    }

                case 6:
                    {
                        //primaryColor = new Color(1, 0, 1 - Mathf.Max(1, t));
                        primaryColor = Color.Lerp(new Color(1, 0, 1), new Color(1, 0, 0), t);
                        break;
                    }
            }

        playerColor = blackColor.Evaluate(1.0f);
        playerMaterial.SetColor("_EmissionColor",playerColor);
        cam.backgroundColor = blackColor.Evaluate(0.15f);
        platformHeavyColor = blackColor.Evaluate(0.5f);
        platformLightColor = blackColor.Evaluate(0.2f);

        HitPlatform = blackColor.Evaluate(0.3f);
        PerfectlyHitPlatform = blackColor.Evaluate(0.6f);
    }



    IEnumerator ColorChange(int situation)
    {
        
        for (float t = 0.001f; t <= 1.1; t+=Time.deltaTime*colorChangeSpeed)
        {
            switch (situation)
            {
                case 1:
                    {
                        //primaryColor = new Color(1,Mathf.Min(t,1),0);
                        primaryColor = Color.Lerp(new Color(1,0,0), new Color(1,1,0),t);
                        break;
                    }

                case 2:
                    {
                        //primaryColor = new Color(0, 1, Mathf.Min(t, 1));
                        primaryColor = Color.Lerp(new Color(0, 1, 0), new Color(0, 1, 1), t);
                        break;
                    }

                case 3:
                    {
                        //primaryColor = new Color(Mathf.Min(t, 1), 0, 1);
                        primaryColor = Color.Lerp(new Color(0, 0, 1), new Color(1, 0, 1), t);
                        break;
                    }

                case 4:
                    {
                        // primaryColor = new Color(1 - Mathf.Max(1,t),1,0);
                        primaryColor = Color.Lerp(new Color(1, 1, 0), new Color(0, 1, 0), t);
                        break;
                    }

                case 5:
                    {
                        //primaryColor = new Color(0, 1 - Mathf.Max(1, t), 1);
                        primaryColor = Color.Lerp(new Color(0, 1, 1), new Color(0, 0, 1), t);
                        break;
                    }

                case 6:
                    {
                        //primaryColor = new Color(1, 0, 1 - Mathf.Max(1, t));
                        primaryColor = Color.Lerp(new Color(1, 0, 1), new Color(1, 0, 0), t);
                        break;
                    }
            }

            yield return null;
        }

        colorChangeCoroutine = null;
        
        
    }
}
