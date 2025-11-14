using System.Collections;
using UnityEngine;

public class DamageRange : MonoBehaviour
{
    public GameObject mob;

    void Update()
    {
        if (mob.CompareTag("EvilClone"))
        {
            if (mob.GetComponent<EvilClone>().inRange)
            {
                //PlayerStats.Instance.DecreaseCurrHealth(mob.GetComponent<Slime>().damage);
                StartCoroutine(WaitForHit(5, "EvilClone"));
                mob.GetComponent<EvilClone>().inRange = false;
            }
        }
        else if (mob.CompareTag("Mob"))
        {
            if (mob.GetComponent<Slime>().inRange)
            {
                //PlayerStats.Instance.DecreaseCurrHealth(mob.GetComponent<Slime>().damage);
                StartCoroutine(WaitForHit(5));
                mob.GetComponent<Slime>().inRange = false;
            }
        }
    }

    IEnumerator WaitForHit(float duration, string tag = "Mob")
    {
        if (tag.Equals("EvilClone"))
        {
            PlayerStats.Instance.DecreaseCurrHealth(mob.GetComponent<EvilClone>().damage);
            Debug.Log($"Player health: {PlayerStats.Instance.GetCurrHealth()}.");
            yield return new WaitForSeconds(duration);
        }
        else
        {
            PlayerStats.Instance.DecreaseCurrHealth(mob.GetComponent<Slime>().damage);
            Debug.Log($"Player health: {PlayerStats.Instance.GetCurrHealth()}.");
            yield return new WaitForSeconds(duration);
        }
        
    }
}
