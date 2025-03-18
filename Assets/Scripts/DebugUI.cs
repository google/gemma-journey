using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;


namespace GoogleDeepMind.GemmaSampleGame
{
  public class DebugUI : MonoBehaviour
  {
    [SerializeField]
    private Text _currentSecretWordText;

    private SecretManager _secretManager;
    [Inject]
    public void Construct(SecretManager secretManager)
    {
      _secretManager = secretManager;
    }
    // Start is called before the first frame update
    void Start()
    {
      if (_currentSecretWordText != null)
      {
        _currentSecretWordText.text = "[]";
      }
    }

    // Update is called once per frame
    void Update()
    {
      if (_currentSecretWordText != null)
      {
        _currentSecretWordText.text = _secretManager.ToString();
      }
    }

    public void Open()
    {
      if (_currentSecretWordText != null)
      {
        _currentSecretWordText.gameObject.SetActive(true);
      }
    }
    public void Close()
    {
      if (_currentSecretWordText != null)
      {
        _currentSecretWordText.gameObject.SetActive(false);
      }
    }

  }
}
