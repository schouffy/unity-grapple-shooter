using System;
using System.Collections.Generic;
using System.Diagnostics;
using Player;
using Rope;
using Unity.Collections;
using UnityEngine;

namespace Carriable.Carriables
{
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

        [Header("Raycasts")]
        public float raycastRadius;
        public int raycastCount;

        [Header("Physics")]
        public float pullForce;
        public float pushForce;
        public float yMultiplier;
        public float minPhysicsDistance;
        public float maxPhysicsDistance;

        private Vector3 _hit;

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
            if (Input.GetButtonDown("Grapple") && RaycastAll(out var hitInfo))
            {
                grapplingRope.Grapple(grappleTip.position, hitInfo.point);
                _hit = hitInfo.point;
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

        private bool RaycastAll(out RaycastHit hit)
        {
            var divided = raycastRadius / 2f;
            var possible = new List<RaycastHit>(raycastCount * raycastCount);
            var cam = player.playerCamera.transform;

            for (var x = 0; x < raycastCount; x++)
            {
                for (var y = 0; y < raycastCount; y++)
                {
                    var pos = new Vector2(
                        Mathf.Lerp(-divided, divided, x / (float)(raycastCount - 1)),
                        Mathf.Lerp(-divided, divided, y / (float)(raycastCount - 1))
                    );

                    if (!Physics.Raycast(cam.position + cam.right * pos.x + cam.up * pos.y, cam.forward, out var hitInfo, maxDistance)) continue;

                    var distance = Vector3.Distance(cam.position, hitInfo.point);
                    if (whatToGrapple.value != (whatToGrapple.value | (1 << hitInfo.transform.gameObject.layer))) continue;
                    if (distance < minDistance) continue;
                    if (distance > maxDistance) continue;

                    possible.Add(hitInfo);
                }
            }

            var arr = possible.ToArray();

            if (arr.Length > 0)
            {
                var closest = new RaycastHit();
                var distance = 0f;
                var set = false;

                foreach (var hitInfo in arr)
                {
                    var hitDistance = DistanceFromCenter(hitInfo.point);

                    if (!set)
                    {
                        set = true;
                        distance = hitDistance;
                        closest = hitInfo;
                    }
                    else if (hitDistance < distance)
                    {
                        distance = hitDistance;
                        closest = hitInfo;
                    }
                }

                hit = closest;
                return true;
            }

            hit = new RaycastHit();
            return false;
        }

        private float DistanceFromCenter(Vector3 point)
        {
            return Vector2.Distance(player.playerCamera.WorldToViewportPoint(point),
                new Vector2(0.5f, 0.5f));
        }
    }
}