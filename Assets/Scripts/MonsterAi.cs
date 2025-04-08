using System.Collections;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public Transform player;
    public Transform respawnPoint;
    public float moveSpeed = 2f;
    public float detectionRange = 10f;
    public float fearRadius = 8f; // Radius for the player's terror sound
    public AudioSource monsterAudioSource;
    public AudioSource playerAudioSource;
    public AudioClip roarSound; // Monster roar (MP3)
    public AudioClip terrorSound; // Player fear sound (WAV)
    public Light detectionLight;

    private Animator animator;
    private bool inLight = false;
    private bool isChasing = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (monsterAudioSource == null)
        {
            monsterAudioSource = gameObject.AddComponent<AudioSource>();
        }
        if (playerAudioSource == null && player != null)
        {
            playerAudioSource = player.GetComponent<AudioSource>();
            if (playerAudioSource == null)
            {
                playerAudioSource = player.gameObject.AddComponent<AudioSource>();
            }
        }

        monsterAudioSource.clip = roarSound;
    }

    void Update()
    {
        if (inLight)
        {
            ReturnToRespawn();
        }
        else
        {
            DetectAndChasePlayer();
            PlayTerrorSound();
        }

        SwitchRandomAnimations();
    }

    void DetectAndChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {
            isChasing = true;
            animator.SetTrigger("Roar");

            if (!monsterAudioSource.isPlaying)
            {
                monsterAudioSource.PlayOneShot(roarSound);
            }

            StartCoroutine(ChaseAfterRoar());
        }
    }

    IEnumerator ChaseAfterRoar()
    {
        yield return new WaitForSeconds(1.5f);
        if (!inLight)
        {
            animator.SetTrigger("Walk");
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    void ReturnToRespawn()
    {
        isChasing = false;
        animator.SetTrigger("Walk");
        transform.position = Vector3.MoveTowards(transform.position, respawnPoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, respawnPoint.position) < 0.5f)
        {
            animator.SetTrigger("Idle");
        }
    }

    void PlayTerrorSound()
    {
        if (playerAudioSource != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < fearRadius && !playerAudioSource.isPlaying)
            {
                playerAudioSource.PlayOneShot(terrorSound);
            }
        }
    }

    void SwitchRandomAnimations()
    {
        if (!isChasing)
        {
            int randomState = Random.Range(0, 3);
            switch (randomState)
            {
                case 0:
                    animator.SetTrigger("Idle");
                    break;
                case 1:
                    animator.SetTrigger("Flex");
                    break;
                case 2:
                    animator.SetTrigger("Roar");
                    if (!monsterAudioSource.isPlaying)
                    {
                        monsterAudioSource.PlayOneShot(roarSound);
                    }
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LightSource"))
        {
            inLight = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LightSource"))
        {
            inLight = false;
        }
    }
}
