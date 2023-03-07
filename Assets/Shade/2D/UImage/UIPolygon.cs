/// Credit CiaccoDavide
/// Sourced from - http://ciaccodavi.de/unity/uipolygon

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
    public class UIPolygon : MaskableGraphic
    {
        [SerializeField]
        Texture m_Texture;
        public bool fill = true;
        //public bool crosswise=true;
        public bool centerwise=true;
        public float thickness = 5;
        [Range(3, 360)]
        public int sides = 3;
        [Range(0, 360)]
        public float rotation = 0;
        [Range(0, 1)]
        public float[] VerticesDistances = new float[3];
        private float size = 0;

        public override Texture mainTexture
        {
            get
            {
                return m_Texture == null ? s_WhiteTexture : m_Texture;
            }
        }
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value) return;
                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }
        public void DrawPolygon(int _sides)
        {
            sides = _sides;
            VerticesDistances = new float[_sides + 1];
            for (int i = 0; i < _sides; i++) VerticesDistances[i] = 1; ;
            rotation = 0;
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = 0;
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = _rotation;
        }
        void Update()
        {
            size = rectTransform.rect.width;
            if (rectTransform.rect.width > rectTransform.rect.height)
                size = rectTransform.rect.height;
            else
                size = rectTransform.rect.width;
            thickness = (float)Mathf.Clamp(thickness, 0, size / 2);
        }
        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            Vector2 uv4 = new Vector2(0.5f, 0.5f);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;
            Vector2 pos4;
            Vector2 pos5;
            Vector2 pos6;
            Vector2 pos7;
            float degrees = 360f / sides;
            int vertices = sides + 1;
            if (VerticesDistances.Length != vertices)
            {
                VerticesDistances = new float[vertices];
                for (int i = 0; i < vertices - 1; i++) VerticesDistances[i] = 1;
            }
            // last vertex is also the first!
            VerticesDistances[vertices - 1] = VerticesDistances[0];
            float factor=2;
            for (int i = 0; i < vertices; i++)
            {
                float outer = -rectTransform.pivot.x * size * VerticesDistances[i];
                float outer1 = -rectTransform.pivot.x * size * VerticesDistances[i]+ thickness/factor;

                float inner = -rectTransform.pivot.x * size * VerticesDistances[i] + thickness;

                // float co = -rectTransform.pivot.x*size*VerticesDistances[i]*0.5f;
                // float ci = -rectTransform.pivot.x*size*VerticesDistances[i]*0.5f+thickness;

                float rad = Mathf.Deg2Rad * (i * degrees + rotation);
                float rad1 = Mathf.Deg2Rad * (i * degrees + rotation-thickness/Mathf.PI);
                float rad3 = Mathf.Deg2Rad * (i * degrees + rotation+thickness/Mathf.PI);
                float rad2 = Mathf.Deg2Rad * (i * degrees + rotation+90);
                float rad4 = Mathf.Deg2Rad * (i * degrees + rotation+270);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);

                pos0 = prevX;
                pos1 = new Vector2(outer * c, outer * s);
                pos4 = new Vector2(outer1 * Mathf.Cos(rad1), outer1 * Mathf.Sin(rad1));
                pos7 = new Vector2(outer1 * Mathf.Cos(rad3), outer1 * Mathf.Sin(rad3));
                if (fill)
                {
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                    prevX = pos1;
                    prevY = pos2;
                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
                }
                else
                {
                    pos2 = new Vector2(inner * c, inner * s);
                    pos3 = prevY;
                    float radius=thickness/factor;
                    pos5 = new Vector2(radius * Mathf.Cos(rad2), radius * Mathf.Sin(rad2));
                    pos6 = new Vector2(radius * Mathf.Cos(rad4), radius * Mathf.Sin(rad4));
                    //Debug.Log(pos0+":"+pos1+":"+pos2+":"+pos3);
                    prevX = pos1;
                    prevY = pos2;
                    vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
                    if(centerwise)
                        vh.AddUIVertexQuad(SetVbo(new[] { pos4, pos7, pos6, pos5 }, new[] { uv0, uv1, uv2, uv3 }));
                }
                
            }
        }
    }
}
