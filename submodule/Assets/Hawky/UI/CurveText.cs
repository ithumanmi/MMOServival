using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurveText : MonoBehaviour
{
    public float curveMultiplier = 10f; // Điều chỉnh độ cong của văn bản

    private void Start()
    {
        CurveTheText();
    }

    void CurveTheText()
    {
        TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.ForceMeshUpdate(); // Cập nhật mesh trước khi thực hiện thay đổi

        var textInfo = textMesh.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                // Áp dụng công thức cong chữ tại đây. Ví dụ: tạo hiệu ứng sóng
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(orig.x * 0.1f + Time.time) * curveMultiplier, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    void Update()
    {
        CurveTheText();
    }

    private void OnValidate()
    {
        CurveTheText();
    }
}
