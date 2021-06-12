using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float jumphight;
    public Transform gameob;
    bool isground;
    Animator anim;
    int curHp;
    int maxHp = 3;
    bool isHit = false;
    public Main main;
    public bool key = false;
    bool canTP = true;
    public bool inWater = false;
    bool isclimb = false;
    int coins = 0;
    bool canHit = true;
    public GameObject blueGem, greenGem;
    int gemCount = 0;
    float hitTimer = 0f;
    public Image PlayerCountDown;
    float insideTimer = -1f;
    public float insideTimerUp = 5f;
    public Image insideCountDown;
    public inventari inventari;
    public soundeff soundeff;

    public Joystick joystick;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        curHp = maxHp;
    }


    void Update()
    {
        if (inWater && !isclimb)
        {
            anim.SetInteger("state", 4);
            isground = true;
            if (joystick.Horizontal >= 0.3f || joystick.Horizontal <= -0.3f)
                flip();
        }
        else
        {
            checkground();

            if (joystick.Horizontal < 0.3f && joystick.Horizontal > -0.3f && (isground) && !isclimb)
            {
                anim.SetInteger("state", 1);
            }
            else
            {
                flip();
                if (isground && !isclimb)
                    anim.SetInteger("state", 2);
            }
        }
       


        if (insideTimer >= 0f)
        {
            insideTimer += Time.deltaTime;
            if (insideTimer >= insideTimerUp)
            {
                insideTimer = -1f;
                RecountHp(-1);
            }
            else
                insideCountDown.fillAmount = 1 - (insideTimer / insideTimerUp);
        }

       
    }

    public void jump()
    {
        if (isground)
        {
            rb.AddForce(transform.up * jumphight, ForceMode2D.Impulse);
            soundeff.PlayJumpSound();
        }
    }


    private void FixedUpdate()
    {
        if (joystick.Horizontal >= 0.3f)
            rb.velocity = new Vector2(speed, rb.velocity.y);
        else if (joystick.Horizontal <= -0.3f)
            rb.velocity = new Vector2(-speed, rb.velocity.y);

        else
            rb.velocity = new Vector2(0f, rb.velocity.y);



    }


    void flip()
    {
        if (joystick.Horizontal >= 0.4f)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (joystick.Horizontal <= -0.4f)
            transform.localRotation = Quaternion.Euler(0, 180, 0);

    }

    void checkground()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameob.position, 0.2f);
        isground = colliders.Length > 1;
        if (!isground && !isclimb)
            anim.SetInteger("state", 3);
    }

    public void RecountHp(int deltaHP)
    {
        
        if (deltaHP < 0 && canHit) 
        {
            curHp = curHp + deltaHP;
            StopCoroutine(Onhit());
            canHit = true;
            isHit = true;
            StartCoroutine(Onhit());
      
        }
        else if (curHp > maxHp)
        {
            curHp = curHp + deltaHP;
            curHp = maxHp;
        }
        print(curHp);
        if (curHp <= 0)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            Invoke("Lose", 1.5f);
        }
    }


    IEnumerator Onhit()
    {
        if(isHit)
        GetComponent<SpriteRenderer>().color = new Color(255f, GetComponent<SpriteRenderer>().color.g - 0.04f, GetComponent<SpriteRenderer>().color.b - 0.04f);
        else
            GetComponent<SpriteRenderer>().color = new Color(255f, GetComponent<SpriteRenderer>().color.g + 0.04f, GetComponent<SpriteRenderer>().color.b + 0.04f);
        if (GetComponent<SpriteRenderer>().color.g == 1f)
        {
            StopCoroutine(Onhit());
            canHit = true;
        }
        if (GetComponent<SpriteRenderer>().color.g <= 0)
            isHit = false;
        yield return new WaitForSeconds(0.02f);
        StartCoroutine(Onhit());
    }

    void Lose()
    {
        main.GetComponent<Main>().Lose();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Key")
        {
            Destroy(collision.gameObject);
            key = true;
            inventari.Add_key();
        }

        if (collision.gameObject.tag == "Door")
        {
            if (collision.gameObject.GetComponent<Door>().isOpen && canTP)
            {
                collision.gameObject.GetComponent<Door>().teleport(gameObject);
                canTP = false;
                StartCoroutine(TPwait());
            }
            else if (key)
                collision.gameObject.GetComponent<Door>().Unlock();
        }

        if (collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            coins++;
            soundeff.PlayCoinSound();
            print("Монеты " + coins);
        }

        if (collision.gameObject.tag == "HP")
        {
            Destroy(collision.gameObject);
            //RecountHp(curHp++);
            inventari.Add_hp();
        }

        if (collision.gameObject.tag == "Mushroom")
        {
            Destroy(collision.gameObject);
            RecountHp(-1);
        }

        if (collision.gameObject.tag == "gemBlue")
        {
            Destroy(collision.gameObject);
            // StartCoroutine(NoHit());
            inventari.Add_bg();
        }

        if (collision.gameObject.tag == "gemGreen")
        {
            Destroy(collision.gameObject);
            // StartCoroutine(SpeedBonus());
            inventari.Add_gg();
        }

        if (collision.gameObject.tag == "TimerButtonStart")
        {
            insideTimer = 0f;
        }

        if (collision.gameObject.tag == "TimerButtonStop")
        {
            insideTimer = -1f;
            insideCountDown.fillAmount = 0f;
        }
    }

    IEnumerator TPwait()
    {
        yield return new WaitForSeconds(1f);
        canTP = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            isclimb = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if (Input.GetAxis("Vertical") == 0)
            {
                anim.SetInteger("state", 5);
            }
            else
            {
                anim.SetInteger("state", 6);
                transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 0.5f * Time.deltaTime);
            }
        }

        if (collision.gameObject.tag == "icy")
        {
            if (rb.gravityScale == 1f)
            {
                rb.gravityScale = 7f;
                speed *= 0.25f;
            }
        }

        if (collision.gameObject.tag == "Lava")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= 3f)
            {
                hitTimer = 0f;
                PlayerCountDown.fillAmount = 1f;
                RecountHp(-1);
            }
            else
                PlayerCountDown.fillAmount = 1 - (hitTimer / 3f);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            isclimb = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (collision.gameObject.tag == "icy")
        {
            if(rb.gravityScale == 7f)
                {
                rb.gravityScale = 1f;
                speed *= 4f; 
            }
        }

        if (collision.gameObject.tag == "Lava")
        {
            hitTimer = 0f;
            PlayerCountDown.fillAmount = 0f;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trampoline")
            StartCoroutine(TrampolineAnim(collision.gameObject.GetComponentInParent<Animator>()));
        if(collision.gameObject.tag == "qwek")
        {
            speed *= 0.25f;
            rb.mass *= 100f;
        }
    }
    IEnumerator TrampolineAnim(Animator an) {
        an.SetBool("isjump", true);
        yield return new WaitForSeconds(0.5f);
        an.SetBool("isjump", false);
    }

    IEnumerator NoHit()
    {
        gemCount++;
        blueGem.SetActive(true);
        CheckGems(blueGem);

        canHit = false;
        blueGem.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        print("Неуязвим");
        yield return new WaitForSeconds(4f);
        StartCoroutine(invis(blueGem.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);
        canHit = true;

        gemCount--;
        blueGem.SetActive(false);
        print("Уязвим");

        CheckGems(greenGem);
    }

    IEnumerator SpeedBonus ()
    {
        gemCount++;
        greenGem.SetActive(true);
        CheckGems(greenGem);

        speed = speed * 1.5f;
        greenGem.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        print("Скороть 2х");
        yield return new WaitForSeconds(9f);
        StartCoroutine(invis(greenGem.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);

        gemCount--;
        greenGem.SetActive(false);
        speed = speed - 3.5f;
        print("Скорость 1х");
    }

    void CheckGems(GameObject obj)
    {
        if (gemCount == 1)
            obj.transform.localPosition = new Vector3(0f, 0.6f, obj.transform.localPosition.z);
        else if (gemCount == 2)
        {
            blueGem.transform.localPosition = new Vector3(-0.4f, 0.6f, blueGem.transform.localPosition.z);
            greenGem.transform.localPosition = new Vector3(0.4f, 0.6f, greenGem.transform.localPosition.z);
        }
    }

    IEnumerator invis(SpriteRenderer spr, float time)
    {
        spr.color = new Color(1f, 1f, 1f, spr.color.a - time * 2);
        yield return new WaitForSeconds(time);
        if (spr.color.a > 0)
            StartCoroutine(invis(spr, time));
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "qwek")
        {
            speed *= 4;
            rb.mass *= 0.01f;
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    public int GetHP()
    {
        return curHp;
    }

    public void BlueGem()
    {
        StartCoroutine(NoHit());
    }

    public void GreenGem()
    {
        StartCoroutine(SpeedBonus());
    }

    public void Hp_Gem()
    {
        RecountHp(curHp++);
    }
}
