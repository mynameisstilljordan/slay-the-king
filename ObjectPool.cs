using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    /// <summary>
    /// HOW TO IMPLEMENT:
    /// 
    /// When requesting to "spawn" a pooled gameobject:
    /// Gameobject "name" = ObjectPool.Instance.GetObject(parameter);
    /// 
    /// if ("name" != null){
    ///     "name".transform.position = XYZ;
    ///     "name".SetActive(true);
    /// }
    /// 
    /// HOW TO SEND BACK TO POOL:
    /// 
    /// Instead of destroying the gameobject, simply disable it: 
    /// "name".SetActive(false);
    /// </summary>

    public static ObjectPool Instance; //the instance (for singleton pattern)

    int _amountToPool = 5; //the number of each object to pool

    private List<GameObject> _hologram = new List<GameObject>(); //list for ground

    [SerializeField] private GameObject _hologramPrefab; //prefab of ground
    private GameObject _chessBoard;

    private void Awake() {
        if (Instance == null) Instance = this; //set instance to this (if null)
    }

    // Start is called before the first frame update
    void Start() {
        _chessBoard = GameObject.FindGameObjectWithTag("chessBoard");
        
        //ground
        for (int i = 0; i < _amountToPool; i++) { //for the number of objects to pool
            GameObject obj = Instantiate(_hologramPrefab); //instantiate the object
            obj.SetActive(false); //disable the object
            _hologram.Add(obj); //add it to the queue
        }

        foreach (GameObject hologram in _hologram) {
            hologram.transform.SetParent(_chessBoard.transform);
            hologram.transform.localScale = new Vector3(0.125f, 0.125f, 1f); //set the localscale of the piece
        }
    }

    //this method returns the requested pooled object
    public GameObject GetObject() {
        for (int i = 0; i < _hologram.Count; i++) { //for all the gameobjects in the pool
                    if (!_hologram[i].activeInHierarchy) return _hologram[i]; //if an inactive gameobject is found, return it
        }
        return null; //if there is no inactive gameobject of this type, return null
    }
}
