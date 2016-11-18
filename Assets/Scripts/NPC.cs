using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	public Player player;
	public int maxMonstersToKill;
	public Vector3 npcTargetMonsterPos;
	public Rect textRect; // TODO: not used
	public int numMonstersLeft;
	public float monsterAttackSpeed;

	Transform targetMonster;
	float timePlayerWasNearby;

	void Start () {
		numMonstersLeft = -100;

		// generate texture for NPC
		var texture = TexGen.MakeTexture_RandomMirroredSprite(8, 0.6f, Color.green, 13);
		TexGen.SetSpriteFromTexture(GetComponent<Renderer>(), 0.1f, texture);
	}

	void Update () {
		// hide target monster
		if(targetMonster != null && Time.time - timePlayerWasNearby > 1)
			Component.DestroyObject(targetMonster.gameObject);
	}

	// TODO: text position depends on screen aspect ratio!
//	void OnGUI()
//	{
//		if(targetMonster != null)
//			GUI.Label(textRect, numMonstersLeft + "x");
//	}

	public void PlayerIsNearby(Component monsterPrefab)
	{
		timePlayerWasNearby = Time.time;

		if(numMonstersLeft < 1)
		{
			// first quest, or previous quest completed

			if(numMonstersLeft != -100)
			{
				// quest completed
				GetComponent<ParticleSystem>().Play();
				player.QuestCompleted();
				monsterAttackSpeed = Mathf.Max(0.05f, monsterAttackSpeed * 0.9f);
			}

			print("NPC picked a new monster type");
			if(targetMonster != null)
				Component.DestroyObject(targetMonster.gameObject);
			numMonstersLeft = (int)(Random.value * (maxMonstersToKill - 0.01)) + 1;

			// make new texture for monster prefab
			var texture = TexGen.MakeTexture_RandomMirroredSprite(8, 0.5f);
			TexGen.SetSpriteFromTexture(monsterPrefab.GetComponent<Renderer>(), 0.1f, texture);
		}

		if(targetMonster == null)
		{
			print("spawned a monster target above NPC");
			targetMonster = (Transform)Component.Instantiate(monsterPrefab);
			targetMonster.position = npcTargetMonsterPos;
		}		
	}

	public void MonsterDies()
	{
		numMonstersLeft--;
	}
}