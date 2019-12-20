using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionController : MonoBehaviour
{
    private MainLevelController mainLevelController;
    private List<GameObject> gameObjectInLevel;
    private List<Transform> enemiesTransform;
    private List<Vector2> enemiesDefaultPosition;
    [SerializeField] private int levelIndex;
    private static string name_EnemyPositions = "EnemyPositions";
    private static string tag_Player = "Player";

    private bool isNotAbleToSwitchToThis;
    private Vector2 tempTransferPosition;
    private float playerMoveInDistanceX;

    private float playerMoveInMinusDistanceX = 2f;

    public int LevelIndex
    {
        get
        {
            return levelIndex;
        }
    }

    public void SetUpStart()
    {
        mainLevelController = FindObjectOfType<MainLevelController>();
        gameObjectInLevel = new List<GameObject>();
        enemiesTransform = new List<Transform>();
        enemiesDefaultPosition = new List<Vector2>();
        tempTransferPosition = new Vector2();

        playerMoveInDistanceX = (transform.GetComponent<BoxCollider2D>().size.x / 2) - playerMoveInMinusDistanceX;

        foreach (Transform child in transform)
        {
            gameObjectInLevel.Add(child.gameObject);
        }

        Transform tempTransform = transform.Find(name_EnemyPositions);
        if (tempTransform != null)
        {
            foreach (Transform enemy in tempTransform)
            {
                enemiesTransform.Add(enemy);
                enemiesDefaultPosition.Add(enemiesTransform[enemiesTransform.Count-1].position);
            }
        }

    }

    public void SetDisableToSwitch(bool isAblemove)
    {
        isNotAbleToSwitchToThis = isAblemove;
    }

    public void SetChildrenActive(bool isActive)
    {
        foreach(GameObject tempGameObject in gameObjectInLevel)
        {
            tempGameObject.SetActive(isActive);

            if (isActive)
            {
                for (int i=0; i<enemiesTransform.Count; i++)
                {
                    enemiesTransform[i].position = enemiesDefaultPosition[i];
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tag_Player))
        {
            if (!isNotAbleToSwitchToThis)
            {
                Global.gameManager.lastCheckPointLevelIndex = levelIndex;

                tempTransferPosition.x = transform.position.x + ((transform.position.x > collision.transform.parent.position.x) ? -playerMoveInDistanceX : playerMoveInDistanceX);
                tempTransferPosition.y = collision.transform.parent.position.y;

                mainLevelController.SwitchToAnotherLevelAndStartScreenFading(this, tempTransferPosition);

                isNotAbleToSwitchToThis = true;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tag_Player))
        {
            isNotAbleToSwitchToThis = false;
        }
    }

}
