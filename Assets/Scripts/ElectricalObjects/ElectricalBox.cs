using UnityEngine;

public class ElectricalBox : Electrical
{ 
    private MeshRenderer meshRenderer;

    [Header("Electrical Box Variables")]
    public Color deactivatedColor;
    public Color activatedColor;

    public float sparkleSize = 5; 

    private GameObject interactionParticle;
    private ObjectPooler.Key interactionKey = ObjectPooler.Key.InteractionParticle;


    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        interactionParticle = ObjectPooler.GetPooler(interactionKey).GetPooledObject();
        interactionParticle.transform.position = transform.position;
        interactionParticle.transform.rotation = transform.rotation;

        interactionParticle.transform.parent = gameObject.transform;

        interactionParticle.transform.localScale = new Vector3(transform.localScale.x + sparkleSize, transform.localScale.y + sparkleSize, transform.localScale.z + sparkleSize);
        interactionParticle.SetActive(true);
    }

    private void Update()
    {
        if (IsPowered())
        {
            if (meshRenderer.material.color != activatedColor)
            {
                meshRenderer.material.color = activatedColor;
            }
        }
        else
        {
            if (meshRenderer.material.color != deactivatedColor)
            {
                meshRenderer.material.color = deactivatedColor;
            }
        }
    }
}