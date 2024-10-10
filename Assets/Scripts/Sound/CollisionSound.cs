using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip collisionSound;
    
    private void OnCollisionEnter(Collision collision)
    {

        if (audioSource != null && collisionSound != null)
        {
            float collisionForce = collision.relativeVelocity.magnitude;
            
            audioSource.PlayOneShot(collisionSound, Mathf.Clamp(collisionForce / 10f, 0.1f, 1.0f));
        }
    }
}
