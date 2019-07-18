using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionController : MonoBehaviour
{
    private MainLevelController mainLevelController;
    private List<GameObject> gameObjectInLevel;
    [SerializeField] private int levelIndex;

    public int LevelIndex
    {
        get
        {
            return levelIndex;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        mainLevelController = FindObjectOfType<MainLevelController>();
        gameObjectInLevel = new List<GameObject>();

        foreach(Transform child in transform)
        {
            gameObjectInLevel.Add(child.gameObject);
        }

    }

    public void setChildrenActive(bool isActive)
    {
        foreach(GameObject tempGameObject in gameObjectInLevel)
        {
            tempGameObject.SetActive(isActive);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mainLevelController.currentLevel.setChildrenActive(false);
            mainLevelController.currentLevel = this;
            setChildrenActive(true);
        }
    }

}
