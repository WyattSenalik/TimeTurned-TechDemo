using UnityEngine;
// Original Authors - Wyatt Senalik

/// <summary>
/// Watches the transform of the object its attached to and stores
/// velocity for its position, rotation, and scale.
/// 
/// Rotation not yet supported for 3D, only for 2D (z rotation).
/// </summary>
[DisallowMultipleComponent]
public class VelocityMonitor : MonoBehaviour
{
    private const bool IS_DEBUGGING = false;
    private const float DEF_POS_TOLERANCE = 0.01f;
    private const float DEF_ROT_TOLERANCE = 0.1f;
    private const float DEF_SCALE_TOLERANCE = 0.01f;

    // These should never be changed during runtime.
    [SerializeField]
    private eUnityUpdateChoice m_updateTime = eUnityUpdateChoice.LateUpdate;
    [Tooltip("If the world or local position/rotation should be monitored. " +
        "The local scale is ALWAYS monitored. World scale is NEVER monitored.")]
    [SerializeField] private eRelativeSpace m_space = eRelativeSpace.World;

    // Position
    public Vector3 positionVelocity { get; private set; }
    public Vector3 prevPositionVelocity { get; private set; }
    // Scale
    public Vector3 scaleVelocity { get; private set; }
    public Vector3 prevScaleVelocity { get; private set; }
    // Rotation (Only 2D currently supported)
    public float zAngleVelocity { get; private set; }
    public float prevZAngleVelocity { get; private set; }

    private TransformData m_curTransData = null;
    private TransformData m_prevTransData = null;
    private bool m_isWorldSpace = true;


    // Domestic Initialization
    private void Awake()
    {
        m_isWorldSpace = m_space == eRelativeSpace.World;
    }
    private void Update()
    {
        if (m_updateTime != eUnityUpdateChoice.Update) { return; }

        CalculateVelocities(Time.deltaTime);
    }
    private void LateUpdate()
    {
        if (m_updateTime != eUnityUpdateChoice.LateUpdate) { return; }

        CalculateVelocities(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if (m_updateTime != eUnityUpdateChoice.FixedUpdate) { return; }

        CalculateVelocities(Time.deltaTime);
    }


    /// <summary>
    /// If any (position, rotation, scale) velocity is not the 
    /// same as it was last update.
    /// </summary>
    public bool DidAnyVelocityChange(float? tolerance = null)
    {
        return DidPositionVelocityChange(tolerance) ||
            DidRotationVelocityChange(tolerance) ||
            DidScaleVelocityChange(tolerance);
    }
    /// <summary>
    /// If position velocity is not the same as it was last update.
    /// </summary>
    public bool DidPositionVelocityChange(float? tolerance = null)
    {
        // Set default param vals
        if (tolerance == null) { tolerance = DEF_POS_TOLERANCE; }

        bool didVelChange = !CompareVectors(prevPositionVelocity, positionVelocity,
            tolerance.Value);
        #region Logs
        CustomDebug.LogForComponent($"Did Pos Vel Change? {didVelChange}. " +
            $"Prev={prevPositionVelocity}. Cur={positionVelocity}", this,
            IS_DEBUGGING);
        #endregion Logs
        return didVelChange;
    }
    /// <summary>
    /// If rotation velocity is not the same as it was last update.
    /// </summary>
    public bool DidRotationVelocityChange(float? tolerance = null)
    {
        // Set default param vals
        if (tolerance == null) { tolerance = DEF_ROT_TOLERANCE; }

        bool didVelChange = !CompareFloats(zAngleVelocity,
            prevZAngleVelocity, tolerance.Value);
        #region Logs
        CustomDebug.LogForComponent($"Did Rot Vel Change? {didVelChange}. " +
            $"Prev={prevZAngleVelocity}. Cur={zAngleVelocity}", this,
            IS_DEBUGGING);
        #endregion Logs
        return didVelChange;
    }
    /// <summary>
    /// If scale velocity is not the same as it was last update.
    /// </summary>
    public bool DidScaleVelocityChange(float? tolerance = null)
    {
        // Set default param vals
        if (tolerance == null) { tolerance = DEF_SCALE_TOLERANCE; }

        bool didVelChange = !CompareVectors(prevScaleVelocity, scaleVelocity,
            tolerance.Value);
        #region Logs
        CustomDebug.LogForComponent($"Did Scale Vel Change? {didVelChange}. " +
            $"Prev={prevScaleVelocity}. Cur={scaleVelocity}", this,
            IS_DEBUGGING);
        #endregion Logs
        return didVelChange;
    }
    /// <summary>
    /// If rotation is changing (if rotation velocity is non-zero).
    /// </summary>
    public bool IsRotationChanging()
    {
        return zAngleVelocity > DEF_ROT_TOLERANCE;
    }
    /// <summary>
    /// If rotation was changing last update 
    /// (if previous rotation velocity is non-zero).
    /// </summary>
    public bool WasRotationChanging()
    {
        return prevZAngleVelocity > DEF_ROT_TOLERANCE;
    }
    /// <summary>
    /// If rotation is chaning right now or was changing last update 
    /// (if previous rotation velocity is non-zero).
    /// </summary>
    public bool IsOrWasRotationChanging()
    {
        return IsRotationChanging() || WasRotationChanging();
    }


    /// <summary>
    /// Calculates the velocities for position, scale, and rotation.
    /// </summary>
    private void CalculateVelocities(float deltaTime)
    {
        // Update previous and current data
        m_prevTransData = m_curTransData;
        m_curTransData = new TransformData(transform, m_isWorldSpace);

        prevPositionVelocity = positionVelocity;
        prevZAngleVelocity = zAngleVelocity;
        prevScaleVelocity = scaleVelocity;

        // If there is no previous data yet, stop.
        if (m_prevTransData == null) { return; }

        // Determine velocities. Assumes previous data was from last update call.
        float inverseDeltaT = 1.0f / deltaTime;
        // For Position and Scale, velocity (m/s) =
        // (p_1 - p_0) / (t_1 - t_0) = (p_1 - p_0) / (delta t)
        positionVelocity = (m_curTransData.position - m_prevTransData.position) * inverseDeltaT;
        scaleVelocity = (m_curTransData.scale - m_prevTransData.scale) * inverseDeltaT;

        // Rotation, just look at the z angle (2D only)
        float prevAngle = m_prevTransData.eulerAngles.z;
        float curAngle = m_curTransData.eulerAngles.z;
        zAngleVelocity = AngleHelpers.SubtractAngles(prevAngle, curAngle) * inverseDeltaT;
        #region Logs
        CustomDebug.LogForComponent($"Cur Rot Vel found to be {zAngleVelocity} " +
            $"with prev={prevAngle}, cur={curAngle}, deltaT={deltaTime}, " +
            $"and invDeltaT={inverseDeltaT}", this, IS_DEBUGGING);
        #endregion Logs


        /* No longer used. Instead we watch the euler angles.
         * 
        // For Quaternions, velocity is much more complicated.
        // 2.0 * exp(log(q_1 / q_0) / (delta t)) * (q_0^(-1)). Found from 
        // https://gamedev.stackexchange.com/questions/30926/quaternion-dfference-time-angular-velocity-gyroscope-in-physics-library
        Quaternion inversePrevQ = Quaternion.Inverse(m_prevTransData.rotation);
        Quaternion diffQ = m_curTransData.rotation * inversePrevQ;
        rotationVelocity = (diffQ.Log().Scale(inverseDeltaT).Exp() * inversePrevQ).Scale(2.0f);
        */
    }
    private bool CompareFloats(float a, float b, float tolerance)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }
    private bool CompareVectors(Vector3 a, Vector3 b, float tolerance)
    {
        Vector3 diff = a - b;
        if (Mathf.Abs(diff.x) > tolerance) { return false; }
        if (Mathf.Abs(diff.y) > tolerance) { return false; }
        if (Mathf.Abs(diff.z) > tolerance) { return false; }
        return true;
    }
    private bool CompareQuaternions(Quaternion a, Quaternion b, float tolerance)
    {
        if (Mathf.Abs(a.x - b.x) > tolerance) { return false; }
        if (Mathf.Abs(a.y - b.y) > tolerance) { return false; }
        if (Mathf.Abs(a.z - b.z) > tolerance) { return false; }
        if (Mathf.Abs(a.w - b.w) > tolerance) { return false; }
        return true;
    }
}
