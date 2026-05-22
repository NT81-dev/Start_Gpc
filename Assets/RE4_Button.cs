using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class RE4Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    private AudioSource audioSource;
    public AudioClip hoverSound; // カーソルが重なった時の音
    public AudioClip clickSound; // 決定時の音

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // ボタンコンポーネントのクリックイベントに決定音を登録
        GetComponent<Button>().onClick.AddListener(() => {
            if (clickSound) audioSource.PlayOneShot(clickSound);
        });
    }

    // マウスが乗った時
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // もし自分が選ばれている状態なら、選択を解除して Normal に戻るようにする
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // コントローラーやキーボードで選択された時
    public void OnSelect(BaseEventData eventData)
    {
        PlayHoverSound();
    }

    private void PlayHoverSound()
    {
        if (hoverSound && audioSource)
            audioSource.PlayOneShot(hoverSound);
    }
}