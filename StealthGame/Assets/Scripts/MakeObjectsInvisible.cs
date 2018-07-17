using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeObjectsInvisible : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float radius = 1;
    [SerializeField] LayerMask layerMask = 0;
    [SerializeField] string objectTag = "Wall";

    private List<GameObject> previousHitObjects;


    private void Awake()
    {
        previousHitObjects = new List<GameObject>();
    }

    void LateUpdate()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        Vector3 direction = player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, dist, layerMask);

        Debug.DrawRay(transform.position, direction, Color.cyan);

        List<GameObject> hitObjects = new List<GameObject>();

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == objectTag)
            {
                GameObject hitObject = hit.collider.gameObject;

                hitObjects.Add(hitObject);

                Renderer renderer = hitObject.GetComponent<Renderer>();
                if (renderer)
                {
                    Color newColor = renderer.material.color;
                    newColor.a = 0.1f; // Set Alpha to 0.1f
                    renderer.material.color = newColor;
                }
            }

            // Get the game objects that are no longer in the sphere cast
            foreach (GameObject go in previousHitObjects)
            {
                if (!hitObjects.Contains(go))
                {
                    Renderer renderer = go.GetComponent<Renderer>();
                    if (renderer)
                    {
                        Color newColor = renderer.material.color;
                        newColor.a = 1; // Set alpha to 1
                        renderer.material.color = newColor;
                    }
                }
            }


            previousHitObjects = hitObjects;
        }
    }
}
