using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NewBehaviourScript : MonoBehaviour, IDragHandler
{
    [Header("連動させるパネル（RectTransformをアタッチ）")]
    public RectTransform leftPanel;
    public RectTransform rightPanel;

    [Header("シアン色の境界線（自分自身（Handle）のRectTransform）")]
    public RectTransform handleRect;

    private RectTransform parentRect;
    private float currentRatio = 0.7f; // 画面の分割比率（0.0 〜 1.0）

    void Start()
    {
        // 親のDetailPanelのRectTransformを取得
        parentRect = transform.parent.GetComponent<RectTransform>();
        if (parentRect == null)
        {
            parentRect = leftPanel.parent.GetComponent<RectTransform>();
        }

        // 実行した瞬間のエディタ上のアンカー位置（0.7など）を自動で読み取って基準にする
        if (handleRect != null)
        {
            currentRatio = handleRect.anchorMin.x;
        }

        // 画面の大きさを同期
        UpdateLayout();
    }

    // 中央の線がドラッグされた時にUnityが正確に呼び出す関数
    public void OnDrag(PointerEventData eventData)
    {
        if (parentRect == null) return;

        float totalWidth = parentRect.rect.width;
        if (totalWidth <= 0) return;

        // 1. マウスが横に動いた「ピクセル数」を、全体の比率（0.0〜1.0）に変換
        float deltaRatio = eventData.delta.x / totalWidth;

        // 2. 現在の比率に加算し、0.0〜1.0の範囲に制限
        currentRatio = Mathf.Clamp(currentRatio + deltaRatio, 0.0f, 1.0f);

        // 3. 画面とHandleのアンカー位置を再計算
        UpdateLayout();
    }

    void UpdateLayout()
    {
        if (parentRect == null || leftPanel == null || rightPanel == null || handleRect == null) return;

        float totalWidth = parentRect.rect.width;

        // 比率から、左右の正確なピクセル幅を計算
        float leftWidth = totalWidth * currentRatio;
        float rightWidth = totalWidth * (1.0f - currentRatio);

        // 左右のパネルの横幅をリアルタイムに変更
        leftPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, leftWidth);
        rightPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rightWidth);

        // 🔥 Handle自身のアンカー（基準線）を、現在の分割比率（currentRatio）に上書きする
        handleRect.anchorMin = new Vector2(currentRatio, 0);
        handleRect.anchorMax = new Vector2(currentRatio, 1);
        
        // アンカー自体が動くので、位置のズレ（Offset）は常に0（ぴったり中央）で固定
        handleRect.anchoredPosition = Vector2.zero;
    }
}