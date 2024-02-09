using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

using FinksQuest.Core;

namespace FinksQuest.UI
{
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
            _audioSource = _dialogPanel.GetComponent<AudioSource>();
            _textMeshPro = GameManager.Instance._textDialog;
        }

        private void OnEnable()
        {
            if (_message != "")
            {
                // On désactive les controles du joueur
                GameManager.Instance.Player._playerInput.SwitchCurrentActionMap("UI");

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
                string device = GameManager.Instance.Player._playerInput.currentControlScheme;

                // On cherche dans le _message toutes les occurences [...] via une Regex
                string pattern = @"\[(.*?)\]";

                foreach (Match match in Regex.Matches(_message, pattern, RegexOptions.IgnoreCase))
                {
                    InputAction action = GameManager.Instance.Player._playerInput.actions.FindAction(match.Value.Replace("[", "").Replace("]", ""));

                    if (action == null)
                    {
                        Debug.LogError("Action " + match.Value.Replace("[", "").Replace("]", "") + " not found !");
                        continue;
                    }

                    var bindingsString = new StringBuilder();
                    var bindingForAction = new ArrayList();

                    foreach (var binding in action.bindings)
                    {
                        if (binding.groups.Contains(device))
                        {
                            bindingForAction.Add(binding);
                        }
                    }

                    for (int i = 0; i < bindingForAction.Count; i++)
                    {
                        InputBinding binding = (InputBinding)bindingForAction[i];
                        bindingsString.Append("[");
                        // On récupère le nom de la touche
                        if (binding.isPartOfComposite)
                        {
                            // On récupère le nom du composite
                            string compositeName = binding.ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
                            bindingsString.Append(compositeName);
                        }
                        else
                        {
                            // On récupère le nom de la touche
                            string compositeName = binding.ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
                            bindingsString.Append(compositeName);
                        }
                        bindingsString.Append("]");

                        if (i < bindingForAction.Count - 1)
                        {
                            bindingsString.Append(" / ");
                        }
                    }

                    var displayString = bindingsString.ToString();

                    // On met le texte en italique
                    displayString = "<i>" + displayString + "</i>";

                    _message = _message.Replace(match.Value, displayString);
                }

                GameManager.Instance.Player.GetAnimator().SetBool("isRunning", false);

                StartCoroutine(ShowDialog());
            }
        }

        private void Update()
        {
            // Si panel actif, que l'animation du texte est finie et qu'une touche (n'importe laquelle) est pressée
            if (_panelActivated && _animationFinished && Input.anyKeyDown)
            {
                // On disable le panel
                _dialogPanel.gameObject.SetActive(false);
                _panelActivated = false;

                // On réactive le joueur
                GameManager.Instance.Player._playerInput.SwitchCurrentActionMap("Player");

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
                // Si c'est le début d'une balise, on veut savoir ce que c'est pour ne pas l'afficher
                else if (_message[currentIndex].Equals("<"))
                {
                    // On cherche la fin de la balise
                    int endTagIndex = _message.IndexOf(">", currentIndex);

                    // On récupère la balise
                    string tag = _message.Substring(currentIndex, endTagIndex - currentIndex + 1);

                    // On cherche si c'est une balise de fin
                    if (tag.Contains("</"))
                    {
                        // On cherche la balise de début
                        int startTagIndex = _message.LastIndexOf("<", currentIndex);

                        // On récupère la balise
                        string startTag = _message.Substring(startTagIndex, currentIndex - startTagIndex + 1);

                        // On cherche si c'est une balise de fin
                        if (startTag.Contains("<i>"))
                        {
                            _audioSource.volume = 1;
                        }
                    }
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
}