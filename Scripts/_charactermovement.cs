using UnityEngine;

public class _charactermovement : MonoBehaviour
{
    public float speed = 5f;
    public float jump = 2f;
    void Start()
    {
        
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 curPos = transform.position;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector2 newPos = new Vector2(curPos.x + Time.deltaTime * speed, curPos.y);
            transform.position = newPos;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector2 newPos = new Vector2(curPos.x - Time.deltaTime * speed, curPos.y);
            transform.position = newPos;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 newPos = new Vector2(curPos.x, curPos.y + jump);
            transform.position = newPos;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vector2 newPos = new Vector2(curPos.x + Time.deltaTime * speed * 2f, curPos.y);
            transform.position = newPos;
        }
    }
}