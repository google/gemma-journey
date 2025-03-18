using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Gemma Sample Game/GameSettings")]
    public class GameSettings : ScriptableObject
{
    [Header("Urls")]
    [SerializeField] private string privacyPolicyUrl;
    [SerializeField] private string termsOfUseUrl;
    [SerializeField] private string learnMoreUrl;
    [SerializeField] private string licenseUrl;

    public string PrivacyPolicyUrl => privacyPolicyUrl;
    public string TermsOfUseUrl => termsOfUseUrl;
    public string LearnMoreUrl => learnMoreUrl;
    public string LicenseUrl => licenseUrl;
}
}
