using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogBox : MonoBehaviour
{
    [TextArea(3, 10)] public string _message;
    [SerializeField] private float _displaySpeed = 3f;
    [SerializeField] private float _panelSpeed = 0.3f;
    [SerializeField] AnimationCurve _panelAnimationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);


    private RectTransform _dialogPanel;
    private TextMeshProUGUI _textMeshPro;

    private string _currentText;
    private int currentIndex = 0;
    private bool _panelActivated = false;
    private bool _animationFinished = false;
    [SerializeField] private bool _freezeTime;

    private AudioSource _audioSource;

    private void Awake()
    {
        _dialogPanel = GameManager.Instance._dialogPanel;
        _textMeshPro = GameManager.Instance._textDialog;
        _audioSource = _dialogPanel.GetComponent<AudioSource>();
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        Debug.Log("DialogBox enabled");
        if (_message != "")
        {
            // On désactive les controles du joueur
            GameManager.Instance._playerInput.SwitchCurrentActionMap("UI");

            if (_freezeTime)
            {
                // Temps de jeu à 0
                Time.timeScale = 0f;
            }

            _currentText = "";

            if (_textMeshPro != null)
            {
                _textMeshPro.text = _currentText;
            }

            if (_dialogPanel != null)
            {
                _dialogPanel.anchoredPosition = new Vector2(0f, -_dialogPanel.rect.height);
                _dialogPanel.gameObject.SetActive(true);
            }

            // On récupère le device actuel
            string device = GameManager.Instance._playerInput.currentControlScheme;

            InputAction action = GameManager.Instance._playerInput.actions.FindAction("Activate");
            int bindingIndex = action.GetBindingIndex(group: device);
            var displayString = action.GetBindingDisplayString(bindingIndex, out string deviceLayoutName, out string controlPath);

            // Replace RMB by right click
            // displayString = displayString.Replace("RMB", "Right Mouse Button");

            // On remplace le texte de tutoriel par le bon [ACTIVATE] devient activateKey
            string textToDisplay = _message.Replace("[ACTIVATE]", "[" + displayString + "]");

            _message = textToDisplay;

            GameManager.Instance._playerController._playerAnimator.SetBool("isRunning", false);

            StartCoroutine(ShowDialog());
        }
    }

    private void Update()
    {
        // Si panel actif, que l'animation du texte est finie et qu'une touche (n'importe laquelle) est pressée
        if (_panelActivated && _animationFinished && Input.anyKeyDown)
        {
            Debug.Log("Panel désactivé & apppuie sur une touche");
            // On disable le panel
            _dialogPanel.gameObject.SetActive(false);
            _panelActivated = false;

            // On réactive le joueur
            GameManager.Instance._playerInput.SwitchCurrentActionMap("Player");

            if (_freezeTime)
            {
                // Temps de jeu à 1
                Time.timeScale = 1f;
            }
        }
    }

    private IEnumerator ShowDialog()
    {
        float elapsedtime = 0f;

        // Affichage du panel
        while (elapsedtime < 0.3f)
        {
            elapsedtime += Time.deltaTime;
            float newY = Mathf.Lerp(_dialogPanel.anchoredPosition.y, 72f, _panelAnimationCurve.Evaluate(elapsedtime / _panelSpeed));
            _dialogPanel.anchoredPosition = new Vector2(0f, newY);

            yield return null;
        }
        _panelActivated = true;

        // Affichage du texte
        elapsedtime = 0f;

        float durationByLetter = _displaySpeed / _message.Length;

        // On joue le son de scroll
        _audioSource.Play();

        while (currentIndex < _message.Length)
        {
            if (_message[currentIndex].Equals(" "))
            {
                _audioSource.volume = 0;
            }
            _currentText += _message[currentIndex];
            _textMeshPro.text = _currentText;

            currentIndex += 1;

            yield return new WaitForSeconds(durationByLetter);

            _audioSource.volume = 1;

            elapsedtime += Time.deltaTime;

            yield return null;
        }

        // On joue le son de scroll
        _audioSource.Stop();

        _animationFinished = true;
    }

}