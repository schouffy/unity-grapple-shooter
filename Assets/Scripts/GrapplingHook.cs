using System;
using System.Collections.Generic;
using Player;
using Rope;
using Unity.Collections;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("Grappling")]
    public GrapplingRope grapplingRope;
    public PlayerController player;
    public Transform grappleTip;
    public Transform grappleHolder;
    public LayerMask whatToGrapple;
    public float maxDistance;
    public float minDistance;
    public float rotationSmooth;
    private Quaternion _initialHolderRotation;

    [Header("Visual")]
    public TextMesh GrappleStatus;
    private float _initialCameraFov;
    public float FovTargetWhenGrappling;
    public float MinSpeedBeforeFovIncrease;
    public float FovIncreaseSpeed;
    public float FovDecreaseSpeed;
    public ParticleSystem SpeedEffect;
    private Color _fxOpaque;
    private Color _fxTransparent;


    [Header("Raycasts")]
    public float RaycastMaxRadius;

    [Header("Physics")]
    public float pullForce;
    public float pushForce;
    public float yMultiplier;
    public float minPhysicsDistance;
    public float maxPhysicsDistance;

    private Vector3 _hit;

    public bool Grappling => grapplingRope.Grappling;

    private void Start()
    {
        _initialHolderRotation = grappleHolder.localRotation;
        _initialCameraFov = Camera.main.fieldOfView;
        var fxMain = SpeedEffect.main.startColor;
        _fxOpaque = fxMain.color;
        _fxOpaque.a = 1;
        _fxTransparent = fxMain.color;
        _fxTransparent.a = 0;
    }

    private void FixedUpdate()
    {
        if (Input.GetButton("Grapple") && grapplingRope.Grappling)
        {
            grappleHolder.rotation = Quaternion.Lerp(grappleHolder.rotation, Quaternion.LookRotation(-(grappleHolder.position - _hit)), rotationSmooth * Time.fixedDeltaTime);

            var distance = Vector3.Distance(player.transform.position, _hit);
            if (!(distance >= minPhysicsDistance) || !(distance <= maxPhysicsDistance)) return;
            player.playerRigidBody.velocity += pullForce * Time.fixedDeltaTime * yMultiplier * Mathf.Abs(_hit.y - player.transform.position.y) * (_hit - player.transform.position).normalized;
            player.playerRigidBody.velocity += pushForce * Time.fixedDeltaTime * player.transform.forward;
        }
        else
        {
            grappleHolder.localRotation = Quaternion.Lerp(grappleHolder.localRotation, _initialHolderRotation, rotationSmooth * Time.fixedDeltaTime);
        }
    }

    private void LateUpdate()
    {
        bool canGrapple = RaycastAll(out var hitInfo, out bool surfaceNotGrappable);
        GrappleStatus.text = canGrapple ? "YES" : "NO";

        if (Input.GetButtonDown("Grapple") && canGrapple)
        {
            grapplingRope.Grapple(grappleTip.position, hitInfo.point);
            _hit = hitInfo.point;
        }
        else if (Input.GetButtonDown("Grapple") && surfaceNotGrappable)
        {
            grapplingRope.GrappleAndFail(grappleTip.position, hitInfo.point);
        }

        if (Input.GetButtonUp("Grapple"))
        {
            grapplingRope.UnGrapple();
        }

        if (Input.GetButton("Grapple") && grapplingRope.Grappling)
        {
            grapplingRope.UpdateStart(grappleTip.position);
        }

        grapplingRope.UpdateGrapple();
        ShowSpeedFx();
    }



    void ShowSpeedFx()
    {
        // Check rigidbody velocity on camera forward axis
        var speed = Camera.main.transform.InverseTransformDirection(player.playerRigidBody.velocity);
        if (speed.z > MinSpeedBeforeFovIncrease)
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, FovTargetWhenGrappling, FovIncreaseSpeed * speed.z * Time.deltaTime);
        else
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _initialCameraFov, FovDecreaseSpeed * Time.deltaTime);

        var fovNormalized = (Camera.main.fieldOfView - _initialCameraFov) / (FovTargetWhenGrappling - _initialCameraFov);

        var main = SpeedEffect.main;
        main.startColor = new ParticleSystem.MinMaxGradient(Color.Lerp(_fxTransparent, _fxOpaque, fovNormalized));
    }

    private bool RaycastAll(out RaycastHit hit, out bool surfaceNotGrappable)
    {
        bool hasHitUngrappable = false;
        var cam = player.playerCamera.transform;
        surfaceNotGrappable = false;
        RaycastHit notGrappableHitInfo = new RaycastHit();

        bool ExamineCast(RaycastHit cast)
        {
            var distance = Vector3.Distance(cam.position, cast.point);
            var isUngrappable = whatToGrapple.value != (whatToGrapple.value | (1 << cast.transform.gameObject.layer));

            if (distance < minDistance || distance > maxDistance)
                return false;
            else if (isUngrappable)
            {
                notGrappableHitInfo = cast;
                hasHitUngrappable = true;
                return false;
            }
            else
                return true;
        }

        if (Physics.Raycast(cam.position, cam.forward * (maxDistance + 100), out var hitInfo))
        {
            if (ExamineCast(hitInfo))
            {
                hit = hitInfo;
                return true;
            }
        }

        for (float radius = 0.1f; radius < RaycastMaxRadius; radius += 0.1f)
        {
            if (Physics.SphereCast(cam.position, radius, cam.forward * (maxDistance + 100), out hitInfo))
            {
                if (ExamineCast(hitInfo))
                {
                    hit = hitInfo;
                    return true;
                }
            }
        }

        hit = new RaycastHit();
        if (hasHitUngrappable)
        {
            hit = notGrappableHitInfo;
            surfaceNotGrappable = true;
        }

        return false;
    }

    private float DistanceFromCenter(Vector3 point)
    {
        return Vector2.Distance(player.playerCamera.WorldToViewportPoint(point),
            new Vector2(0.5f, 0.5f));
    }
}