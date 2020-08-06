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

    [Header("Visual")]
    public TextMesh GrappleStatus;

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
            grappleHolder.localRotation = Quaternion.Lerp(grappleHolder.localRotation, Quaternion.Euler(0, 0, 0), rotationSmooth * Time.fixedDeltaTime);
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


        //var divided = raycastRadius / 2f;
        //var possible = new List<RaycastHit>(raycastCount * raycastCount);
        
        //bool hasHitUngrappable = false;

        //for (var x = 0; x < raycastCount; x++)
        //{
        //    for (var y = 0; y < raycastCount; y++)
        //    {
        //        var pos = new Vector2(
        //            Mathf.Lerp(-divided, divided, x / (float)(raycastCount - 1)),
        //            Mathf.Lerp(-divided, divided, y / (float)(raycastCount - 1))
        //        );

        //        if (!Physics.Raycast(cam.position + cam.right * pos.x + cam.up * pos.y, cam.forward, out var hitInfo, maxDistance + 20)) continue;

        //        var distance = Vector3.Distance(cam.position, hitInfo.point);
        //        if (whatToGrapple.value != (whatToGrapple.value | (1 << hitInfo.transform.gameObject.layer)))
        //        {
        //            hasHitUngrappable = true;
        //            continue;
        //        }
        //        if (distance < minDistance) continue;
        //        if (distance > maxDistance)
        //        {
        //            Debug.Log($"Too far. ({distance} m)");
        //            continue;
        //        }

        //        possible.Add(hitInfo);
        //    }
        //}

        //var arr = possible.ToArray();

        //if (arr.Length > 0)
        //{
        //    var closest = new RaycastHit();
        //    var distance = 0f;
        //    var set = false;

        //    foreach (var hitInfo in arr)
        //    {
        //        var hitDistance = DistanceFromCenter(hitInfo.point);

        //        if (!set)
        //        {
        //            set = true;
        //            distance = hitDistance;
        //            closest = hitInfo;
        //        }
        //        else if (hitDistance < distance)
        //        {
        //            distance = hitDistance;
        //            closest = hitInfo;
        //        }
        //    }

        //    hit = closest;
        //    return true;
        //}
        //else if (hasHitUngrappable)
        //{
        //    Debug.Log("TODO Can't grapple to this. Do some visual FX.");
        //}

        //hit = new RaycastHit();
        //return false;
    }

    private float DistanceFromCenter(Vector3 point)
    {
        return Vector2.Distance(player.playerCamera.WorldToViewportPoint(point),
            new Vector2(0.5f, 0.5f));
    }
}