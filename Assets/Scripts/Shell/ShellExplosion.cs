using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask; // a layer in which only tanks exist
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              

    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime); // destroy the gameobject after 2 seconds
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        // Gets all colliders, at the shell's position within the explosion radius, which are tanks and puts them in an array

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            // Get a rigidbody in a collider so that we can add an explosion force to the tank

            if (!targetRigidbody)
                continue; // if no rigidbody in collider go back to the start of the for loop and find a rigidbody

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
            // Add a force to the tank's rigidbody so it moves when it takes damage

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            // Get a TankHealth from the targetRigidbody, the tank object, which also has a tank health script

            if (!targetHealth)
                continue; // if the targetRigidbody, the tank object, has no tank health then find an object that does have tank health

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
        Destroy (gameObject);
    } // Find all the tanks in an area around the shell and damage them.


    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position; // vector from the target to the shell

        float explosionDistance = explosionToTarget.magnitude; // magnitude of vector

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius; // a float between 0 and 1

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage); // making sure damage is not negative, so we dont give health

        return damage;
    } // Calculate the amount of damage a target should take based on it's position.
}
