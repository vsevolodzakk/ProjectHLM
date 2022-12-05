using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemies = new List<GameObject> ();

    private PlayerController _player;

    [SerializeField] private Vector2 _shootNoizePosition;

    public static GameDirector Instance { get; private set; }

    public Vector2 ShootNoizePosition 
    {
        get { return _shootNoizePosition; }
    }

    public List<GameObject> Enemies
    {
        get { return _enemies; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EnemyController.OnEnemyDeath += RemoveEnemyFromChain;
        WeaponController.OnShotFired += LocateShotPosition; 
    }

    private void Start()
    {
        DisplayEnemies();   
        _player = FindObjectOfType<PlayerController> ();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            // If correct Enemy dies, remove it from chain
            // Else resurrect Enemy
            //RemoveEnemyFromChain(0);
            DisplayEnemies();
        }
    }

    private void RemoveEnemyFromChain(int position)
    {
        _enemies.RemoveAt(position);
    }

    private void LocateShotPosition()
    {
        _shootNoizePosition = _player.transform.position;
    }

    private void DisplayEnemies()
    {
        // Display chain of Enemies in the console
        foreach(GameObject obj in _enemies)
            Debug.Log(obj.name);

        Debug.LogWarning("**************");
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyDeath -= RemoveEnemyFromChain;
        WeaponController.OnShotFired -= LocateShotPosition;
    }
}
