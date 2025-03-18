using UnityEngine;
using VContainer;
using VContainer.Unity;
using GemmaCpp;


namespace GoogleDeepMind.GemmaSampleGame
{
    /**
     * This is a VContainer lifetime scope for the ChatbotTest scene.
     */
    public class ChatbotTestLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private GemmaManagerSettings gemmaSettings;

        [SerializeField]
        private ChatbotForm chatbotForm;

        protected override void Configure(IContainerBuilder builder)
        {
            // Create and configure GemmaManager
            var gemmaManagerObj = new GameObject("GemmaManager");
            var gemmaManager = gemmaManagerObj.AddComponent<GemmaManager>();
            gemmaManager.settings = gemmaSettings;
            DontDestroyOnLoad(gemmaManagerObj);

            // Register GemmaManager for injection
            builder.RegisterComponent(gemmaManager);

            // Register ChatbotForm for injection
            builder.RegisterComponent(chatbotForm);
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

    }
}