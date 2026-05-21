using UnityEngine;
using System.Linq;
using KSP.Localization;

namespace ModuleAnimateGenericEffects
{

#if false
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
            //if (!HighLogic.LoadedSceneIsEditor)
            {
                base.Toggle();
                int num = base.part.symmetryCounterparts.Count;
                while (num-- > 0)
                {
                    if ((Object)base.part.symmetryCounterparts[num] != (Object)base.part)
                    {
                        base.part.symmetryCounterparts[num].Modules.GetModule<ModuleAnimateGenericEffects>(0).DoToggle();
                    }
                }
            }
        }
        void DoToggle()
        {
            base.Toggle();
        }
    }
#endif
#if false
    public class ModuleDeployableSolarPanelwSecondaryAnim : ModuleDeployableSolarPanel
    {
        internal static Dictionary<string, List<Sequence>> seqDict = null;
        Sequence.MovementStatus movementStatus = Sequence.MovementStatus.undef;
        List<Sequence> sequenceAnimations = null;

        Sequence firstSeq = null;
        Sequence lastSeq = null;

        #region KSPFields
        [KSPField]
        public new string extendActionName = "#autoLOC_6001354";

        [KSPField]
        public new string retractActionName = "#autoLOC_6001354";

        [KSPField]
        public new string extendpanelsActionName = "#autoLOC_6001354";
        #endregion

        [KSPEvent(guiActive = true, guiActiveEditor = true,  guiName = "Extend test" )]
        public   void Extend2()
        {
            base.Extend();
            StartCoroutine("SlowUpdate");

        }

        public void Start()
        {
            Events["Extend"].active = false;

        }

        int cnt = 0;
        IEnumerator SlowUpdate()
        {
            while (true)
            {

                yield return new WaitForSeconds(1f);
                if (anim.isPlaying )
                {
                    if (cnt++ >8)
                        anim.Stop();
                    continue;
                }
                ModuleAnimateGeneric mag = part.FindModuleImplementing<ModuleAnimateGeneric>();
                if (mag != null)
                {
                    if (!mag.IsMoving())
                    {
                        mag.enabled = true;
                        mag.isOneShot = true;
                        mag.Toggle();
                    }
                }
                else
                {
                }
                if (mag.Progress < 1)
                    yield return new WaitForSeconds(1f);
                else
                {
                    mag.Toggle();
                    yield return null;
                }
            }
        }
    }
#endif

}