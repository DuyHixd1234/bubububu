using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    Animator _animator;
    public AudioSource src;
    public AudioClip sfxRun, sfxBlade, sfxJump;

    private bool isAttackSoundPlaying = false;
    private bool isJumpSoundPlaying = false;

    public GameObject jumpEffectPrefab;

    private Coroutine soundCoroutine;

    public PlayerHealthHandler playerHealthHandler;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Idle()
    {
        if (_animator != null)
            _animator.SetBool("do_idle", true);
    }

    public void Run()
    {
        if (_animator != null)
            _animator.SetBool("do_idle", false);

        // Start playing the run sound (handled elsewhere)
    }

    public void Jump()
    {
        if (_animator != null)
            _animator.SetTrigger("do_jump");

        if (!isJumpSoundPlaying)
        {
            StartCoroutine(PlayJumpSound());
        }

        if (jumpEffectPrefab != null)
        {
            Instantiate(jumpEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    IEnumerator PlayJumpSound()
    {
        if (src == null || sfxJump == null)
            yield break;

        isJumpSoundPlaying = true;

        // tiny delay to let animation trigger (keeps previous behavior)
        yield return new WaitForSeconds(0.01f);

        // Play the jump sound using the clip length to wait
        float length = sfxJump.length;
        src.clip = sfxJump;
        src.Play();

        yield return new WaitForSeconds(length);

        // Reset flag to indicate that the jump sound has finished playing
        isJumpSoundPlaying = false;
    }

    IEnumerator ResetAttackSoundFlag()
    {
        // Determine attack clip duration safely
        float length = 0f;
        if (sfxBlade != null)
        {
            length = sfxBlade.length;
        }
        else if (src != null && src.clip != null)
        {
            length = src.clip.length;
        }

        if (length > 0f)
            yield return new WaitForSeconds(length);

        // Reset flag to indicate that the attack sound has finished playing
        isAttackSoundPlaying = false;
    }

    public void AirBorn()
    {
        if (_animator != null)
            _animator.SetBool("do_falling", true);
    }

    public void Landed()
    {
        if (_animator != null)
            _animator.SetBool("do_falling", false);
    }

    public void Attack()
    {
        if (_animator != null)
            _animator.SetTrigger("do_attack");

        if (src == null || sfxBlade == null)
            return;

        if (!isAttackSoundPlaying)
        {
            isAttackSoundPlaying = true;

            src.loop = false;

            src.clip = sfxBlade;
            src.Play();

            StartCoroutine(ResetAttackSoundFlag());
        }
    }

    public void PlayerDied()
    {
        if (playerHealthHandler != null && playerHealthHandler.PlayerCurrentHealth == 0)
        {
            if (src != null)
                src.Stop();
        }
    }

    public void RestartSound()
    {
        if (src != null)
            src.Play();
    }
}           