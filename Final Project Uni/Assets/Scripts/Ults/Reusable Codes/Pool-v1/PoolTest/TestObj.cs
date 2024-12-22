using UnityEngine;

public class TestObj : MonoBehaviour
{
    public float t = 2;
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        dir = new Vector3(1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (t <= 0)
        {
            dir = dir * -1;
            t = 2;
        }
        else
        {
            t -= Time.deltaTime;
        }
        moveCharacter(dir);

    }
    void moveCharacter(Vector3 direction)
    {
        transform.position += direction * 10 * Time.deltaTime;
    }
}
