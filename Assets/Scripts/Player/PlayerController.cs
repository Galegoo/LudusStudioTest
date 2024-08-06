using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool isMoving;
    [SerializeField] private Vector2 input;
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask NPCLayer;
    [SerializeField] static bool npcWasTouched;
    [SerializeField] static bool limitedMovment;
    [SerializeField] static bool playerIsLeft;
    [SerializeField] static bool playerIsRight;
    [SerializeField] static bool playerIsUp;
    [SerializeField] static bool playerIsDown;


    private Animator animatorController;
    public Animator[] equipmentAnimatorController;

    private void Awake()
    {
        animatorController = GetComponent<Animator>();
        npcWasTouched = false;
        limitedMovment = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving && !limitedMovment)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                
                animatorController.SetFloat("moveX", input.x);
                animatorController.SetFloat("moveY", input.y);
                if (input.x == 1)
                {
                    playerIsRight = true;
                    playerIsUp = false;
                    playerIsLeft = false;
                    playerIsDown = false;
                }
                else if (input.x == -1)
                {
                    playerIsRight = false;
                    playerIsUp = false;
                    playerIsLeft = true;
                    playerIsDown = false;
                }
                if (input.y == 1)
                {
                    playerIsRight = false;
                    playerIsUp = true;
                    playerIsLeft = false;
                    playerIsDown = false;
                }
                else if (input.y == -1)
                {
                    playerIsRight = false;
                    playerIsUp = false;
                    playerIsLeft = false;
                    playerIsDown = true;
                }
                for (int i = 0; i < equipmentAnimatorController.Length; i++)
                {
                    if (equipmentAnimatorController[i].gameObject.activeSelf)
                    {
                        equipmentAnimatorController[i].SetFloat("moveX", input.x);
                        equipmentAnimatorController[i].SetFloat("moveY", input.y);
                    }
                }
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                    if (isWalkable(targetPos)) // check if the player is touching any fourniture or blocked area
                        StartCoroutine(Move(targetPos)); // moves the player
            }
        }
        animatorController.SetBool("isMoving", isMoving); //says to the player it is moving
       
        
        for (int i = 0; i < equipmentAnimatorController.Length; i++) //say to all equipment it is moving
        {
            equipmentAnimatorController[i].SetBool("isMoving", isMoving);
        }

        isNPC(); //check if the player is in front of the NPC
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
    private bool isWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos,0.3f,solidObjectsLayer)!= null)
        {
            return false;
        }
        return true;
    }

   private void isNPC()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.001f, NPCLayer) != null)
        {
            npcWasTouched = true;          
        }
        else
        {
            npcWasTouched = false;
        }
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = GetComponent<Renderer>().material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            GetComponent<Renderer>().material.color = newColor;
            transform.GetChild(0).GetComponent<Renderer>().material.color = newColor;
            transform.GetChild(1).GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Store");
    }

    public static bool GetNpcWasTouched()
    {
        return npcWasTouched;
    }
    public static void changeLimitMovmentStatus(bool state)
    {
        limitedMovment = state;
    }

    public static int CheckInputX()
    {
        if (playerIsRight)
            return 1;
        else if (playerIsLeft)
            return -1;
        else
            return 0;
    }

    public static int CheckInputY()
    {
        if (playerIsUp)
            return 1;
        else if (playerIsDown)
            return -1;
        else 
            return 0;
    }
}
