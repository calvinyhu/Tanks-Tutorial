using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1; // player identifier to assign controller
    public float m_Speed = 12f; // tank speed
    public float m_TurnSpeed = 180f; // degrees tank will turn over a period of time
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody; // stores reference to tank's rigidbody
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;

    private void Awake() // called regardless of whether or not the tank is on or off, starts when the scene starts
    {
        m_Rigidbody = GetComponent<Rigidbody>(); // storing a reference of a component to a GameObject
    }

    private void OnEnable() // called after Awake(), called when this script is turned on
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    private void OnDisable () // called when a tank is destroyed
    {
        m_Rigidbody.isKinematic = true; // if isKinematic is on it means no forces will affect it
    }

    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update() // Store the player's input and make sure the audio for the engine is playing. Runs every frame, 30 fps, 60 fps
    {
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName); // every float between 1 and -1
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName); // every float between 1 and -1

        EngineAudio();
    }


    private void EngineAudio() // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
    {
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) // if tank is idling
        {
            if (m_MovementAudio.clip == m_EngineDriving) // if its playing EngineDriving, then its playing the wrong audio
            {
                m_MovementAudio.clip = m_EngineIdling; // does not Play() the clip
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play(); // must Play() the clip
            }
        }
        else // if tank is moving
        {
            if (m_MovementAudio.clip == m_EngineIdling) // if its playing EngineIdling, then its playing the wrong audio
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }


    private void FixedUpdate() // Move and turn the tank. Runs every physics steps
    {
        Move();
        Turn();
    }


    private void Move() // Adjust the position of the tank based on the player's input.
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        // creating a vector called movement that will be equal to the tanks forward vector multiplied by a an input value and scaled by a speed value per frame
        // Time.deltaTime smooths out the movement. for example keeps tank moving 12 units per second.

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        // must add m_Rigidbody.position to movement so that tank moves relative to itself and not to the game worlds coordinates
    }


    private void Turn() // Adjust the rotation of the tank based on the player's input.
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        // turn input scaled by a factor of m_TurnSpeed and smoothed out by Time.deltaTime

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        // Quarternion is Unity's variable to store rotation data

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        // rotate the tank RELATIVE to the tank. It doesn't make sense to ADD Quaternions. You can only multiply Quaternions together
    }
}
