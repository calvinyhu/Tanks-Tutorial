using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f; // approx time for the camera to wait until it should move
    public float m_ScreenEdgeBuffer = 4f; // a number that we add to the size so that the tanks are not at the edge of the screen
    public float m_MinSize = 6.5f; // we don't want the camera to become extremely small
    [HideInInspector] public Transform[] m_Targets; // array of tanks


    private Camera m_Camera; // reference to camera
    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition; // the position the camera is trying to reach (the position between the two tanks, their average pos)


    private void Awake() // setup all references
    {
        m_Camera = GetComponentInChildren<Camera>();
        // create a reference to main camera (must use "InChildren" because main camera is a child)
    }


    private void FixedUpdate() // tanks are physics objects so camera will be in sync with tanks by using FixedUpdate
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        // ref writes back to that variable "m_MoveVelocity" 
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3(); // create new Vector3 but leave it blank
        int numTargets = 0; // default to 0, but will be number of tanks on screen

        for (int i = 0; i < m_Targets.Length; i++) // cycle through array/list of tanks to do stuff AND get numTargets
        {
            if (!m_Targets[i].gameObject.activeSelf) // check if a tank is NOT active
                continue; // skip until next iteration of for loop

            // assuming a tank is active
            averagePos += m_Targets[i].position; // add tank pos to averagePos
            numTargets++;
        }

        if (numTargets > 0) // if there are more than 0 tanks
            averagePos /= numTargets; // find average

        averagePos.y = transform.position.y; // inherit CameraRig's y position, so it remains frozen, it does not move up and down

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize() // Find the tank that's furthest away from desired position and change size to the largest distance
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++) // looping through active tanks and skipping inactive tanks (destroyed tanks)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}