﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PositionIndex : int
{
    rShldrBend = 0,
    rForearmBend,
    rHand,
    rThumb2,
    rMid1,

    lShldrBend,
    lForearmBend,
    lHand,
    lThumb2,
    lMid1,

    lEar,
    lEye,
    rEar,
    rEye,
    Nose,

    rThighBend,
    rShin,
    rFoot,
    rToe,

    lThighBend,
    lShin,
    lFoot,
    lToe,

    abdomenUpper,

    //Calculated coordinates
    hip,
    head,
    neck,
    spine,

    Count,
    None,
}

public static partial class EnumExtend
{
    public static int Int(this PositionIndex i)
    {
        return (int)i;
    }
}

public class VNectModel : MonoBehaviour
{

    public class JointPoint
    {
        public Vector2 Pos2D = new Vector2();
        public float score2D;

        public Vector3 Pos3D = new Vector3();
        public Vector3 Now3D = new Vector3();
        public Vector3[] PrevPos3D = new Vector3[6];
        public float score3D;

        // Bones
        public Transform Transform = null;
        public Quaternion InitRotation;
        public Quaternion Inverse;
        public Quaternion InverseRotation;

        public JointPoint Child = null;

        //public static float Q = 0.0001f;
        //public static float R = 0.0002f;
        public static float Q = 0.001f;
        public static float R = 0.0015f;
        public Vector3 P = new Vector3();
        public Vector3 X = new Vector3();
        public Vector3 K = new Vector3();
    }

    public class Skeleton
    {
        public GameObject LineObject;
        public LineRenderer Line;

        public JointPoint start = null;
        public JointPoint end = null;
    }

    private List<Skeleton> Skeletons = new List<Skeleton>();
    public Material SkeletonMaterial;

    public float SkeletonX;
    public float SkeletonY;
    public float SkeletonZ;
    public float SkeletonScale;

    // Joint position and bone
    private JointPoint[] jointPoints;
    public JointPoint[] JointPoints { get { return jointPoints; } }

    private Vector3 initPosition; // Initial center position

    private Quaternion InitGazeRotation;
    private Quaternion gazeInverse;

    // UnityChan
    public GameObject ModelObject;
    public GameObject Nose;
    private Animator anim;

    private float movementScale = 0.01f;
    private float centerTall = 224 * 0.75f;
    private float tall = 224 * 0.75f;
    private float prevTall = 224 * 0.75f;
    public float ZScale = 0.8f;

    public JointPoint[] Init(int inputImageSize)
    {
        movementScale = 0.01f * 224f / inputImageSize;
        centerTall = inputImageSize * 0.75f;
        tall = inputImageSize * 0.75f;
        prevTall = inputImageSize * 0.75f;

        jointPoints = new JointPoint[PositionIndex.Count.Int()];
        for (var i = 0; i < PositionIndex.Count.Int(); i++) jointPoints[i] = new JointPoint();

        anim = ModelObject.GetComponent<Animator>();
        jointPoints[PositionIndex.hip.Int()].Transform = transform;

        // Right Arm
        jointPoints[PositionIndex.rShldrBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        jointPoints[PositionIndex.rForearmBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        jointPoints[PositionIndex.rHand.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightHand);
        jointPoints[PositionIndex.rThumb2.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        jointPoints[PositionIndex.rMid1.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        // Left Arm
        jointPoints[PositionIndex.lShldrBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        jointPoints[PositionIndex.lForearmBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        jointPoints[PositionIndex.lHand.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        jointPoints[PositionIndex.lThumb2.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        jointPoints[PositionIndex.lMid1.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        // Face
        jointPoints[PositionIndex.lEar.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.lEye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftEye);
        jointPoints[PositionIndex.rEar.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.rEye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightEye);
        jointPoints[PositionIndex.Nose.Int()].Transform = Nose.transform;

        // Right Leg
        jointPoints[PositionIndex.rThighBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        jointPoints[PositionIndex.rShin.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        jointPoints[PositionIndex.rFoot.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        jointPoints[PositionIndex.rToe.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightToes);

        // Left Leg
        jointPoints[PositionIndex.lThighBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        jointPoints[PositionIndex.lShin.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        jointPoints[PositionIndex.lFoot.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        jointPoints[PositionIndex.lToe.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftToes);

        // etc
        jointPoints[PositionIndex.abdomenUpper.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);
        jointPoints[PositionIndex.head.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.hip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Hips);
        jointPoints[PositionIndex.neck.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Neck);
        jointPoints[PositionIndex.spine.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);

        // Child Settings
        // Right Arm
        jointPoints[PositionIndex.rShldrBend.Int()].Child = jointPoints[PositionIndex.rForearmBend.Int()];
        jointPoints[PositionIndex.rForearmBend.Int()].Child = jointPoints[PositionIndex.rHand.Int()];

        // Left Arm
        jointPoints[PositionIndex.lShldrBend.Int()].Child = jointPoints[PositionIndex.lForearmBend.Int()];
        jointPoints[PositionIndex.lForearmBend.Int()].Child = jointPoints[PositionIndex.lHand.Int()];

        // Fase

        // Right Leg
        jointPoints[PositionIndex.rThighBend.Int()].Child = jointPoints[PositionIndex.rShin.Int()];
        jointPoints[PositionIndex.rShin.Int()].Child = jointPoints[PositionIndex.rFoot.Int()];
        jointPoints[PositionIndex.rFoot.Int()].Child = jointPoints[PositionIndex.rToe.Int()];

        // Left Leg
        jointPoints[PositionIndex.lThighBend.Int()].Child = jointPoints[PositionIndex.lShin.Int()];
        jointPoints[PositionIndex.lShin.Int()].Child = jointPoints[PositionIndex.lFoot.Int()];
        jointPoints[PositionIndex.lFoot.Int()].Child = jointPoints[PositionIndex.lToe.Int()];

        // etc
        jointPoints[PositionIndex.spine.Int()].Child = jointPoints[PositionIndex.neck.Int()];
        jointPoints[PositionIndex.neck.Int()].Child = jointPoints[PositionIndex.head.Int()];
        //jointPoints[PositionIndex.head.Int()].Child = jointPoints[PositionIndex.Nose.Int()];
        //jointPoints[PositionIndex.hip.Int()].Child = jointPoints[PositionIndex.spine.Int()];

        // Line Child Settings
        // Right Arm
        AddSkeleton(PositionIndex.rShldrBend, PositionIndex.rForearmBend);
        AddSkeleton(PositionIndex.rForearmBend, PositionIndex.rHand);
        AddSkeleton(PositionIndex.rHand, PositionIndex.rThumb2);
        AddSkeleton(PositionIndex.rHand, PositionIndex.rMid1);

        // Left Arm
        AddSkeleton(PositionIndex.lShldrBend, PositionIndex.lForearmBend);
        AddSkeleton(PositionIndex.lForearmBend, PositionIndex.lHand);
        AddSkeleton(PositionIndex.lHand, PositionIndex.lThumb2);
        AddSkeleton(PositionIndex.lHand, PositionIndex.lMid1);

        // Face
        //AddSkeleton(PositionIndex.lEar, PositionIndex.lEye);
        //AddSkeleton(PositionIndex.lEye, PositionIndex.Nose);
        //AddSkeleton(PositionIndex.rEar, PositionIndex.rEye);
        //AddSkeleton(PositionIndex.rEye, PositionIndex.Nose);
        AddSkeleton(PositionIndex.lEar, PositionIndex.Nose);
        AddSkeleton(PositionIndex.rEar, PositionIndex.Nose);

        // Right Leg
        AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShin);
        AddSkeleton(PositionIndex.rShin, PositionIndex.rFoot);
        AddSkeleton(PositionIndex.rFoot, PositionIndex.rToe);

        // Left Leg
        AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShin);
        AddSkeleton(PositionIndex.lShin, PositionIndex.lFoot);
        AddSkeleton(PositionIndex.lFoot, PositionIndex.lToe);

        // etc
        AddSkeleton(PositionIndex.spine, PositionIndex.neck);
        AddSkeleton(PositionIndex.neck, PositionIndex.head);
        AddSkeleton(PositionIndex.head, PositionIndex.Nose);
        AddSkeleton(PositionIndex.neck, PositionIndex.rShldrBend);
        AddSkeleton(PositionIndex.neck, PositionIndex.lShldrBend);
        AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShldrBend);
        AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShldrBend);
        AddSkeleton(PositionIndex.rShldrBend, PositionIndex.abdomenUpper);
        AddSkeleton(PositionIndex.lShldrBend, PositionIndex.abdomenUpper);
        AddSkeleton(PositionIndex.rThighBend, PositionIndex.abdomenUpper);
        AddSkeleton(PositionIndex.lThighBend, PositionIndex.abdomenUpper);
        AddSkeleton(PositionIndex.lThighBend, PositionIndex.rThighBend);

        // Set Inverse
        var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Transform.position, jointPoints[PositionIndex.lThighBend.Int()].Transform.position, jointPoints[PositionIndex.rThighBend.Int()].Transform.position);
        foreach (var jointPoint in jointPoints)
        {
            if (jointPoint.Transform != null)
            {
                jointPoint.InitRotation = jointPoint.Transform.rotation;
            }

            if (jointPoint.Child != null)
            {
                jointPoint.Inverse = GetInverse(jointPoint, jointPoint.Child, forward);
                jointPoint.InverseRotation = jointPoint.Inverse * jointPoint.InitRotation;
            }
        }
        var hip = jointPoints[PositionIndex.hip.Int()];
        initPosition = transform.position;
        //initPosition = jointPoints[PositionIndex.hip.Int()].Transform.position;
        hip.Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
        hip.InverseRotation = hip.Inverse * hip.InitRotation;

        // For Head Rotation
        var head = jointPoints[PositionIndex.head.Int()];
        head.InitRotation = jointPoints[PositionIndex.head.Int()].Transform.rotation;
        var gaze = jointPoints[PositionIndex.Nose.Int()].Transform.position - jointPoints[PositionIndex.head.Int()].Transform.position;
        head.Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));
        head.InverseRotation = head.Inverse * head.InitRotation;

        var lHand = jointPoints[PositionIndex.lHand.Int()];
        var lf = TriangleNormal(lHand.Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
        lHand.InitRotation = lHand.Transform.rotation;
        lHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Transform.position - jointPoints[PositionIndex.lMid1.Int()].Transform.position, lf));
        lHand.InverseRotation = lHand.Inverse * lHand.InitRotation;

        var rHand = jointPoints[PositionIndex.rHand.Int()];
        var rf = TriangleNormal(rHand.Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
        rHand.InitRotation = jointPoints[PositionIndex.rHand.Int()].Transform.rotation;
        rHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Transform.position - jointPoints[PositionIndex.rMid1.Int()].Transform.position, rf));
        rHand.InverseRotation = rHand.Inverse * rHand.InitRotation;

        jointPoints[PositionIndex.hip.Int()].score3D = 1f;
        jointPoints[PositionIndex.neck.Int()].score3D = 1f;
        jointPoints[PositionIndex.Nose.Int()].score3D = 1f;
        jointPoints[PositionIndex.head.Int()].score3D = 1f;
        jointPoints[PositionIndex.spine.Int()].score3D = 1f;

        return JointPoints;
    }

    public void SetNose(float x, float y, float z)
    {
        if (this.Nose == null)
        {
            this.Nose = new GameObject(this.name + "_Nose");
        }
        var ani = ModelObject.GetComponent<Animator>();
        var t = ani.GetBoneTransform(HumanBodyBones.Head);
        this.Nose.transform.position = new Vector3(t.position.x + x, t.position.y + y, t.position.z + z);

    }

    public void SetSettings(AvatarSetting setting)
    {
        this.name = setting.AvatarName;

        ResetPosition(setting.PosX, setting.PosY, setting.PosZ);
        SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
        SetScale(setting.Scale);
        SetZScale(setting.DepthScale);

        SetSkeleton(setting.SkeletonVisible == 1);

        SkeletonX = setting.SkeletonPosX;
        SkeletonY = setting.SkeletonPosY;
        SkeletonZ = setting.SkeletonPosZ;
        SkeletonScale = setting.SkeletonScale;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetZScale(float zScale)
    {
        ZScale = zScale;
    }

    public void SetSkeleton(bool flag)
    {
        foreach (var sk in Skeletons)
        {
            sk.LineObject.SetActive(flag);
        }
    }

    public void ResetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
        initPosition = transform.position;
    }

    public Vector3 GetHeadPosition()
    {
        return anim.GetBoneTransform(HumanBodyBones.Head).position;
    }

    public void Show()
    {
    }

    public void Hide()
    {
        SetSkeleton(false);
    }


    public void PoseUpdate()
    {
        // 身長からｚの移動量を算出
        var t1 = Vector3.Distance(jointPoints[PositionIndex.head.Int()].Pos3D, jointPoints[PositionIndex.neck.Int()].Pos3D);
        var t2 = Vector3.Distance(jointPoints[PositionIndex.neck.Int()].Pos3D, jointPoints[PositionIndex.spine.Int()].Pos3D);
        var pm = (jointPoints[PositionIndex.rThighBend.Int()].Pos3D + jointPoints[PositionIndex.lThighBend.Int()].Pos3D) / 2f;
        var t3 = Vector3.Distance(jointPoints[PositionIndex.spine.Int()].Pos3D, pm);
        var t4r = Vector3.Distance(jointPoints[PositionIndex.rThighBend.Int()].Pos3D, jointPoints[PositionIndex.rShin.Int()].Pos3D);
        var t4l = Vector3.Distance(jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.lShin.Int()].Pos3D);
        var t4 = (t4r + t4l) / 2f;
        var t5r = Vector3.Distance(jointPoints[PositionIndex.rShin.Int()].Pos3D, jointPoints[PositionIndex.rFoot.Int()].Pos3D);
        var t5l = Vector3.Distance(jointPoints[PositionIndex.lShin.Int()].Pos3D, jointPoints[PositionIndex.lFoot.Int()].Pos3D);
        var t5 = (t5r + t5l) / 2f;
        var t = t1 + t2 + t3 + t4 + t5;

        tall = t * 0.7f + prevTall * 0.3f;
        prevTall = tall;

        if (tall == 0)
        {
            t = centerTall;
        }
        var dz = (centerTall - tall) / centerTall * ZScale;

        // センターの移動と回転
        var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Pos3D, jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.rThighBend.Int()].Pos3D);
        jointPoints[PositionIndex.hip.Int()].Transform.position = jointPoints[PositionIndex.hip.Int()].Pos3D * movementScale + new Vector3(initPosition.x, initPosition.y, initPosition.z + dz);
        jointPoints[PositionIndex.hip.Int()].Transform.rotation = Quaternion.LookRotation(forward) * jointPoints[PositionIndex.hip.Int()].InverseRotation;

        // 各ボーンの回転
        foreach (var jointPoint in jointPoints)
        {
            if (jointPoint.Child != null)
            {
                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, forward) * jointPoint.InverseRotation;
            }
        }

        // Head Rotation
        var gaze = jointPoints[PositionIndex.Nose.Int()].Pos3D - jointPoints[PositionIndex.head.Int()].Pos3D;
        var f = TriangleNormal(jointPoints[PositionIndex.Nose.Int()].Pos3D, jointPoints[PositionIndex.rEar.Int()].Pos3D, jointPoints[PositionIndex.lEar.Int()].Pos3D);
        var head = jointPoints[PositionIndex.head.Int()];
        head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.InverseRotation;
        //head.Transform.eulerAngles = new Vector3(0f, 180f, 0f) - head.Transform.eulerAngles;

        // Wrist rotation (Test code)
        var lHand = jointPoints[PositionIndex.lHand.Int()];
        var lf = TriangleNormal(lHand.Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
        lHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Pos3D - jointPoints[PositionIndex.lMid1.Int()].Pos3D, lf) * lHand.InverseRotation;

        var rHand = jointPoints[PositionIndex.rHand.Int()];
        var rf = TriangleNormal(rHand.Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
        rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * rHand.InverseRotation;

        foreach (var sk in Skeletons)
        {
            var s = sk.start;
            var e = sk.end;

            //            sk.Line.SetPosition(0, new Vector3(s.Transform.position.x - 1.4f, s.Transform.position.y, s.Transform.position.z - 0.1f));
            //            sk.Line.SetPosition(1, new Vector3(e.Transform.position.x - 1.4f, e.Transform.position.y, e.Transform.position.z - 0.1f));
            sk.Line.SetPosition(0, new Vector3(s.Pos3D.x * SkeletonScale + SkeletonX, s.Pos3D.y * SkeletonScale + SkeletonY, s.Pos3D.z * SkeletonScale + SkeletonZ));
            sk.Line.SetPosition(1, new Vector3(e.Pos3D.x * SkeletonScale + SkeletonX, e.Pos3D.y * SkeletonScale + SkeletonY, e.Pos3D.z * SkeletonScale + SkeletonZ));
        }
    }

    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    private Quaternion GetInverse(JointPoint p1, JointPoint p2, Vector3 forward)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position, forward));
    }

    private void AddSkeleton(PositionIndex s, PositionIndex e)
    {
        var sk = new Skeleton()
        {
            LineObject = new GameObject(this.name + "_Skeleton" +  (Skeletons.Count + 1).ToString("00")),
            start = jointPoints[s.Int()],
            end = jointPoints[e.Int()],
        };

        sk.Line = sk.LineObject.AddComponent<LineRenderer>();
        sk.Line.startWidth = 0.04f;
        sk.Line.endWidth = 0.01f;
        //頂点の数を決める
        sk.Line.positionCount = 2;
        sk.Line.material = SkeletonMaterial;

        Skeletons.Add(sk);
    }

    public static bool IsPoseUpdate = false;

    private void Update()
    {
        if (jointPoints != null)
        {
            /**/
            if (IsPoseUpdate)
            {
                PoseUpdate();
            }
            else
            {
                /*
                foreach (var jp in jointPoints)
                {
                    KalmanUpdate(jp);
                }
                PoseUpdate();
                foreach (var jp in jointPoints)
                {
                    jp.Now3D = jp.Pos3D;
                }
                */
            }
            IsPoseUpdate = false;
            /**/
        }
    }

    void KalmanUpdate(VNectModel.JointPoint measurement)
    {
        measurementUpdate(measurement);
        measurement.Pos3D.x = measurement.X.x + (measurement.Now3D.x - measurement.X.x) * measurement.K.x;
        measurement.Pos3D.y = measurement.X.y + (measurement.Now3D.y - measurement.X.y) * measurement.K.y;
        measurement.Pos3D.z = measurement.X.z + (measurement.Now3D.z - measurement.X.z) * measurement.K.z;
        measurement.X = measurement.Pos3D;
    }

    void measurementUpdate(VNectModel.JointPoint measurement)
    {
        measurement.K.x = (measurement.P.x + VNectModel.JointPoint.Q) / (measurement.P.x + VNectModel.JointPoint.Q + VNectModel.JointPoint.R);
        measurement.K.y = (measurement.P.y + VNectModel.JointPoint.Q) / (measurement.P.y + VNectModel.JointPoint.Q + VNectModel.JointPoint.R);
        measurement.K.z = (measurement.P.z + VNectModel.JointPoint.Q) / (measurement.P.z + VNectModel.JointPoint.Q + VNectModel.JointPoint.R);
        measurement.P.x = VNectModel.JointPoint.R * (measurement.P.x + VNectModel.JointPoint.Q) / (VNectModel.JointPoint.R + measurement.P.x + VNectModel.JointPoint.Q);
        measurement.P.y = VNectModel.JointPoint.R * (measurement.P.y + VNectModel.JointPoint.Q) / (VNectModel.JointPoint.R + measurement.P.y + VNectModel.JointPoint.Q);
        measurement.P.z = VNectModel.JointPoint.R * (measurement.P.z + VNectModel.JointPoint.Q) / (VNectModel.JointPoint.R + measurement.P.z + VNectModel.JointPoint.Q);
    }
}
