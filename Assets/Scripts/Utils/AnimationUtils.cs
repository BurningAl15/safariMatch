using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum AnimationType
{
   Scale,
   Move,
   Rotate,
}

[Serializable]
public class AnimationSettings
{
   public AnimationType animationType;
   public float endValue;
   public float duration;
   public float delay;
   public Ease ease;

   public AnimationSettings(AnimationType _animationType, float _endValue, float _duration, float _delay, Ease _ease)
   {
      animationType = _animationType;
      endValue = _endValue;
      duration = _duration;
      delay = _delay;
      ease = _ease;
   }
}

public static class AnimationUtils
{
   public static List<AnimationSettings> reboundAnimationSettings = new List<AnimationSettings>()
   {
      new AnimationSettings(AnimationType.Scale, 1.25f, .2f, 0, Ease.InOutCubic),
      new AnimationSettings(AnimationType.Scale, .3f, .3f,  .2f, Ease.InOutCubic),
      new AnimationSettings(AnimationType.Scale, 1.1f, .1f, .3f, Ease.InOutCubic),
      new AnimationSettings(AnimationType.Scale, 1f, .1f, .1f, Ease.InOutCubic),
   };
}
