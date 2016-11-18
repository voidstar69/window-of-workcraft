using UnityEngine;

public class Player : MonoBehaviour {

	public World world;

	public float speed;
	public float minPos;
	public float maxPos;
	public float npcDist;
	public float attackDist;
	public float attackPeriod;
	public float swordSwingPeriod;
	public float maxSwordScale;
	public int maxHealth;
	public Rect textRect; // TODO: not used

	private Transform sword;
	private Transform healthbar;
	private int health;
	private int XP = 0;
	private int questXP = 3;

	float attackTimer;
	float swordSwingTimer;

	void Start () {
		//transform.localPosition = new Vector2(-0.4f, 0);

		sword = SpawnSword();
		sword.parent = transform;
		sword.localPosition = new Vector2(0.01f, 0.01f);

		health = maxHealth;
		healthbar = (Transform)Component.Instantiate(world.healthPrefab);
		healthbar.parent = this.transform;
		healthbar.localPosition = new Vector2(0, -0.1f);
	}

	void OnGUI()
	{
		Rect rect = new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, 100);

		// TODO: text vanishes when we try to use GUIStyle!
		//GUIStyle style = new GUIStyle();
		//style.fontSize = 10;
		//style.fontStyle = FontStyle.Bold; 

		GUI.Label(rect, XP + " XP"); //, style);
		
		//		if(targetMonster != null)
		//			GUI.Label(textRect, numMonstersLeft + "x");
	}

	void Update () {

		// TODO: tex gen here is slower, but good for experimenting. Eventually move to Start()
		// moire pattern
		//var texture = TexGen.MakeTextureUnit(16, (x,y) => new Color(((x * y * 100) % 3) / 2, 0, 0));
		//var texture = TexGen.MakeTexture_Sierpinski(16);
		//var texture = TexGen.MakeTexture_Random(16);
		//var texture = TexGen.MakeTexture_RandomSplat(8);
		//var texture = TexGen.MakeTexture_RandomMirroredSprite(8, 0.6f, Color.green, 13);
		//TexGen.SetSpriteFromTexture(renderer, 0.1f, texture);

    // keyboard input
    float xInput = Input.GetAxis("Horizontal");

    // touch input
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
    {
      Vector2 delta = Input.GetTouch(0).deltaPosition;
      xInput += delta.x / 5f;
    }

		// movement
		float translation = xInput * speed;
		translation *= Time.deltaTime;
		var pos = transform.position;
		pos.x += translation;
		pos.x = Mathf.Min(Mathf.Max(pos.x, minPos), maxPos);
		transform.position = pos;

		// player near monster?
		if(world.MonsterExists() && DistToMonster() <= attackDist)
		{
			world.PlayerNearMonster();
		}

		// attack monster?
		attackTimer -= Time.deltaTime;
		if(attackTimer < 0 && world.MonsterExists() && DistToMonster() <= attackDist)
		{
			attackTimer = attackPeriod;
			world.PlayerAttackMonster();
			XP++;
			GetComponent<ParticleSystem>().Play();

			// start swinging sword
			swordSwingTimer = swordSwingPeriod;
		}

		swordSwingTimer -= Time.deltaTime;
		if(swordSwingTimer > 0)
		{
			sword.Rotate(0, 0, Time.deltaTime / swordSwingPeriod * -90.0f);
		}
		else
		{
			sword.localRotation = new Quaternion();
		}

		// talk to NPC
		if(pos.x - minPos <= npcDist)
		{
			world.PlayerIsNearNPC();
		}
	}

	public float DistToMonster()
	{
		// TODO: assumes monster at maxPos
		return maxPos - transform.position.x;
	}

	public void MonsterAttacked()
	{
		// player only takes damage near monster
		if(DistToMonster() > attackDist)
			return;

		print("player takes damage");
		health--;
		healthbar.localScale = new Vector2((float)health / maxHealth, (float)health / maxHealth);
		//transform.Translate(0, -0.01F, 0);
		
		if(health <= 0)
		{
			// Respawn player
			XP = Mathf.Max(0, XP / 2);
			questXP = 3;

			health = maxHealth;
			healthbar.localScale = Vector2.one;
			sword.localScale = Vector2.one;
			transform.localPosition = new Vector2(-0.4f, 0);

			// Freeze player in a spot
			//speed = 0;
			//transform.position = new Vector2(0.5f, -0.2f);
		}
	}

	public void QuestCompleted()
	{
		print("player completed quest");

		// heal player
		health = maxHealth;
		healthbar.localScale = Vector2.one;

		// reward player with quest XP. Next quest will have double XP!
		questXP *= 2;
		XP += questXP;

		// make sword bigger 
		var scale = sword.localScale.x * 1.1f;
		scale = Mathf.Min(scale, maxSwordScale);
		sword.localScale = new Vector2(scale, scale);
	}

	private Transform SpawnSword()
	{
		print("spawned a sword");
		return (Transform)Component.Instantiate(world.swordPrefab);
	}
}