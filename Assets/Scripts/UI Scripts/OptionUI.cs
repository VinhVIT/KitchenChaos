using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider soundEffectSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button MUButton;
    [SerializeField] private Button MDButton;
    [SerializeField] private Button MLButton;
    [SerializeField] private Button MRButton;
    [SerializeField] private Button PUButton;
    [SerializeField] private Button CutButton;
    [SerializeField] private Button PauseButton;
    [SerializeField] private TextMeshProUGUI MUText;
    [SerializeField] private TextMeshProUGUI MDText;
    [SerializeField] private TextMeshProUGUI MLText;
    [SerializeField] private TextMeshProUGUI MRText;
    [SerializeField] private TextMeshProUGUI PUText;
    [SerializeField] private TextMeshProUGUI CutText;
    [SerializeField] private TextMeshProUGUI PauseText;
    [SerializeField] private Transform pressAKeyToRebindTransform;

    private void Awake()
    {
        MUButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveUp); });
        MDButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveDown); });
        MLButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveLeft); });
        MRButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveRight); });
        PUButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.PickUp); });
        CutButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Cut); });
        PauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
    }
    private void Start()
    {
        soundEffectSlider.value = SoundManager.Instance.GetVolumeMultiply();
        musicSlider.value = MusicManager.Instance.GetMusicVolume();

        GameManager.Instance.OnLocalGameUnPaused += GameManager_OnGameUnPaused;

        UpdateVisual();
        HidePressAKeyToReBind();
        Hide();
    }

    private void GameManager_OnGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void Update()
    {
        SoundManager.Instance.ChangeVolume(soundEffectSlider.value);
        MusicManager.Instance.ChangeMusicVolume(musicSlider.value);
    }
    private void UpdateVisual()
    {
        MUText.text = GameInput.Instance.GetBindingInputText(GameInput.Binding.MoveUp);
        MDText.text = GameInput.Instance.GetBindingInputText(GameInput.Binding.MoveDown);
        MLText.text = GameInput.Instance.GetBindingInputText(GameInput.Binding.MoveLeft);
        MRText.text = GameInput.Instance.GetBindingInputText(GameInput.Binding.MoveRight);
        PUText.text = GameInput.Instance.GetBindingInputText(GameInput.Binding.PickUp);
        CutText.text = GameInput.Instance.GetBindingInputText(GameInput.Binding.Cut);
        PauseText.text = GameInput.Instance.GetBindingInputText(GameInput.Binding.Pause);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private void ShowPressAKeyToReBind()
    {
        pressAKeyToRebindTransform.gameObject.SetActive(true);
    }
    private void HidePressAKeyToReBind()
    {
        pressAKeyToRebindTransform.gameObject.SetActive(false);
    }
    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressAKeyToReBind();
        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressAKeyToReBind();
            UpdateVisual();
        });
    }
}
