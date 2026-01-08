using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class AlembicPlayer : MonoBehaviour
{
    public AlembicStreamPlayer alembic;
    public float duration = 0.958f; // смотри Time Range → to
    public float speed = 1f;

    private float time;

    void Update()
    {
        time += Time.deltaTime * speed;

        if (time > duration)
            time = 0f;

        alembic.CurrentTime = time;
    }
}
