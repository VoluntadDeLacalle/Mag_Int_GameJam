using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public TMPro.TextMeshProUGUI itemHighlight;
    public Transform pickupTransform;
    public float pickupRadius = 3;
    public LayerMask playerMask;
    public List<Transform> raycastOrigins = new List<Transform>();
    [Header("Purely Gizmo Variables")]
    [Range(0, 1)]
    public float raycastOriginRadius = 0;
    
    private Dictionary<float, Ray> currentRaycasts = new Dictionary<float, Ray>();
    private List<Item> currentItemsInRange = new List<Item>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pickupTransform.position, pickupRadius);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        for (int i = 0; i < raycastOrigins.Count; i++)
        {
            Gizmos.DrawSphere(raycastOrigins[i].position, raycastOriginRadius);
        }

        foreach (KeyValuePair<float, Ray> currentPair in currentRaycasts)
        {
            Gizmos.DrawRay(currentPair.Value.origin, currentPair.Value.direction * currentPair.Key);
        }
    }

    void AddItemInRange(Item tempItem, GameObject otherGameObject)
    {
        if (tempItem.isEquipped != true && tempItem.gameObject.transform.root.GetComponent<Player>() == null)
        {
            if (!currentItemsInRange.Contains(otherGameObject.GetComponent<Item>()))
            {
                currentItemsInRange.Add(otherGameObject.GetComponent<Item>());
            }
        }
        else if (tempItem.isEquipped && tempItem.itemType == Item.TypeTag.grip && tempItem.gameObject.transform.root.GetComponent<Player>() == null)
        {
            if (!currentItemsInRange.Contains(otherGameObject.GetComponent<Item>()))
            {
                currentItemsInRange.Add(otherGameObject.GetComponent<Item>());
            }
        }
    }

    void CheckItemsInRange()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(pickupTransform.position, pickupRadius);
        currentRaycasts.Clear();
        currentItemsInRange.Clear();

        for (int i = 0; i < collidersInRange.Length; i++)
        {
            Item tempItem = null;
            tempItem = collidersInRange[i].gameObject.GetComponent<Item>();
            if(tempItem != null)
            {
                for (int j = 0; j < raycastOrigins.Count; j++)
                {
                    Ray currentRay = new Ray(raycastOrigins[j].position, (tempItem.gameObject.transform.position - raycastOrigins[j].position).normalized);
                    float distance = Vector3.Distance(raycastOrigins[j].position, tempItem.gameObject.transform.position);

                    LayerMask invertedPlayerMask = ~playerMask;
                    RaycastHit hitInfo;

                    if (Physics.Raycast(currentRay, out hitInfo, distance, invertedPlayerMask))
                    {
                        if (hitInfo.collider != collidersInRange[i])
                        {
                            continue;
                        }

                        currentRaycasts.Add(hitInfo.distance, currentRay);
                        AddItemInRange(tempItem, collidersInRange[i].gameObject);
                        break;
                    }
                }
            }
        }
    }

    private void Update()
    {
        CheckItemsInRange();


        if (itemHighlight == null || !Player.Instance.IsAlive())
        {
            return;
        }

        if (currentItemsInRange.Count > 0 && itemHighlight.text == "")
        {
            itemHighlight.text = "Press 'E' to pick up item!";
        }
        else if (currentItemsInRange.Count == 0 && itemHighlight.text != "")
        {
            itemHighlight.text = "";
        }

        if (currentItemsInRange.Count > 0)
        {
            if (!Player.Instance.vThirdPersonInput.CanMove())
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Time.timeScale != 0.0f || Player.Instance.vThirdPersonInput.CanMove())
                {
                    PickupItem();
                    Player.Instance.anim.SetTrigger("PickupTrigger");
                }
            }
        }
    }

    public void PickupItem()
    {
        int randNumb = Random.Range(0, currentItemsInRange.Count);
        Item tempItem = currentItemsInRange[randNumb];

        Vector3 itemDir = (tempItem.gameObject.transform.position - Player.Instance.transform.position).normalized;
        itemDir.y = 0;
        Player.Instance.transform.rotation = Quaternion.LookRotation(itemDir);

        if (tempItem.isEquipped == true && tempItem.itemType == Item.TypeTag.grip)
        {
            Inventory.Instance.AddToInventory(tempItem.GetComponentInChildren<ChassisItem>());
        }
        else
        {
            Inventory.Instance.AddToInventory(tempItem);

        }
        currentItemsInRange.RemoveAt(randNumb);
    }
}
