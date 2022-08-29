using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Object prefab
    [SerializeField] private GameObject _prefab;

    public static ObjectPool Instance { get; private set; }
    private Queue<GameObject> _objects = new Queue<GameObject>();

    // Singleton
    private void Awake()
    {
        Instance = this;
    }

    // Get Object from Pool
    public GameObject Get()
    {
        if (_objects.Count == 0)
            AddObjects(1);
        return _objects.Dequeue();
    }

    // Return Object to Pool
    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        _objects.Enqueue(objectToReturn);
    }

    // Add more Object instances to Pool
    private void AddObjects(int count)
    {
        var _newObject = GameObject.Instantiate(_prefab);
        _newObject.SetActive(false);
        _objects.Enqueue(_newObject);

        _newObject.GetComponent<IGameObjectPooled>().Pool = this;
    }
}
