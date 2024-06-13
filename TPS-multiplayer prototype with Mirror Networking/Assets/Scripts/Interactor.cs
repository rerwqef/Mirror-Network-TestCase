using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Interactor : NetworkBehaviour
{

    public float raycastDistance = 10.0f; // The distance of the raycast
    private Renderer ballRenderer; // The Renderer component of the ball
    private Camera mainCamera; // Reference to the main camera
    private Coroutine colorChangeCoroutine; // Reference to the color change coroutine

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
   /*     Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Create a ray from the center of the screen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Perform a raycast
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Interactable")) // Check if the object has the tag "Interactable"
            {
                Renderer hitRenderer = hit.collider.gameObject.GetComponent<Renderer>();
                if (ballRenderer != hitRenderer)
                {
                    if (colorChangeCoroutine != null)
                    {
                        StopCoroutine(colorChangeCoroutine);
                    }
                    ballRenderer = hitRenderer;
                    CmdStartColorChange(ballRenderer.gameObject);
                }
            }
            else
            {
                ResetColor();
            }
        }
        else
        {
            ResetColor();
        }

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.green); // For visualization in the editor
    }
    private void ResetColor()
    {
        if (ballRenderer != null)
        {
            if (colorChangeCoroutine != null)
            {
                StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = null;
            }
            CmdResetColor(ballRenderer.gameObject);
            ballRenderer = null;
        }
    }

    [Command]
    private void CmdStartColorChange(GameObject hitObject)
    {
        RpcStartColorChange(hitObject);
    }

    [ClientRpc]
    private void RpcStartColorChange(GameObject hitObject)
    {
        Renderer hitRenderer = hitObject.GetComponent<Renderer>();
        if (hitRenderer != null)
        {
            if (colorChangeCoroutine != null)
            {
                StopCoroutine(colorChangeCoroutine);
            }
            colorChangeCoroutine = StartCoroutine(ChangeColorContinuously(hitRenderer));
        }
    }

    private IEnumerator ChangeColorContinuously(Renderer renderer)
    {
        while (true)
        {
            CmdChangeColor(renderer.gameObject, GetRandomColor());
            yield return new WaitForSeconds(2f);
        }
    }

    [Command]
    private void CmdChangeColor(GameObject hitObject, Color newColor)
    {
        RpcChangeColor(hitObject, newColor);
    }

    [ClientRpc]
    private void RpcChangeColor(GameObject hitObject, Color color)
    {
        Renderer hitRenderer = hitObject.GetComponent<Renderer>();
        if (hitRenderer != null)
        {
            hitRenderer.material.color = color;
        }
    }

    [Command]
    private void CmdResetColor(GameObject hitObject)
    {
        RpcResetColor(hitObject);
    }

    [ClientRpc]
    private void RpcResetColor(GameObject hitObject)
    {
        Renderer hitRenderer = hitObject.GetComponent<Renderer>();
        if (hitRenderer != null)
        {
            hitRenderer.material.color = Color.white;
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value); // Generate a random color
    }
}