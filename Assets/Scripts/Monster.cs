using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public Transform swordPrefab;
	public Transform healthPrefab;
	public int maxHealth;
	public float swordSwingPeriod;

	private World world;	
	private Transform sword;
	private Transform healthbar;
	private float swordSwingTimer;
	private float playerNearbyTime;
	private int health;

	void Start () {
	}

	public void SetWorld(World theWorld)
	{
		this.world = theWorld;

		swordSwingPeriod = world.npc.monsterAttackSpeed;

		health = maxHealth;
		healthbar = (Transform)Component.Instantiate(healthPrefab);
		healthbar.parent = this.transform;
		healthbar.localPosition = new Vector2(0, -0.1f);
	}

	void Update () {

		if(swordSwingTimer > 0 && swordSwingTimer < Time.deltaTime)
		{
			// sword has reached lowest point of swing
			world.player.MonsterAttacked();
		}

		swordSwingTimer -= Time.deltaTime;

		if(swordSwingTimer < 0 && Time.time - playerNearbyTime < 0.5)
		{
			// player is nearby - attack!

			if(sword == null)
			{
				// create sword. Cannot be done in Start - the prefab is missing 
				sword = SpawnSword();
				sword.parent = this.transform;
				sword.localPosition = new Vector2(-0.05f, 0.01f);
			}

			// start swinging sword
			swordSwingTimer = swordSwingPeriod;
			sword.localRotation = new Quaternion();
		}

		if(sword != null)
		{
			if(swordSwingTimer > 0)
			{
				sword.Rotate(0, 0, Time.deltaTime / swordSwingPeriod * 90.0f);
			}
			else
			{
				sword.localRotation = new Quaternion();
			}
		}
	}
	
	public void PlayerNearby()
	{
		playerNearbyTime = Time.time;
	}

	public void TakeDamage()
	{
		print("monster takes damage");
		health--;
		healthbar.localScale = new Vector2((float)health / maxHealth, (float)health / maxHealth);
		//transform.Translate(0, -0.01F, 0);

		if(health <= 0)
		{
			world.MonsterDies();
			Component.DestroyObject(this.gameObject);
		}
	}

	private Transform SpawnSword()
	{
		print("spawned a sword");
		var thing = (Transform)Component.Instantiate(swordPrefab);
		return thing;
	}
}