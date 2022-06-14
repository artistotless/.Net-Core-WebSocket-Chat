using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIEventer : MonoBehaviour
{
    public static event UnityAction<string> MessageEnterEvent;

    [SerializeField]
    private InputField _messageField;

    [SerializeField]
    private Button _sendBtn;

    void Start()
    {
        _sendBtn.onClick.AddListener(() =>
        {
            MessageEnterEvent(_messageField.text);
        });
    }

}