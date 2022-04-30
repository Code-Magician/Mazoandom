using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
    enum Is
    {
        Ragdoll, Prefab
    }
    [SerializeField] Is characterIs;
    [SerializeField] float maxSinkHeight = 2f;
    float sinkDelay = 5f;
    float sinkDistance = 0.0f;

    bool isFirstInvoke = true;


    private void Start()
    {
        sinkDistance = 0;
        sinkDelay = Random.Range(5f, 11f);

        if (characterIs == Is.Ragdoll)
            InvokeRepeating("SinkSelf", sinkDelay, 0.025f);
    }


    public void StartSink()
    {
        InvokeRepeating("SinkSelf", sinkDelay, 0.025f);
    }

    public void SinkSelf()
    {
        if (sinkDistance >= maxSinkHeight)
        {
            Destroy(gameObject);
            return;
        }

        if (isFirstInvoke)
        {
            isFirstInvoke = false;

            Collider xt = GetComponent<Collider>();
            if (xt != null)
                Destroy(xt);

            Collider[] cols = gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider x in cols)
                Destroy(x);

            Rigidbody[] rigids = gameObject.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody x in rigids)
            {
                x.useGravity = false;
            }
        }

        transform.Translate(new Vector3(0, -0.01f, 0));
        sinkDistance += 0.01f;
    }
}
