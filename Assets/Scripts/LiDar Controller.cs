using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class LiDarController : MonoBehaviour
{
    private Physics _physics;
    private bool spraying;
    private float cooldownTimer;
    private LayerMask wallMask;
    private List<GameObject> scanMemory = new List<GameObject>();
    [Header("Setup")] [SerializeField] private Transform origin;
    [SerializeField] private List<Material> reset;

    [Header("Parameters")] [SerializeField]
    private float maxRadius;

    [SerializeField] private float maxDistance;
    [SerializeField] private float angle;
    [SerializeField] private int pixelsPerSpray;
    [SerializeField] private float sprayInterval;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!spraying)
            {
                Spray();
                spraying = true;
            }
        }

        if (spraying)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= sprayInterval)
            {
                spraying = false;
                cooldownTimer = 0;
            }
        }
    }

    private void Start()
    {
        wallMask = LayerMask.GetMask("Wall");
    }

    private void Spray()
    {
        var origin = transform.position + (transform.forward * 0.5f * maxRadius);
        RaycastHit[] coneHits =
            Physics.SphereCastAll(origin, maxRadius, transform.forward, maxDistance,wallMask);
        if (coneHits.Length > pixelsPerSpray)
        {
            foreach (var hit in coneHits)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }

            for (int i = 0; i < pixelsPerSpray; i++)
            {
                var random = Random.Range(0, coneHits.Length);
                var dist = Vector3.Distance(transform.position, coneHits[random].point);
                scanMemory.Add(coneHits[random].collider.gameObject);
                if (scanMemory.Count > 1000)
                {
                    scanMemory[0].GetComponent<MeshRenderer>().SetMaterials(reset);
                    scanMemory.RemoveAt(0);
                }
                if (dist <= maxDistance+maxRadius*2)
                {
                    coneHits[random].collider.gameObject.GetComponent<Renderer>().material.color =
                        determineColor(dist);
                }
                else
                {
                    coneHits[random].collider.gameObject.GetComponent<Renderer>().material.color =
                        Color.red;
                }
            }
        }
    }

    private Color determineColor(float dist)
    {
        var val = dist / maxDistance;
        float red = 0f, blue = 0f, green = 0f;
        if (val < 0.5f)
        {
            green = Mathf.Abs(val-0.1f) / 0.5f * 255f;
            red = 255 - green;
            blue = 0f;
        }
        else if (val >= 0.5f)
        {
            blue = val * 225f;
            green = 255 - blue;
            red = 0f;
        }

        return new Color(red / 255f, green / 255f, blue / 255f);
    }
}