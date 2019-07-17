using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OpenningTimelineController : MonoBehaviour
{
    private bool isMoviePlayed = false; //! This one need to push it in GameManager

    public bool IsMoviePlayed
    {
        get
        {
            return isMoviePlayed;
        }
    }
    
    private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset menuWaiting;
    [SerializeField] private PlayableAsset openingMovie1;
    [SerializeField] private PlayableAsset openingMovie2;
    [SerializeField] private GameObject singGameObject;
    [SerializeField] private Transform playerPivotPoint;
    [SerializeField] private GameObject boatGameObject;
    private float singDefaultGravityScaleValue;

    private bool isLastOpeningMovie = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (isMoviePlayed == false)
        {
            singDefaultGravityScaleValue = singGameObject.GetComponent<Rigidbody2D>().gravityScale;
            playableDirector = GetComponent<PlayableDirector>();
            PlayMenuWaiting();
        }
        else
        {
            boatGameObject.GetComponent<Animator>().enabled = false;
            playerPivotPoint.GetComponent<Animator>().enabled = false;
            DestroyAllOpeningGameObjects();
        }
    }

    private void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (playableDirector == aDirector)
        {
            if (!isLastOpeningMovie)
            {
                PlayOpeningPart2Movie();
            }
            else
            {
                singGameObject.GetComponent<Transform>().SetParent(null, true);
                EnableSingController();

                //! this one no point, is just for save the isMoviePlayed data, you can change it.

                playableDirector.stopped -= OnPlayableDirectorStopped;
                DestroyAllOpeningGameObjects();
            }
        }
            
    }

    private void PlayMenuWaiting()
    {
        singGameObject.transform.position = playerPivotPoint.position;
        singGameObject.transform.SetParent(playerPivotPoint, true);

        playableDirector.Play(menuWaiting, DirectorWrapMode.Loop);
        DisableSingController();
    }

    private void DisableSingController()
    {
        singGameObject.GetComponent<SingScript>().enabled = false;
        singGameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
    }

    public void PlayOpeningPart1Movie()
    {
        playableDirector.Play(openingMovie1, DirectorWrapMode.None);

        DisableSingController();

        playableDirector.stopped += OnPlayableDirectorStopped;
    }
    private void PlayOpeningPart2Movie()
    {
        playerPivotPoint.SetParent(null, true);

        playableDirector.Play(openingMovie2, DirectorWrapMode.None);
        DisableSingController();

        isLastOpeningMovie = true;
    }

    private void EnableSingController()
    {
        singGameObject.GetComponent<SingScript>().enabled = true;
        singGameObject.GetComponent<Rigidbody2D>().gravityScale = singDefaultGravityScaleValue;
    }

    private void DestroyAllOpeningGameObjects()
    {

        Destroy(playerPivotPoint.gameObject);
        Destroy(gameObject);

    }

}
