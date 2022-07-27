using UnityEngine;
using KSP;
using KSP.UI.Screens;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModuleAnimateGenericEffects
{
    public class ModuleAnimateGenericEffects : ModuleAnimateGeneric
    {
        #region KSPFields
        [KSPField]
        public string deployEffectName = "deploy";

        [KSPField]
        public string postDeployEffectName = "";

        [KSPField]
        public float postDeployEffectLength = 0.5f;

        [KSPField]
        public string retractEffectName = "retract";

        [KSPField]
        public string postRetractEffectName = "";

        [KSPField]
        public float postRetractEffectLength = 0.5f;

        [KSPField(isPersistant = true)]
        new public float animSpeed = -99f;

        #endregion

        string currentEffect = "";

        public void Start()
        {
            OnMoving.Add(DoOnMovingEffect);
            OnStop.Add(StopEffect);

            SetUpAnimation();

            if (reverseGUIName != "")
            {
                Events["Reverse"].active = Events["Reverse"].guiActive = Events["Reverse"].guiActiveUnfocused = true;

                Events["Reverse"].guiName = reverseGUIName;
            }
        }

        public void DoOnMovingEffect(float f1, float f2)
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (anim[animationName].time == 0)
            {
                currentEffect = retractEffectName;
                if (animSpeed != -99)
                    anim[animationName].speed = animSpeed;
            }
            else
            {
                currentEffect = deployEffectName;
                if (animSpeed != -99)
                    anim[animationName].speed = -animSpeed;
            }
            if (currentEffect != "")
                base.part.Effect(currentEffect, 1f, -1);

        }

        public void StopEffect(float f1)
        {
            //if (HighLogic.LoadedSceneIsEditor)
            //    return;


            base.part.Effect(currentEffect, 0f, -1);
            float len = 0.5f;
            if (deployPercent > 0)
            {
                currentEffect = postDeployEffectName;
                len = postDeployEffectLength;
            }
            else
            {
                currentEffect = postRetractEffectName;
                len = postRetractEffectLength;
            }
            if (currentEffect != "")
            {
                base.part.Effect(currentEffect, 1f, -1);
                StartCoroutine(StopIt(len));
            }
        }

        #region RampUp
        // *****************************************************************************************************
        // animationRampspeed is how quickly it gets up to speed.  1 meaning it gets to full speed (as defined by 
        // the animSpeed and customAnimationSpeed) immediately, less than that will ramp up over time
        [KSPField]
        public float animationRampSpeed = 0.005f;

        [KSPField]
        public float customAnimationSpeed = 1f;

        private enum RampDirection { none, up, down };

        [KSPField]
        private RampDirection rampDirection = RampDirection.none;

        [KSPField]
        public string reverseGUIName = "";

        [KSPField]
        private float ramp;
        // private int animStage;

        [KSPField]
        bool stopInProgress = false;

        [KSPField]
        float minSpeed = 0f;
        [KSPField]
        float maxSpeed = 1f;
        [KSPField]
        bool reverseInProgress = false;

#if false
        void DisplayRampData(string str)
        {
            Debug.Log("ModuleAnimateGenericEffects." + str + ", animationRampSpeed: " + animationRampSpeed.ToString("F3") +
                ", rampDirection: " + rampDirection +
                ", ramp: " + ramp.ToString("F3") +
                ", minSpeed: " + minSpeed + ", maxSpeed: " + maxSpeed + ", reverseInProgress: " + reverseInProgress + ", stopInProgress: " + stopInProgress);
        }
#endif

        public void SetUpAnimation()
        {
            var animationState = anim[animationName];

            animationState.speed = 0;
            animationState.normalizedTime = 0f;
            animationState.enabled = true;
            animationState.wrapMode = WrapMode.Loop;
            ramp = 0;
        }

        public void UpdateAnimations()
        {
            if (anim != null)
            {
                float origSpd = anim[animationName].speed;
                switch (rampDirection)
                {
                    case RampDirection.up:
                        if (ramp < maxSpeed)
                        {
                            ramp += animationRampSpeed;
                        }
                        ramp = Math.Min(ramp, maxSpeed);
                        break;
                    case RampDirection.down:
                        if (ramp > minSpeed)
                        {
                            ramp += animationRampSpeed;
                        }
                        ramp = Math.Max(ramp, minSpeed);
                        break;
                }

                anim[animationName].speed = customAnimationSpeed * ramp;



                if (stopInProgress)
                {
                    if ((rampDirection == RampDirection.down && ramp <= 0) ||
                        (rampDirection == RampDirection.up && ramp >= 0))
                    {
                        rampDirection = RampDirection.none;
                        anim[animationName].speed = 0;
                        animationRampSpeed = -animationRampSpeed;
                        ramp = 0;
                        stopInProgress = false;
                        reverseInProgress = false;
                    }
                }
                else
                {
                    if (!reverseInProgress)
                    {
                        if (ramp == 0)
                        {
                            rampDirection = RampDirection.none;
                        }
                        else if (ramp == 1 || ramp == -1)
                        {
                            rampDirection = RampDirection.none;
                        }
                    }
                    else
                    {
                        switch (rampDirection)
                        {
                            case RampDirection.up:
                                if (ramp > 0)
                                    reverseInProgress = false;
                                break;
                            case RampDirection.down:
                                if (ramp < 0)
                                    reverseInProgress = false;
                                break;
                        }

                    }
                }
                if (anim[animationName].speed != 0 && !anim.IsPlaying(animationName))
                    anim.Play(animationName);
            }
        }


        void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight) // && animationRampSpeed != 0)
            {
                UpdateAnimations();
                //DisplayRampData("FixedUpdate");
            }
        }
        // *****************************************************************************************************
        #endregion



        IEnumerator StopIt(float time)
        {
            yield return new WaitForSeconds(time);
            base.part.Effect(currentEffect, 0f, -1);
            currentEffect = "";
        }

        public void OnDestroy()
        {
            OnMoving.Remove(DoOnMovingEffect);
            OnStop.Remove(StopEffect);
        }

        [KSPEvent(unfocusedRange = 5f, guiActiveUnfocused = true, guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001329")]
        new public void Toggle()
        {
            //DisplayRampData("Toggle start");
            //if (!HighLogic.LoadedSceneIsEditor)
            if (!stopInProgress)
            {
                if (animationRampSpeed > 0)
                {
                    if (anim[animationName].speed == 0)
                        rampDirection = RampDirection.up;
                    else
                        rampDirection = RampDirection.down;
                }
                else
                {
                    if (anim[animationName].speed == 0)
                        rampDirection = RampDirection.down;
                    else
                        rampDirection = RampDirection.up;

                }
                if (ramp != 0)
                {
                    stopInProgress = true;
                    animationRampSpeed = -animationRampSpeed;
                }
                if (!stopInProgress)
                {
                    if (rampDirection == RampDirection.down)
                    {
                        minSpeed = -1f;
                        maxSpeed = 0f;
                    }
                    else
                    {
                        minSpeed = 0f;
                        maxSpeed = 1f;
                    }
                }
                //DisplayRampData("Toggle end");

                base.Toggle();
                int num = base.part.symmetryCounterparts.Count;
                while (num-- > 0)
                {
                    if ((UnityEngine.Object)base.part.symmetryCounterparts[num] != (UnityEngine.Object)base.part)
                    {
                        base.part.symmetryCounterparts[num].Modules.GetModule<ModuleAnimateGenericEffects>(0).DoToggle();
                    }
                }
            }
        }

        [KSPEvent(unfocusedRange = 5f, active = false, guiActiveUnfocused = false, guiActive = false, guiActiveEditor = false, guiName = "filler")]
        public void Reverse()
        {
            if (anim[animationName].speed != 0)
            {
                reverseInProgress = true;
                animationRampSpeed = -animationRampSpeed;
                if (anim[animationName].speed < 0)
                {
                    minSpeed = 0f;
                    maxSpeed = 1f;

                    rampDirection = RampDirection.up;
                }
                else
                {
                    minSpeed = -1f;
                    maxSpeed = 0f;

                    rampDirection = RampDirection.down;
                }
                int num = base.part.symmetryCounterparts.Count;
                while (num-- > 0)
                {
                    if ((UnityEngine.Object)base.part.symmetryCounterparts[num] != (UnityEngine.Object)base.part)
                    {
                        base.part.symmetryCounterparts[num].Modules.GetModule<ModuleAnimateGenericEffects>(0).Reverse();
                    }
                }

            }
        }

        void DoToggle()
        {
            base.Toggle();
        }
    }
}