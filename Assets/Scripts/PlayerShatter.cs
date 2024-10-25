using UnityEngine;

public class PlayerShatter : MonoBehaviour
{
    public int capsuleFragments = 10;
    public int cubeFragments = 5;
    public float explosionForce = 10f;
    public float explosionRadius = 1.0f;
    public float fadeTime = 2f;
    public Color capsuleColor = Color.white;
    public Color cubeColor = Color.white;
    public float fragmentSize = 0.1f;

    void Start()
    {
        Shatter();
    }

    void Shatter()
    {
        Vector3 center = transform.position;

        // 胶囊体碎片
        ShatterCapsule(center);

        // 立方体碎片
        ShatterCube(center);

        Destroy(gameObject, fadeTime);
    }

    void ShatterCapsule(Vector3 center)
    {
        for (int i = 0; i < capsuleFragments; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float height = Random.Range(-0.5f, 0.5f);
            Vector3 position = center + new Vector3(Mathf.Cos(angle) * 0.5f, height, Mathf.Sin(angle) * 0.5f);
            CreateFragment(position, PrimitiveType.Sphere, capsuleColor);
        }
    }

    void ShatterCube(Vector3 center)
    {
        float offset = fragmentSize * (cubeFragments - 1) / 2f;
        for (int x = 0; x < cubeFragments; x++)
        {
            for (int y = 0; y < cubeFragments; y++)
            {
                for (int z = 0; z < cubeFragments; z++)
                {
                    Vector3 position = new Vector3(
                        center.x - offset + x * fragmentSize,
                        center.y + 1f - offset + y * fragmentSize, // 假设立方体在胶囊体上方
                        center.z - offset + z * fragmentSize
                    );
                    CreateFragment(position, PrimitiveType.Cube, cubeColor);
                }
            }
        }
    }

    void CreateFragment(Vector3 position, PrimitiveType type, Color color)
    {
        GameObject fragment = GameObject.CreatePrimitive(type);
        fragment.transform.localScale = Vector3.one * fragmentSize;
        fragment.transform.position = position;

        Renderer fragmentRenderer = fragment.GetComponent<Renderer>();
        fragmentRenderer.material = new Material(Shader.Find("Standard"));
        fragmentRenderer.material.color = color;

        Rigidbody rb = fragment.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

        //Debug.Log($"Created {type} fragment at position: {position}, Size: {fragmentSize}");
        //Debug.DrawLine(position, position + Vector3.up * 0.5f, Color.yellow, 5f);

        Destroy(fragment, fadeTime);
    }
}
