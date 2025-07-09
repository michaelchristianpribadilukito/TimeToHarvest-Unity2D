using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Animator animator;
    private Vector3 direction;

    private Player playerController;
    private AudioSource walkAudioSource;

    private void Awake() 
    {
        playerController = GetComponent<Player>();
        walkAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Cek jika game di-pause ATAU jika pemain sedang melakukan aksi lain (mencangkul, dll.)
        if (GameManager.instance.isPaused || (playerController != null && playerController.isPerformingAction))
        {
            // PERBAIKAN: Jika suara jalan masih berputar, hentikan sekarang.
            if (walkAudioSource != null && walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }

            // Atur arah ke nol agar pemain tidak meluncur
            direction = Vector3.zero;
            // Perbarui animasi ke kondisi idle
            AnimateMovement(direction);
            // Hentikan eksekusi fungsi Update lebih lanjut
            return;
        }

        // Kode di bawah ini hanya akan berjalan jika game TIDAK di-pause dan pemain TIDAK melakukan aksi.
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontalInput, verticalInput);

        AnimateMovement(direction);

        // Logika untuk play/stop SFX berjalan (ini tetap sama)
        if (walkAudioSource != null)
        {
            if (direction.magnitude > 0 && !walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
            else if (direction.magnitude == 0 && walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        // Move Player
        transform.position += direction * speed * Time.deltaTime;
    }

    private void AnimateMovement(Vector3 direction)
    {
        if (animator != null)
        {
            if (direction.magnitude > 0)
            {
                animator.SetBool("isMoving", true);
                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.y);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }
}
