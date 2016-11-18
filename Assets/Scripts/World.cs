using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

	public Transform monsterPrefab;
	public Transform swordPrefab;
	public Transform healthPrefab;
	public Player player;
	public NPC npc;

	public Vector3 monsterSpawnPos;
	public float monsterSpawnDelay;
	public float monsterSpawnDist;
	float monsterSpawnTimer;

	private Transform monster;
	
	void Start () {
		monsterSpawnTimer = monsterSpawnDelay;
	}
	
	void Update () {
		if(monster == null)
		{
			monsterSpawnTimer -= Time.deltaTime;
			if(monsterSpawnTimer < 0 && player.DistToMonster() > monsterSpawnDist && npc.numMonstersLeft > 0)
			{
				monsterSpawnTimer = monsterSpawnDelay;
				monster = SpawnMonster();
			}
		}
	}
	
	public void PlayerNearMonster()
	{
		// TODO: how to get hold of Monster object?
		monster.BroadcastMessage("PlayerNearby");
	}

	public void PlayerAttackMonster()
	{
		// TODO: how to get hold of Monster object?
		monster.BroadcastMessage("TakeDamage");
	}

	public void PlayerIsNearNPC()
	{
		npc.PlayerIsNearby(monsterPrefab);

//		if(monster == null)
//			monster = SpawnMonster();
	}

	public bool MonsterExists()
	{
		return monster != null;
	}

	public void MonsterDies()
	{
		print("monster died");
		npc.MonsterDies();
	}

	private Transform SpawnMonster()
	{
		print("spawned a monster");
		var thing = Object.Instantiate(monsterPrefab) as Transform;
		thing.position = monsterSpawnPos;
		thing.BroadcastMessage("SetWorld", this);
		return thing;
	}
}