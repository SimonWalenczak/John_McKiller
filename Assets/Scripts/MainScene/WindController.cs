using System;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public static WindController Instance;
    
    public Transform[] sails;
    public Transform sailRef;
    public Transform windOrigin;

    public float maxScale = 3f;
    public float minScale = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already another Wind Controller in this scene !");
        }
    }

    void Update()
    {
        float dotProduct = Vector3.Dot(sailRef.transform.forward, windOrigin.forward);

        float angle = Vector3.Angle(sailRef.transform.forward, windOrigin.forward);

        if (Mathf.Approximately(angle, 180f))
        {
            for (int i = 0; i < sails.Length; i++)
            {
                sails[i].transform.localScale = new Vector3(sailRef.transform.localScale.x,
                    sailRef.transform.localScale.y, minScale);
            }
        }
        else
        {
            float scale = Mathf.Lerp(minScale, maxScale, Mathf.InverseLerp(-1f, 1f, dotProduct));

            for (int i = 0; i < sails.Length; i++)
            {
                sails[i].transform.localScale =
                    new Vector3(sailRef.transform.localScale.x, sailRef.transform.localScale.y, scale);
            }
        }
    }
}