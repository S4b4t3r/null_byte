/*
    Reads the player's stats and displays it on UI.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDriver : MonoBehaviour
{
    static UIDriver uiInstance;   
    PlayerController player;

    // HP
    public GameObject hpSlotHolder;
    Animator hpshAnim;
    public GameObject hpSlotgo;
    List<GameObject> hpslots;
    List<ParticleSystem> hpslotsPs;
    float maxhp;
    float hp;
    // Mana
    public GameObject fireballBar;
    Animator fbbAnim;
    RectTransform fbBarRt;
    public GameObject fireballFill;
    RectTransform fbFillRt;

    void Awake()
    {
        // Make sure only one player instance exists
        if (uiInstance == null)
        {
            DontDestroyOnLoad(this);
            uiInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        fbBarRt = fireballBar.GetComponent<RectTransform>();
        fbbAnim = fireballBar.GetComponent<Animator>();
        fbFillRt = fireballFill.GetComponent<RectTransform>();
        
        hpslots = new List<GameObject>();
        hpslotsPs = new List<ParticleSystem>();
        hpshAnim = hpSlotHolder.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Max HP change
        if (player.maxhp != maxhp)
        {
            maxhp = player.maxhp;

            int slotsDiff = (int)maxhp - hpslots.Count;
            if (slotsDiff > 0)
            {
                for(int i = hpslots.Count; maxhp > i; i++)
                {
                    GameObject newSlot = Instantiate(hpSlotgo, hpSlotHolder.transform);
                    newSlot.transform.localPosition = new Vector3(16f*i, 0f, 0f);
                    hpslots.Add(newSlot);
                    hpslotsPs.Add(newSlot.GetComponentInChildren<ParticleSystem>());
                }
            } else if (slotsDiff < 0)
            {
                for(int i = hpslots.Count; maxhp < i; i--)
                {
                    Destroy(hpslots[i-1]);
                    hpslots.RemoveAt(i-1);
                    hpslotsPs.RemoveAt(i-1);
                }
            }
        }

        // HP change
        if (player.hp != hp)
        {
            hp = player.hp;

            for(int i = 0; hpslots.Count > i; i++)
            {
                var emission = hpslotsPs[i].emission;
                emission.rateOverTime = Mathf.Clamp(40*(hp - i), 0f, 40f);
            }

        }

    }

    public void HideUI()
    {
        fbbAnim.SetBool("Show", false);
        hpshAnim.SetBool("Show", false);
    }

    public void HideUIInstant()
    {
        fbbAnim.SetTrigger("HideInstant");
        fbbAnim.SetBool("Show", false);
        hpshAnim.SetTrigger("HideInstant");
        hpshAnim.SetBool("Show", false);
    }

    public void ShowUI()
    {
        fbbAnim.GetComponent<Animator>().SetBool("Show", true);
        hpshAnim.GetComponent<Animator>().SetBool("Show", true);
    }

    public void ShowUIInstant()
    {
        fbbAnim.SetTrigger("ShowInstant");
        fbbAnim.SetBool("Show", true);
        hpshAnim.SetTrigger("ShowInstant");
        hpshAnim.SetBool("Show", true);
    }
}
