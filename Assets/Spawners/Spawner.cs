using UnityEngine;
using System.Collections;
using System;

public class Spawner : MonoBehaviour {
    [Header("Object References")]
    public Unit unitPrefab;
    public Transform container;
    [Space(5)]

    [Header("Spawner Stats")]
    public Team team = Team.T1;
    public float spawningInterval = 10;
    public Vector3 destination;
    [Space(5)]

    private float timeUntilSpawn;

	void Start () {
        timeUntilSpawn = spawningInterval;
    }
	
	void Update () {
        timeUntilSpawn -= Time.deltaTime;
        if (timeUntilSpawn < 0)
        {
            timeUntilSpawn += spawningInterval;
            StartCoroutine(CreateUnit());
        }
	}

    private IEnumerator CreateUnit()
    {
        //Unit unit = Instantiate(unitPrefab.gameObject, transform.position, Quaternion.identity) as Unit;
        Unit unit = Instantiate(unitPrefab);
        unit.transform.localPosition = transform.position;
        unit.transform.SetParent(container);
        unit.team = team;
        unit.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;

        Data.GetInstance().AddUnit(unit);

        yield return null;
        
        Command command = new Command(CommandType.Attack, destination);
        if (destination.Equals(Vector3.zero)) command.pos = transform.position;
        unit.PerformCommand(command);
    }
}
