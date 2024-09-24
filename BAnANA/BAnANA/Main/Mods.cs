using BAnANA.Utility;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using HarmonyLib;
using GorillaLocomotion;

namespace BAnANA
{
    public class Mods : MonoBehaviour
    {
        public static GameObject leftaimer = null;
        public static GameObject rightaimer = null;
        public static bool didSqueeze2 = false;
        public static bool didSqueeze3 = false;
        public static float speedtopull = 2.5f;
        public static float pullspeed = 5f;
        public static bool cangrapple = true;
        public static bool canleftgrapple = true;
        public static bool wackstart = false;
        public static bool start = true;
        public static bool inAllowedRoom = false;
        public static float maxDistance = 100f;
        public static float Spring;
        public static float Damper;
        public static float maxSpeedMultiplier = 0.5f;
        public static float MassScale;
        public static Vector3 grapplePoint;
        public static Vector3 leftgrapplePoint;
        public static SpringJoint joint;
        public static SpringJoint leftjoint;
        public static LineRenderer lr;
        public static LineRenderer leftlr;
        public static Color grapplecolor;
        public static float sp = 3500f;
        public static float dp = 3000f;
        public static float ms = 5f;
        public static Color rc = Color.white;

        public static void IronMonke()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().AddForce(new Vector3(20 * GorillaLocomotion.Player.Instance.leftControllerTransform.right.x * -1f, 20 * GorillaLocomotion.Player.Instance.leftControllerTransform.right.y * -1f, 20 * GorillaLocomotion.Player.Instance.leftControllerTransform.right.z * -1f), ForceMode.Acceleration);
            }

            if (ControllerInputPoller.instance.rightGrab || Mouse.current.rightButton.isPressed)
            {
                GorillaTagger.Instance.StartVibration(forLeftController: false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().AddForce(new Vector3(20 * GorillaLocomotion.Player.Instance.rightControllerTransform.right.x, 20 * GorillaLocomotion.Player.Instance.rightControllerTransform.right.y, 20 * GorillaLocomotion.Player.Instance.rightControllerTransform.right.z), ForceMode.Acceleration);
            }
        }

        // community made mods below //

        public static void Webshooters()
        {
            Vector3 localControllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Vector3 localControllerVelocity2 = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
            RaycastHit raycastHit;
            bool flag = Physics.Raycast(Player.Instance.rightControllerTransform.position, Player.Instance.rightControllerTransform.forward, out raycastHit, maxDistance) && rightaimer == null;
            if (flag)
            {
                rightaimer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rightaimer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                Object.Destroy(rightaimer.GetComponent<Rigidbody>());
                Object.Destroy(rightaimer.GetComponent<SphereCollider>());
                rightaimer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            bool flag2 = ControllerInputPoller.instance.rightControllerIndexFloat == 0f;
            if (flag2)
            {
                rightaimer.GetComponent<MeshRenderer>().material.color = new Color32(0, byte.MaxValue, 0, 50);
                rightaimer.transform.position = raycastHit.point;
            }
            else
            {
                rightaimer.GetComponent<MeshRenderer>().material.color = Color.clear;
            }
            bool flag3 = (double)ControllerInputPoller.instance.rightControllerIndexFloat > 0.1 && !didSqueeze3;
            if (flag3)
            {
                didSqueeze3 = true;
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(82, false, 1f);
            }
            bool flag4 = ControllerInputPoller.instance.rightControllerIndexFloat == 0f && didSqueeze3;
            if (flag4)
            {
                didSqueeze3 = false;
            }
            RaycastHit raycastHit2;
            bool flag5 = Physics.Raycast(Player.Instance.leftControllerTransform.position, Player.Instance.leftControllerTransform.forward, out raycastHit2, maxDistance) && leftaimer == null;
            if (flag5)
            {
                leftaimer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                leftaimer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                Object.Destroy(leftaimer.GetComponent<Rigidbody>());
                Object.Destroy(leftaimer.GetComponent<SphereCollider>());
                leftaimer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            bool flag6 = ControllerInputPoller.instance.leftControllerIndexFloat == 0f;
            if (flag6)
            {
                leftaimer.GetComponent<MeshRenderer>().material.color = new Color32(0, byte.MaxValue, 0, 50);
                leftaimer.transform.position = raycastHit2.point;
            }
            else
            {
                leftaimer.GetComponent<MeshRenderer>().material.color = Color.clear;
            }
            bool flag7 = (double)ControllerInputPoller.instance.leftControllerIndexFloat > 0.1 && !didSqueeze2;
            if (flag7)
            {
                didSqueeze2 = true;
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(82, true, 1f);
            }
            bool flag8 = ControllerInputPoller.instance.leftControllerIndexFloat == 0f && didSqueeze2;
            if (flag8)
            {
                didSqueeze2 = false;
            }
            bool flag9 = !wackstart;
            if (flag9)
            {
                GameObject gameObject = new GameObject();
                Spring = sp;
                Damper = dp;
                MassScale = ms;
                grapplecolor = rc;
                lr = Player.Instance.gameObject.AddComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = Color.white;
                lr.endColor = Color.white;
                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
                leftlr = gameObject.AddComponent<LineRenderer>();
                leftlr.material = new Material(Shader.Find("Sprites/Default"));
                leftlr.startColor = Color.white;
                leftlr.endColor = Color.white;
                leftlr.startWidth = 0.05f;
                leftlr.endWidth = 0.05f;
                wackstart = true;
            }
            DrawRope(Player.Instance);
            LeftDrawRope(Player.Instance);
            bool flag10 = ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f;
            if (flag10)
            {
                bool flag11 = cangrapple;
                if (flag11)
                {
                    Spring = sp;
                    StartGrapple(Player.Instance);
                    cangrapple = false;
                }
                bool flag12 = localControllerVelocity.magnitude >= speedtopull;
                if (flag12)
                {
                    Player.Instance.GetComponent<Rigidbody>().velocity += (grapplePoint - Player.Instance.transform.position).normalized * pullspeed;
                }
            }
            else
            {
                StopGrapple(Player.Instance);
            }
            bool flag13 = ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f && ControllerInputPoller.instance.leftControllerIndexFloat > 0.1f;
            if (flag13)
            {
                Spring /= 2f;
            }
            else
            {
                Spring = sp;
            }
            bool flag14 = ControllerInputPoller.instance.leftControllerIndexFloat > 0.1f;
            if (flag14)
            {
                bool flag15 = canleftgrapple;
                if (flag15)
                {
                    Spring = sp;
                    LeftStartGrapple(Player.Instance);
                    canleftgrapple = false;
                }
                bool flag16 = localControllerVelocity2.magnitude >= speedtopull;
                if (flag16)
                {
                    Player.Instance.GetComponent<Rigidbody>().velocity += (leftgrapplePoint - Player.Instance.transform.position).normalized * pullspeed;
                }
            }
            else
            {
                LeftStopGrapple();
            }
        }
        public static void DestroyAimers()
        {
            Object.Destroy(rightaimer);
            Object.Destroy(leftaimer);
        }
        public static void StartGrapple(global::GorillaLocomotion.Player __instance)
        {
            RaycastHit raycastHit;
            bool flag = Physics.Raycast(__instance.rightControllerTransform.position, __instance.rightControllerTransform.forward, out raycastHit, maxDistance);
            if (flag)
            {
                grapplePoint = raycastHit.point;
                joint = __instance.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;
                float num = Vector3.Distance(__instance.bodyCollider.attachedRigidbody.position, grapplePoint);
                joint.maxDistance = num * 0.8f;
                joint.minDistance = num * 0.25f;
                joint.spring = Spring;
                joint.damper = Damper;
                joint.massScale = MassScale;
                lr.positionCount = 2;
            }
        }
        public static void DrawRope(global::GorillaLocomotion.Player __instance)
        {
            bool flag = joint;
            if (flag)
            {
                lr.SetPosition(0, __instance.rightControllerTransform.position);
                lr.SetPosition(1, grapplePoint);
            }
        }
        public static void StopGrapple(global::GorillaLocomotion.Player __instance)
        {
            lr.positionCount = 0;
            Object.Destroy(joint);
            cangrapple = true;
        }
        public static void LeftStartGrapple(global::GorillaLocomotion.Player __instance)
        {
            RaycastHit raycastHit;
            bool flag = Physics.Raycast(__instance.leftControllerTransform.position, __instance.leftControllerTransform.forward, out raycastHit, maxDistance);
            if (flag)
            {
                leftgrapplePoint = raycastHit.point;
                leftjoint = __instance.gameObject.AddComponent<SpringJoint>();
                leftjoint.autoConfigureConnectedAnchor = false;
                leftjoint.connectedAnchor = leftgrapplePoint;
                float num = Vector3.Distance(__instance.bodyCollider.attachedRigidbody.position, leftgrapplePoint);
                leftjoint.maxDistance = num * 0.8f;
                leftjoint.minDistance = num * 0.25f;
                leftjoint.spring = Spring;
                leftjoint.damper = Damper;
                leftjoint.massScale = MassScale;
                leftlr.positionCount = 2;
            }
        }
        public static void LeftDrawRope(global::GorillaLocomotion.Player __instance)
        {
            bool flag = leftjoint;
            if (flag)
            {
                leftlr.SetPosition(0, __instance.leftControllerTransform.position);
                leftlr.SetPosition(1, leftgrapplePoint);
            }
        }
        public static void LeftStopGrapple()
        {
            leftlr.positionCount = 0;
            Object.Destroy(leftjoint);
            canleftgrapple = true;
        }
    }
}
