using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    [Serializable]
    public class TransitionHolder
    {
        [SerializeField] private StyleSheet styleSheet;
        public StyleSheet StyleSheet => styleSheet;

        public string TransitionShort => "transition-short";
        public string TransitionLong => "transition-long";
        public string Fade => "fade";
        public string SlideDownFade => "slide-down-fade";
        public string SlideRightFade => "slide-right-fade";
    }
}