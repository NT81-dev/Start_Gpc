using UnityEngine;
using UnityEngine.UI;

public class UIWaveformRenderer : Graphic
{
    [Header("基本設定")]
    public int points = 120;         // 固定波形なので、点を多めにするとトゲが綺麗に出ます
    public float thickness = 3f;
    public float amplitude = 60f;

    [Header("スキャン（走査）設定")]
    public float scanSpeed = 0.5f;   // 1秒間に画面をどれくらい進むか（0.5なら2秒で1往復）
    public float tailLength = 0.3f;  // 残像の長さ（0.0 〜 1.0 の割合。0.3なら画面の3割の長さの残像）

    private float m_ScanProgress = 0f; // 現在のスキャン位置（0.0：左端 〜 1.0：右端）

    void Update()
    {
        // スキャン位置を 0.0 〜 1.0 の間でループさせる
        m_ScanProgress += Time.deltaTime * scanSpeed;
        if (m_ScanProgress > 1f)
        {
            m_ScanProgress -= 1f;
        }

        SetAllDirty(); // 毎フレーム再描画
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;
        float width = rect.width;

        if (points < 3) return;

        for (int i = 0; i < points; i++)
        {
            // 0.0 (左端) 〜 1.0 (右端)
            float progress = (float)i / (points - 1);
            
            // 1. 位置（X, Y）の計算 ➔ 完全にその場に固定！
            float x = rect.xMin + progress * width;
            
            // 心電図のトゲの位置を画面の固定位置に設定（今回は例として画面内に3つトゲを固定）
            float y = CalculateFixedECG(progress); 

            // 2. 💡最重要：スキャン位置に基づいた「透明度（残像）」の計算
            float alpha = 0f;

            // スキャン線より後ろ（過去）の点の場合
            if (progress <= m_ScanProgress)
            {
                // スキャン線に近いほど 1.0（明るい）、離れるほど 0.0（消える）
                float distance = m_ScanProgress - progress;
                alpha = Mathf.Max(0f, 1f - (distance / tailLength));
            }
            // スキャン線が1周して右端から左端に戻った時の、前回の残像の処理
            else
            {
                float distance = (m_ScanProgress + 1f) - progress;
                alpha = Mathf.Max(0f, 1f - (distance / tailLength));
            }

            // 先端（スキャン線のまさにその場所）だけ、さらに強く光らせる演出
            if (Mathf.Abs(progress - m_ScanProgress) < 0.01f)
            {
                alpha = 1f; 
            }

            // 色を適用
            Color vertexColor = color;
            vertexColor.a = alpha;

            // 頂点登録
            UIVertex v = UIVertex.simpleVert;
            v.color = vertexColor;

            v.position = new Vector2(x, y - thickness / 2);
            vh.AddVert(v);
            v.position = new Vector2(x, y + thickness / 2);
            vh.AddVert(v);

            if (i > 0)
            {
                int n = i * 2;
                vh.AddTriangle(n - 2, n - 1, n);
                vh.AddTriangle(n - 1, n + 1, n);
            }
        }
    }

    // 画面の特定の「場所（progress）」に心電図のトゲを配置する関数
    // 画面の特定の「場所（progress）」に心電図のトゲを配置する関数
    float CalculateFixedECG(float progress)
    {
        // progressは 0.0（左端）〜 1.0（右端）
        float h = 0f;

        // トゲ1つあたりの横幅（0.12 = 画面の12%の幅を使って1つのトゲを描く）
        float waveWidth = 0.12f; 

        // 1つ目のトゲ（左側：0.15の地点からスタート）
        if (progress > 0.15f && progress < 0.15f + waveWidth)
        {
            float t = (progress - 0.15f) / waveWidth; // 0.0 〜 1.0 に正規化
            h = Mathf.Sin(t * Mathf.PI * 2f) * amplitude;
        }
        
        // 2つ目のトゲ（中央：0.45の地点からスタート）
        else if (progress > 0.45f && progress < 0.45f + waveWidth)
        {
            float t = (progress - 0.45f) / waveWidth;
            h = Mathf.Sin(t * Mathf.PI * 2f) * (amplitude * 1.1f);
        }

        // 3つ目のトゲ（右側：0.75の地点からスタート）
        else if (progress > 0.75f && progress < 0.75f + waveWidth)
        {
            float t = (progress - 0.75f) / waveWidth;
            h = Mathf.Sin(t * Mathf.PI * 2f) * (amplitude * 0.9f);
        }

        // 💡 隠し味：トゲがない平坦な部分（hが0のとき）にも、
        // 医療モニター特有の微小な「ざわざわしたノイズ」を常に乗せる
        if (Mathf.Abs(h) < 0.01f)
        {
            h = (Mathf.PerlinNoise(progress * 40f, Time.time * 0.5f) - 0.5f) * 2f;
        }
        else
        {
            // トゲの部分にも少しノイズをブレンドしてリアルに
            h += (Mathf.PerlinNoise(progress * 20f, 0f) - 0.5f) * 3f;
        }

        return h;
    }
}