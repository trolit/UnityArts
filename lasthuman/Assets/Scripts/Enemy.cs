﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    public AudioSource[] sounds;

    public static int damageDealt;

    private bool DroppedCoin = false;

    // sound effects:
    public AudioClip slash_hit01;
    public AudioClip slash_hit02;
    public AudioClip slash_hit03;
    public AudioClip slash_hit04;

    public AudioClip zombie_die;

    static AudioSource audioZombie;
    static AudioSource meleeSounds;

    public AudioClip attack01;
    public AudioClip attack02;

    // variable to watch state
    private IEnemyState currentState;

    // property Target to set enemy Target
    public GameObject Target { get; set; }

    [SerializeField]
    private Image image;

    [SerializeField]
    private float MeleeRange;

    // color damage manager
    public static bool playertxtColor;


    // variables start with small letter
    private static Enemy instance;

    // player singleton
    public static Enemy Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Enemy>();
            }
            return instance;
        }
    }

    public bool InMeleeRange
    {
        get
        {
            if (Target != null)
            {
                // if range is enough to do melee attack return true
                return Vector2.Distance(transform.position, Target.transform.position) <= MeleeRange;
            }

            // else false
            return false;
        }
    }

    public override bool IsDead
    {
        get
        {
            return healthStat.CurrentValue <= 0;
        }

        set
        {

        }
    }

    [SerializeField]
    private Transform leftEdge;
    [SerializeField]
    private Transform rightEdge;

    private Canvas healthCanvas;

    // to manipulate dead body
    private BoxCollider2D bodyCollider;

    [SerializeField]
    private Text task1_status;
    private static int task1_counter = 0;

    [SerializeField]
    private Text task2_status;
    private int task2_counter = 0;

    [SerializeField]
    private Text task3_status;
    private static int task3_counter = 0;

    [SerializeField]
    private Text task1_cross;
    [SerializeField]
    private Text task2_cross;
    [SerializeField]
    private Text task3_cross;

    private bool doneQuest1 = false;
    private bool doneQuest2 = false;
    private bool doneQuest3 = false;

    // Use this for initialization
    public override void Start()
    {
        // reset values
        task1_counter = 0;
        task2_counter = 0;
        task3_counter = 0;
        task1_cross.text = "";
        task2_cross.text = "";
        task3_cross.text = "";
        doneQuest1 = false;
        doneQuest2 = false;
        doneQuest3 = false;

        sounds = GetComponents<AudioSource>();
        audioZombie = sounds[0];
        meleeSounds = sounds[1];

        bodyCollider = GetComponent<BoxCollider2D>();

        base.Start();

        // RemoveTarget function called whenever Players Dead event is triggered
        // we can access Player instance cause we have that singleton...
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);

        ChangeState(new IdleState());

        // reference to Canvas so will be able to attach/detach Canvas which has enemy health bar
        healthCanvas = transform.GetComponentInChildren<Canvas>();

        FloatingTextController.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        // if not dead
        if (!IsDead)
        {
            if (!TakingDamage)
            {
                currentState.Execute();
            }

            // look at target if taking damage
            LookAtTarget();
        }


        // flip enemy bar
        if (facingRight)
        {
            image.fillOrigin = 0;
        }
        else
        {
            image.fillOrigin = 1;
        }
    }

    public void RemoveTarget()
    {
        // if enemy kills player
        // goes into Patrol State
        // and loses target
        Target = null;
        ChangeState(new PatrolState());
    }

    // keep an eye on player
    private void LookAtTarget()
    {
        if (Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;
            if (xDir < 0 && facingRight || xDir > 0 && !facingRight)
            {
                ChangeDirection();
            }
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();

        }

        //change current state to a new state
        currentState = newState;

        // enter new state
        currentState.Enter(this);
    }

    public void Move()
    {
        // if attack is false, we can move
        if (!Attack)
        {
            // can move unless on the edge
            if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) ||
                (GetDirection().x < 0 && transform.position.x > leftEdge.position.x))
            {
                // to avoid sliding
                MyAnimator.SetFloat("speed", 1);

                transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
            }
            // if our current state is PatrolState
            // Change direction so we can move and
            // continue patrolling
            else if (currentState is PatrolState)
            {
                ChangeDirection();
            }
        }
    }

    public Vector2 GetDirection()
    {
        // short form of if statement
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }



    public override IEnumerator TakeDamage()
    {
        if (!healthCanvas.isActiveAndEnabled && !IsDead)
        {
            healthCanvas.enabled = true;
        }

        int damage;
        if (Player.firedSoul)
        {
            damage = Random.Range(20, 60);
            healthStat.CurrentValue -= damage;
        }
        else
        {
            // 5 - 20
            damage = Random.Range(15, 30);
            healthStat.CurrentValue -= damage;
            playertxtColor = true;
        }

        // if attacked by player from back, turn around
        if(Target == null && !IsDead)
        {
            ChangeDirection();
        }

        if (!IsDead)
        {
            FloatingTextController.CreateFloatingText(damage.ToString(), transform);

            MyAnimator.SetTrigger("damage");

            if (!facingRight)
            {
                Instantiate(GameManager.Instance.BloodEffect, new Vector3(transform.position.x, transform.position.y), GameManager.Instance.BloodEffect.transform.rotation);
            }
            else if (facingRight)
            {
                ParticleSystem bloodObject = Instantiate(GameManager.Instance.BloodEffect, new Vector3(transform.position.x, transform.position.y), GameManager.Instance.BloodEffect.transform.rotation);
                bloodObject.transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1);
            }

            int random = Random.Range(1, 4);
            if (random == 1) audioZombie.PlayOneShot(Enemy.Instance.slash_hit01);
            else if (random == 2) audioZombie.PlayOneShot(Enemy.Instance.slash_hit02);
            else if (random == 3) audioZombie.PlayOneShot(Enemy.Instance.slash_hit03);
            else if (random == 4) audioZombie.PlayOneShot(Enemy.Instance.slash_hit04);
        }
        else if (IsDead && !DroppedCoin)
        {
            // if someone got killed increase one of the three global variables
            SaveOverallKills();

            if (task1_counter < 6)
            {
                task1_counter++;
                task1_status.text = task1_counter + "/6";
            }

            if (task1_counter == 6 && !doneQuest1)
            {
                Player.completedQuests++;
                task1_cross.text = "____________________________";
                doneQuest1 = true;
            }

            if (gameObject.name == "CondemnedWarrior" || gameObject.name == "CondemnedWarrior1")
            {
                if (task3_counter < 2)
                {
                    task3_counter++;
                    task3_status.text = task3_counter + "/2";
                }

                if (task3_counter == 2 && !doneQuest3)
                {
                    Player.completedQuests++;
                    task3_cross.text = "____________________________";
                    doneQuest3 = true;
                }
            }
            else if(gameObject.name == "Troll1")
            {
                if(task2_counter < 1)
                {
                    task2_counter++;
                    task2_status.text = task2_counter + "/1";
                }

                if(task2_counter == 1 && !doneQuest2)
                {
                    Player.completedQuests++;
                    task2_cross.text = "____________________________";
                    doneQuest2 = true;
                }

                bodyCollider.size = new Vector2(1.37f, 4.39f);
            }
            else
            {
                // place dead body on the ground correctly
                // by changing its collider
                bodyCollider.size = new Vector2(1.37f, 3.0f);
            }

            Time.timeScale = 0.5f;
            Invoke("TurnOffSlowMotion", 0.5f);

            playertxtColor = true;

            FloatingTextController.CreateFloatingText(damage.ToString(), transform);

            audioZombie.PlayOneShot(zombie_die);
            MyAnimator.SetTrigger("die");

            if (!facingRight)
            {
                Instantiate(GameManager.Instance.BloodEffect, new Vector3(transform.position.x, transform.position.y), GameManager.Instance.BloodEffect.transform.rotation);
            }
            else if (facingRight)
            {
                ParticleSystem bloodObject = Instantiate(GameManager.Instance.BloodEffect, new Vector3(transform.position.x, transform.position.y), GameManager.Instance.BloodEffect.transform.rotation);
                bloodObject.transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1);
            }


            // spawns soul
            Instantiate(GameManager.Instance.SoulPrefab, new Vector3(transform.position.x, transform.position.y + 1.5f), Quaternion.identity);
            healthCanvas.enabled = false;
            DroppedCoin = true;

            yield return null;
        }
    }

    private void SaveOverallKills()
    {
        if ((gameObject.name == "Zombie1" || gameObject.name == "Zombie2" || gameObject.name == "Zombie3" || gameObject.name == "Zombie4") && IsDead)
        {
            KillsSaver.zombiesKilled++;
            PlayerPrefs.SetInt("zombieKill", KillsSaver.zombiesKilled);
        }
        else if((gameObject.name == "CondemnedWarrior" || gameObject.name == "CondemnedWarrior1") && IsDead)
        {
            KillsSaver.warriorsKilled++;
            PlayerPrefs.SetInt("warriorKill", KillsSaver.warriorsKilled);
        }
        else if(gameObject.name == "Troll1" && IsDead)
        {
            KillsSaver.ogresKilled++;
            PlayerPrefs.SetInt("ogreKill", KillsSaver.ogresKilled);
        }
    }

    private void TurnOffSlowMotion()
    {
        Time.timeScale = 1.0f;
    }

    public override void Death()
    {
        Destroy(gameObject);
    }

    private bool playedEffect = false;

    public override void MeleeAttack()
    {
        base.MeleeAttack();

        if(gameObject.name == "Zombie1" || gameObject.name == "Zombie2" || gameObject.name == "Zombie3" || gameObject.name == "Zombie4")
        {
            damageDealt = Random.Range(5, 20);
        }
        else if(gameObject.name == "CondemnedWarrior" || gameObject.name == "CondemnedWarrior1")
        {
            damageDealt = Random.Range(20, 35);
        }
        else if(gameObject.name == "Troll1")
        {
            damageDealt = Random.Range(35, 70);
        }

        int random;
        if (!playedEffect)
        {
            random = Random.Range(1, 2);
            if (random == 1) meleeSounds.PlayOneShot(attack01);
            else if (random == 2) meleeSounds.PlayOneShot(attack02);
            playedEffect = true;
            Invoke("Attackeffect_cooldown", 0.5f);
        }
    }

    private void Attackeffect_cooldown()
    {
        playedEffect = false;
    }
}
