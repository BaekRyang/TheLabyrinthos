using UnityEngine;
using UnityEngine.TextCore.Text;

public class SeeThrough : MonoBehaviour
{
    Renderer ObstacleRenderer;
    [SerializeField] Transform player;
    private int wallLayer;

    private void Awake()
    {
        wallLayer = 1 << LayerMask.NameToLayer("Wall");
    }

    void Update()

    {
        Debug.DrawLine(transform.position, player.position);
        float Distance = Vector3.Distance(transform.position, player.position);

        Vector3 Direction = (player.position - transform.position).normalized;

        RaycastHit[] hit = Physics.RaycastAll(transform.position, Direction, Distance, wallLayer);

        if (hit.Length > 0)

        {
            // 2.맞았으면 Renderer를 얻어온다.
            foreach (var item in hit)
            {
                ObstacleRenderer = item.collider.gameObject.GetComponentInChildren<MeshRenderer>();

                if (ObstacleRenderer != null)

                {
                    // 3. Metrial의 Aplha를 바꾼다.

                    Material Mat = ObstacleRenderer.material;

                    Color matColor = Mat.color;

                    matColor.a = 0.5f;

                    Mat.color = matColor;

                }
            }

        }
    }
}
