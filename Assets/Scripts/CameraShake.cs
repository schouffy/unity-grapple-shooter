using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Explosion")]
    public float FreezeTime;
    public float MaxDistanceFromExplosion;
    public AnimationCurve DistanceFallOf;
    public float ExplosionShakeMagnitude;
    public float ExplosionShakeRoughness;
    public float ExplosionShakeFadeIn;
    public float ExplosionShakeFadeOut;

    [Header("Shoot")]
    public float ShootShakeMagnitude;
    public float ShootShakeRoughness;
    public float ShootShakeFadeIn;
    public float ShootShakeFadeOut;

    [Header("Grapple")]
    public float GrappleShakeMagnitude;
    public float GrappleShakeRoughness;
    public float GrappleShakeFadeIn;
    public float GrappleShakeFadeOut;

    void Awake()
    {
        EventManager.StartListening(EventType.ExplosionNearby, p => ShakeFromExplosion((ExplosionNearbyEventParam)p));
    }

    void ShakeFromExplosion(ExplosionNearbyEventParam explosionInfo)
    {

        if (Vector3.Distance(explosionInfo.Position, transform.position) < MaxDistanceFromExplosion)
        {
            if (FreezeTime > 0)
                StartCoroutine(Freeze());
            var ratio = DistanceFallOf.Evaluate(Vector3.Distance(transform.position, explosionInfo.Position) / MaxDistanceFromExplosion);
            Shake(ExplosionShakeMagnitude * ratio, ExplosionShakeRoughness * ratio, ExplosionShakeFadeIn * ratio, ExplosionShakeFadeOut * ratio);
        }
    }

    IEnumerator Freeze()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(FreezeTime);
        Time.timeScale = 1f;
    }

    public void ShakeFromGrappleThrown()
    {
        Shake(GrappleShakeMagnitude, GrappleShakeRoughness, GrappleShakeFadeIn, GrappleShakeFadeOut);
    }

    public void ShakeFromShooting()
    {
        Shake(ShootShakeMagnitude, ShootShakeRoughness, ShootShakeFadeIn, ShootShakeFadeOut);
    }

    void Shake(float magnitude, float roughness, float fadeIn, float fadeOut)
    {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
    }
}
