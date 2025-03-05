using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Added TMP_Text API for text manipulation
using UnityEngine.UI; // Added UnityEngine.UI API for UI manipulation

public class CollisionHandler : MonoBehaviour
{
    // Let's you change the color of an object upon collision
    public bool changeColor;
    public Color myColor;

    // States of GameObjects to destroy them upon collision
    public bool destroyEnemy;
    public bool destroyCollectibles;

    // Allows you to add an audio file that's played on collision
    public AudioClip collisionAudio;
    private AudioSource audioSource;

    // Push Power variable (for pushing objects)
    public float pushPower = 2.0f;  // Default value of 2.0f

    // New variables for score tracking and UI
    public TMPro.TMP_Text scoreUI; // Reference to TMP_Text component for score display
    public int increaseScore = 1; // Value to increase score
    public int decreaseScore = 1; // Value to decrease score
    private int score = 0; // Private variable to track the score

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if scoreUI is not null and update the score display
        if (scoreUI != null)
        {
            scoreUI.text = score.ToString();
        }
    }

    // Existing collision handler for general collisions
    void OnCollisionEnter(Collision other)
    {
        if (changeColor)
        {
            gameObject.GetComponent<Renderer>().material.color = myColor;
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(collisionAudio, 0.5f);
        }

        if ((destroyEnemy && other.gameObject.CompareTag("Enemy")) || (destroyCollectibles && other.gameObject.CompareTag("Collectible")))
        {
            Destroy(other.gameObject);
        }

        // Update score if colliding with collectible or enemy
        if (scoreUI != null && other.gameObject.CompareTag("Collectible"))
        {
            score += increaseScore;
        }

        if (scoreUI != null && other.gameObject.CompareTag("Enemy"))
        {
            score -= decreaseScore;
        }
    }

    // New function to handle CharacterController collisions
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // If no Rigidbody or if Rigidbody is set to Kinematic, do nothing
        if (body == null || body.isKinematic)
        {
            return;
        }

        // Don't push ground or platform objects below the character
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate the push direction (only along x and z axes, no vertical push)
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Apply the push power and pushing direction to the object if it's tagged as "Object"
        if (hit.gameObject.CompareTag("Object"))
        {
            body.velocity = pushDir * pushPower;
        }

        // Play collision sound if audioSource is available
        if (audioSource != null && !audioSource.isPlaying && collisionAudio != null)
        {
            audioSource.PlayOneShot(collisionAudio, 0.5F);
        }

        // Destroy objects tagged as "Enemy" or "Collectible" if the respective flags are true
        if ((destroyEnemy && hit.gameObject.CompareTag("Enemy")) || (destroyCollectibles && hit.gameObject.CompareTag("Collectible")))
        {
            Destroy(hit.gameObject);
        }

        // Update score if hitting a collectible or enemy
        if (scoreUI != null && hit.gameObject.CompareTag("Collectible"))
        {
            score += increaseScore;
        }

        if (scoreUI != null && hit.gameObject.CompareTag("Enemy"))
        {
            score -= decreaseScore;
        }
    }
}