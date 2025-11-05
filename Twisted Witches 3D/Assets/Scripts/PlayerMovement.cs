using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Animator animator;

    [SerializeField] private Camera mainCam;
    private bool shift = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            shift = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            shift = false;
        }

        if (PauseController.IsGamePaused)
        {
            Debug.Log($"Game paused (PlayerMovement sees IsGamePaused={PauseController.IsGamePaused})");
            Vector3 stopMovement = new Vector3(0, 0, 0);
            AnimateMovement(stopMovement);
            return;
        }

        // get input in unity through Input class
        float horizontal = Input.GetAxisRaw("Horizontal");  // want to get input from horizontal access
        float vertical = Input.GetAxisRaw("Vertical");

        // Use verctor to add both x and y values to pos at same time
        Vector3 direction = new Vector3(horizontal, 0, vertical);

        AnimateMovement(direction);

        // Add direction to curr pos and multiply by speed and time
        // Access curr pos using transfrom (which is stored in Vector3)
        if (!shift)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        if (shift)
        {
            transform.position += direction * (speed * 2) * Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        // Get camera position
        Vector3 cameraPos = mainCam.transform.position;
        cameraPos.y = transform.position.y;

        // Make sprite face camera
        transform.LookAt(cameraPos);
        transform.Rotate(0f, 180f, 0f);
    }

    void AnimateMovement(Vector3 direction)
    {
        // Check to see if animator reference is set
        if (animator != null)
        {
            // Ceck if moving
            if (direction.magnitude > 0)  // will have magnitude = 0 if no input from player
            {
                animator.SetBool("isMoving", true);

                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.z);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    // Increase player speed for a certain amount of time
    public void SpeedPotion()
    {
        StartCoroutine(SpeedUp());
    }

    IEnumerator SpeedUp()
    {
        speed *= 2;
        yield return new WaitForSeconds(5f);
        speed /= 2;
    }
}
