using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveRhinoController : MonoBehaviour
{
    [SerializeField] private float moveSpeedX = 0.0f;
    private Vector2 tempMoveSpeed = Vector2.zero;
    [SerializeField] private float existTime = 0.0f;
    private float existTimeTimer = 0.0f;
    [SerializeField] private Animator waveAnimator;
    private bool onWaveMoving = true;
    private SingScript singScript;
    private bool ableToDamagePlayer = true;

    private void Start()
    {
        singScript = FindObjectOfType<SingScript>();
    }

    private void OnEnable()
    {
        waveAnimator.SetBool("Disable", false);
        existTimeTimer = 0.0f;
        onWaveMoving = true;
        ableToDamagePlayer = true;
    }

    private void Update()
    {
        if (onWaveMoving)
        {
            tempMoveSpeed.x = -moveSpeedX;
            tempMoveSpeed.y = 0f;
            transform.Translate(tempMoveSpeed * Time.deltaTime);
        }

        if (existTimeTimer < existTime)
        {
            existTimeTimer += Time.deltaTime;
        }
        else
        {
            waveAnimator.SetBool("Disable", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && ableToDamagePlayer)
        {
            ableToDamagePlayer = false;
            singScript.DamagePlayer(transform);
            StopAllCoroutines();
            StartCoroutine(ColliderIgnoreFewMin());
        }
    }

    private IEnumerator ColliderIgnoreFewMin()
    {
        yield return new WaitForSeconds(2.0f);
        ableToDamagePlayer = true;
    }
    public void CloseTheWave()
    {
        onWaveMoving = false;

        this.transform.position = Vector2.zero;
        this.gameObject.SetActive(false);
    }

}
