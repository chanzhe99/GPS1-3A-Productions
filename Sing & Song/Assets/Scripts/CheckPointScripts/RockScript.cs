using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    [SerializeField] private Animator rockAnimator;
    [SerializeField] private CheckPoint checkPoint;
    [SerializeField] private Transform singStayPoint;
    [SerializeField] private Transform songStayPoint;
    [SerializeField] private Light directionalLight;
    private float originalLightIntensityValue;
    private SingScript singScript;
    private Transform singRigidbody2D;
    private Transform songRigidbody2D;
    private Animator animatorSing;
    private Animator animatorSong;
    

    private void Start()
    {
        originalLightIntensityValue = directionalLight.intensity;
        singRigidbody2D = FindObjectOfType<GameManager>().singGameObject.GetComponent<Transform>();
        songRigidbody2D = FindObjectOfType<GameManager>().songGameObject.GetComponent<Transform>();
        foreach (Animator animator in singRigidbody2D.GetComponentsInChildren<Animator>())
        {
            if(animator.name != singRigidbody2D.name) { animatorSing = animator; break; }
        }
        foreach (Animator animator in songRigidbody2D.GetComponentsInChildren<Animator>())
        {
            if (animator.name != songRigidbody2D.name) { animatorSong = animator; break; }
        }

        singScript = FindObjectOfType<SingScript>();
        checkPoint.rockScript = this;

        SetAnimationSpecialAnimationLayer_WeightBothSingAndSong(0.0f);

        //FindObjectOfType<UIActiveManager>().SaveCurrentMenusActiveStateAndCloseAllMenus();
        //FindObjectOfType<UIActiveManager>().LoadAndOpenAllMenusToBeforeState();
    }

    private void Update()
    {
        /*
        if (Input.GetMouseButton(0))
        {
            animatorSing.SetBool("directlyRun", true);
            animatorSong.SetBool("directlyRun", true);
        }
        */
    }

    public void specialCheckpointFirstTimeSave()
    {
        animatorSing.SetBool(Global.nameAnimatorBool_Sing_IsRunning, false);
        animatorSong.SetBool(Global.nameAnimatorBool_Song_IsRunning, false);
        animatorSong.SetBool(Global.nameAnimatorBool_Song_IsHealing, false);
        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, false);
        StopCoroutine("RockAnimation");
        StartCoroutine("RockAnimation");
    }

    private IEnumerator RockAnimation()
    {
        float time = 0.0f;
        AllowBothSingAndSongDoAction(false);

        singScript.FlipCharacter(this, singRigidbody2D.position.x < singStayPoint.position.x);
        singScript.FlipSong(this, songRigidbody2D.position.x < songStayPoint.position.x);

        SetAnimationSpecialAnimationLayer_WeightBothSingAndSong(1.0f);
        SetAnimationBoolBothSingAndSong("directlyRun", true);

        Vector3 singStartStepPoint = singRigidbody2D.position, songStartStepPoint = songRigidbody2D.position,
            singFowardStep = Vector3.zero, songFowardStep = Vector3.zero;

        while (true)
        {
            time += 1f * Time.deltaTime;   time = time > 1.0f ? 1.0f : time;

            singFowardStep.x = singStayPoint.position.x; singFowardStep.y = singRigidbody2D.position.y; singFowardStep.z = singRigidbody2D.position.z;
            songFowardStep.x = songStayPoint.position.x; songFowardStep.y = songRigidbody2D.position.y; songFowardStep.z = songRigidbody2D.position.z;

            singRigidbody2D.position = Vector3.Lerp(singStartStepPoint, singFowardStep, time);
            songRigidbody2D.position = Vector3.Lerp(songStartStepPoint, songFowardStep, time);

            if (singRigidbody2D.position.x == singStayPoint.position.x && songRigidbody2D.position.x == songStayPoint.position.x) break;

            yield return null;
        }

        SetAnimationBoolBothSingAndSong("directlyRun", false);

        singScript.FlipCharacter(this, singRigidbody2D.position.x < songRigidbody2D.position.x);
        singScript.FlipSong(this, singRigidbody2D.position.x > songRigidbody2D.position.x);

        SetAnimationBoolBothSingAndSong("directlyRest", true);
        rockAnimator.SetBool("isSaving", true);

        singScript.PlayMusicNoteParticles(true);

        yield return StartCoroutine(LightIntensitySwitch(true));

        while (true)
        {
            /*
            for (int i = 0; i < (int)Global.InputsType.Length; i++)
            {
                if (Input.GetButtonDown(Global.nameInputs[i]))
                {
                    break;
                }
            }
            */
            if (Input.GetButtonDown(Global.nameInputs[(int)Global.InputsType.DashButton]) ||
                Input.GetButtonDown(Global.nameInputs[(int)Global.InputsType.HealButton]) ||
                Input.GetButtonDown(Global.nameInputs[(int)Global.InputsType.Horizontal]) ||
                Input.GetButtonDown(Global.nameInputs[(int)Global.InputsType.InteractButton]) ||
                Input.GetButtonDown(Global.nameInputs[(int)Global.InputsType.JumpButton]) ||
                Input.GetButtonDown(Global.nameInputs[(int)Global.InputsType.MeleeAttackButton]) ||
                Input.GetButtonDown(Global.nameInputs[(int)Global.InputsType.SpiritAttackButton]) ||
                Input.GetButtonDown(Global.nameInputs[(int)Global.InputsType.Vertical]))
            {
                singScript.PlayMusicNoteParticles(false);
                break;
            }
            yield return null;
        } // If press any button the will start to end the rest state

        StartCoroutine(LightIntensitySwitch(false));

        SetAnimationBoolBothSingAndSong("directlyRest", false); // Trigger RestEnd Animation
        rockAnimator.SetBool("isSaving", false); // Trigger 

        AnimatorStateInfo animationState = new AnimatorStateInfo();
        
        while (!animationState.IsName("SingIdle"))
        {
            animationState = animatorSing.GetCurrentAnimatorStateInfo(animatorSing.GetLayerIndex(Global.nameAnimatorLayer_SpecialAnimationLayer));
            yield return null;
        }

        Global.userInterfaceActiveManager.SetMenuVisibilitySmoothly(Global.MenusType.InGameUI, true);
        SetAnimationSpecialAnimationLayer_WeightBothSingAndSong(0.0f);
        AllowBothSingAndSongDoAction(true);
    }

    private IEnumerator LightIntensitySwitch(bool LightOff)
    {
        float time = 0.0f;

        while (true)
        {
            time += 1f * Time.deltaTime;   time = time > 1.0f ? 1.0f : time;

            directionalLight.intensity = Mathf.Lerp(LightOff ? originalLightIntensityValue : 0.0f, LightOff ? 0.0f : originalLightIntensityValue, time);

            if (time >= 1.0f) break;

            yield return null;
        }

        yield return null;
    }

    private void SetAnimationSpecialAnimationLayer_WeightBothSingAndSong(float weight)
    {
        animatorSing.SetLayerWeight(animatorSing.GetLayerIndex(Global.nameAnimatorLayer_SpecialAnimationLayer), weight);
        animatorSong.SetLayerWeight(animatorSong.GetLayerIndex(Global.nameAnimatorLayer_SpecialAnimationLayer), weight);
    }

    private void SetAnimationBoolBothSingAndSong(string id, bool value)
    {
        animatorSing.SetBool(id, value);
        animatorSong.SetBool(id, value);
    }

    private void AllowBothSingAndSongDoAction(bool isAllowDoAction)
    {
        singScript.doAnimationFollowPlayerState = isAllowDoAction;
        singScript.CanDoAction = isAllowDoAction;
    }

    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            rockAnimator.SetBool("isSaving", true);
            //print(rockAnimator.GetBool("isSaving"));
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            rockAnimator.SetBool("isSaving", false);
            //print(rockAnimator.GetBool("isSaving"));
        }
    }
    */
}
