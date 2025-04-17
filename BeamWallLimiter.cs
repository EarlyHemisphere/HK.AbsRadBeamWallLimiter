using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using UnityEngine;

namespace BeamWallLimiter {
    public class BeamWallLimiter: MonoBehaviour {
        private PlayMakerFSM attackChoicesFSM;
        
        private bool initialBeamWallUsed = false;
        private bool isPlatsPhase = false;
        private int numAttacksBeforeBeamWalls = -1;
        private BeamWallDirection currentBeamWallDirection;
        private enum BeamWallDirection {
            Left,
            Right
        }
        private const int NUM_ATTACKS_BEFORE_BEAM_WALL = 5;

        public void Awake() {
            On.PlayMakerFSM.OnEnable += OnEnableFSM;
        }

        public void OnEnableFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);

            if (self.gameObject.name == "Absolute Radiance" && self.FsmName == "Attack Choices") {
                attackChoicesFSM = self;

                self.InsertAction("Beam Sweep L 2", new CallMethod {
                    behaviour = this,
                    methodName = "BeamWallAttackLeft",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);
                self.InsertAction("Beam Sweep R 2", new CallMethod {
                    behaviour = this,
                    methodName = "BeamWallAttackRight",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);
                self.InsertAction("Beam Sweep L", new CallMethod {
                    behaviour = this,
                    methodName = "BeamWallAttackLeft",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);
                self.InsertAction("Beam Sweep R", new CallMethod {
                    behaviour = this,
                    methodName = "BeamWallAttackRight",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);
                
                self.InsertAction("A2 Pause", new CallMethod {
                    behaviour = this,
                    methodName = "PlatsPhaseStarted",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);

                self.InsertAction("A1 Choice", new CallMethod {
                    behaviour = this,
                    methodName = "BeamWallCheck",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);
                self.InsertAction("A2 Choice", new CallMethod {
                    behaviour = this,
                    methodName = "BeamWallCheck",
                    parameters = new FsmVar[0],
                    everyFrame = false
                }, 0);

                isPlatsPhase = false;
                initialBeamWallUsed = false;
                numAttacksBeforeBeamWalls = -1;
            }
        }

        public void BeamWallAttackLeft() {
            BeamWallAttack(BeamWallDirection.Left);
        }

        public void BeamWallAttackRight() {
            BeamWallAttack(BeamWallDirection.Right);
        }

        private void BeamWallAttack(BeamWallDirection direction) {
            if (initialBeamWallUsed) {
                if (numAttacksBeforeBeamWalls == -1) return;

                numAttacksBeforeBeamWalls--;
                attackChoicesFSM.SetState("Return");
                return;   
            }

            initialBeamWallUsed = true;

            if (!isPlatsPhase) {
                attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 2).weights = new FsmFloat[]{0.66f, 0.66f, 0.97f, 1.32f, 0, 0, 1.32f, 1.32f};
            } else {
                attackChoicesFSM.GetAction<SendRandomEventV3>("A2 Choice", 2).weights = new FsmFloat[]{0.71f, 1.43f, 1.43f, 1.43f, 0, 0};
            }

            currentBeamWallDirection = direction == BeamWallDirection.Left ? BeamWallDirection.Right : BeamWallDirection.Left;
            numAttacksBeforeBeamWalls = -1;
        }

        public void PlatsPhaseStarted() {
            isPlatsPhase = true;
            initialBeamWallUsed = false;
            numAttacksBeforeBeamWalls = -1;
        }

        public void BeamWallCheck() {
            numAttacksBeforeBeamWalls++;

            if (numAttacksBeforeBeamWalls == NUM_ATTACKS_BEFORE_BEAM_WALL) {
                if (!initialBeamWallUsed) {
                    attackChoicesFSM.SetState(isPlatsPhase ? "Beam Sweep L 2" : "Beam Sweep L");
                } else {
                    string newState = currentBeamWallDirection == BeamWallDirection.Left
                        ? "Beam Sweep L" + (isPlatsPhase ? " 2" : "")
                        : "Beam Sweep R" + (isPlatsPhase ? " 2" : "");
                    currentBeamWallDirection = currentBeamWallDirection == BeamWallDirection.Left ? BeamWallDirection.Right : BeamWallDirection.Left;
                    numAttacksBeforeBeamWalls = -1;
                    attackChoicesFSM.SetState(newState);
                }
            }
        }
    }
}