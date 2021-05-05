using UnityEngine;
using UnityEngine.UI;


using System.Collections;


public class PlayerUI : MonoBehaviour
{
    private WizardMovement target;

    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 0f, 0f);

    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;

    public void SetTarget(WizardMovement _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        target = _target;
        if (gameObject.GetComponent<Text>().text != null)
        {
            gameObject.GetComponent<Text>().text = target.photonView.Owner.NickName;
        }

        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
        CharacterController characterController = _target.GetComponent<CharacterController>();
        // Get data from the Player that won't change during the lifetime of this Component
        if (characterController != null)
        {
            characterControllerHeight = characterController.height;
        }
    }

    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    void LateUpdate()
    {
        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }


        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset + new Vector3(80,-10,0);
            Debug.Log(this.transform.position);
        }
    }
}