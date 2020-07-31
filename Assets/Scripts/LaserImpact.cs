using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserImpact : MonoBehaviour, IPoolable
{
    public float ScaleUpSpeed;
    public float FadeOutSpeed;
    public MeshRenderer PlaneMesh;

    private string ShaderOpacityProperty = "Vector1_BE1784CE";

    public void OnSpawn()
    {
        transform.localScale = Vector3.zero;
        PlaneMesh.material.SetFloat(ShaderOpacityProperty, 1);

        StartCoroutine(_Execute());
    }

    IEnumerator _Execute()
    {
        // scale up
        while (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, ScaleUpSpeed * Time.deltaTime);
            yield return null;
        }

        var opacity = 1f;
        while (opacity != 0)
        {
            opacity = Mathf.MoveTowards(opacity, 0, FadeOutSpeed * Time.deltaTime);
            PlaneMesh.material.SetFloat(ShaderOpacityProperty, opacity);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
