﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SharpSenses.Gestures;
using SharpSenses.Poses;
using SharpSenses.RealSense.Capabilities;
using static System.Console;

namespace SharpSenses.RealSense.Playground {
    internal class Program {

        private static RealSenseCamera _cam;
        private static Dictionary<string, string> _items = new Dictionary<string, string>();

        private static StringBuilder _sb = new StringBuilder();
        private static object _syncRoot = new object();

        private static void Main(string[] args) {
            Item.DefaultNoiseThreshold = 0;

            RealSenseAssembliesLoader.Load();
            _cam = new RealSenseCamera();
            
            //TestHands();
            //TestFace();
            //TestFaceRecognition();
            //TestFacialExpressions();
            //TestEmotions();
            //TestSpeech();
            TestGestures();
            _cam.Start();

            ReadLine();
            _cam.Dispose();
        }

        private static void Update(string key, string value, string valueAfterTwoSeconds = null) {
            lock (_syncRoot) {
                _items[key] = value;
                _sb.Clear();
                foreach (var k in _items.Keys) {
                    _sb.Append(k).Append(": ").AppendLine(_items[k]);
                }
                Clear();
                WriteLine(_sb.ToString());
                if (valueAfterTwoSeconds != null) {
                    UpdateLater(key, valueAfterTwoSeconds);
                }
            }
        }

        private static void UpdateLater(string key, string value, int delayInMillis = 2000) {
            Task.Run(async () => {
                await Task.Delay(delayInMillis);
                Update(key, value);
            });
        }

        private static void TestHands() {
            _cam.AddCapability(Capability.HandTracking);

            _cam.RightHand.Visible += (s, a) => { Update("Hand Right Visible", "True"); };
            _cam.RightHand.NotVisible += (s, a) => { Update("Hand Right Visible","False"); };
            _cam.RightHand.Opened += (s, a) => { Update("Hand Right Open", "True"); };
            _cam.RightHand.Closed += (s, a) => { Update("Hand Right Open", "False");};

            _cam.LeftHand.Visible += (s, a) => { Update("Hand Left Visible", "True"); };
            _cam.LeftHand.NotVisible += (s, a) => { Update("Hand Left Visible", "False"); };
            _cam.LeftHand.Opened += (s, a) => { Update("Hand Left Open", "True"); };
            _cam.LeftHand.Closed += (s, a) => { Update("Hand Left Open", "False"); };

            _cam.RightHand.Moved += (s, a) => { Update("Hand Right Move", a.NewPosition.World.ToString()); };
            _cam.LeftHand.Moved += (s, a) => { Update("Hand Left Move", a.NewPosition.World.ToString()); };

            _cam.RightHand.RotationChanged += (s, a) => {
                var roll = _cam.RightHand.Rotation.Roll;
                var yaw = _cam.RightHand.Rotation.Yaw;
                var pitch = _cam.RightHand.Rotation.Pitch;
                Update("Hand Right Rotation", $"Roll: {roll:0} Yaw: {yaw:0} Pitch {pitch:0}");
            };
            _cam.LeftHand.RotationChanged += (s, a) => {
                var roll = _cam.LeftHand.Rotation.Roll;
                var yaw = _cam.LeftHand.Rotation.Yaw;
                var pitch = _cam.LeftHand.Rotation.Pitch;
                Update("Hand Left Rotation", $"Roll: {roll:0} Yaw: {yaw:0} Pitch {pitch:0}");
            };
        }

        private static void TestFace() {
            _cam.AddCapability(Capability.FaceTracking);

            _cam.Face.Visible += (s, a) => { Update("Face Visible", "True -> id:" + _cam.Face.UserId); };
            _cam.Face.NotVisible += (s, a) => { Update("Face Visible", "False -> id:" + _cam.Face.UserId); };
            _cam.Face.Moved += (s, a) => { Update("Face Move", a.NewPosition.World.ToString()); };
        }

        private static void TestFacialExpressions() {
            _cam.AddCapability(Capability.FacialExpressionTracking);
            _cam.Face.Mouth.Smiled += (s, a) => { Update("Facial Smile", "True", "False"); };
            _cam.Face.EyesDirectionChanged += (s, a) => { Update("Eyes direction", a.NewDirection.ToString()); };

            _cam.Face.LeftEye.Opened += (s, a) => { Update("Eye Left Open", "True"); };
            _cam.Face.LeftEye.Closed += (s, a) => { Update("Eye Left Open", "False"); };
            _cam.Face.LeftEye.Blink += (s, a) => { Update("Eye Left Blink", "True", "False"); };
            _cam.Face.LeftEye.DoubleBlink += (s, a) => { Update("Eye Left Blink", "True", "False"); };
            _cam.Face.WinkedLeft += (s, a) => { Update("Winked Left", "True", "False"); };

            _cam.Face.RightEye.Opened += (s, a) => { Update("Eye Right Open", "True"); };
            _cam.Face.RightEye.Closed += (s, a) => { Update("Eye Right Open", "False"); };
            _cam.Face.RightEye.Blink += (s, a) => { Update("Eye Right Blink", "True", "False"); };
            _cam.Face.RightEye.DoubleBlink += (s, a) => { Update("Eye Right Blink", "True", "False"); };
            _cam.Face.WinkedRight += (s, a) => { Update("Winked Right", "True", "False"); };
        }

        private static void TestEmotions() {
            _cam.AddCapability(Capability.EmotionTracking);

            _cam.Face.EmotionChanged += (sender, args) => {
                WriteLine("Emotion: " + args.NewEmotion);
            };
        }

        private static void TestFaceRecognition() {
            _cam.Face.FaceRecognized += (sender, eventArgs) => {
                WriteLine("User: " + eventArgs.UserId);
            };

            while (true) {
                ReadLine();
                _cam.Face.RecognizeFace();
                WriteLine("Recognize!");
            }
        }

        private static void TestGestures() {
            _cam.AddCapability(Capability.GestureTracking);

            _cam.Gestures.SlideLeft += (s, a) => Update("Swipe Left","True","False");
            _cam.Gestures.SlideRight += (s, a) => Update("Swipe Right", "True", "False");
            _cam.Gestures.SlideUp += (s, a) => Update("Swipe Up", "True", "False");
            _cam.Gestures.SlideDown += (s, a) => Update("Swipe Down", "True", "False");
            _cam.Gestures.MoveForward += (s, a) => Update("Move Forward", "True", "False");
        }

        private static void TestSpeech() {
            _cam.Speech.SpeechRecognized += (sender, eventArgs) => {
                WriteLine("-> Speech Recognized: " + eventArgs.Sentence.ToLower());
            };
            _cam.Speech.EnableRecognition(SupportedLanguage.PtBR);
        }
       
        private static void TrackMovement(Movement m) {
            m.Completed += () => WriteLine(m.Name + " -> DONE!!!!");
            m.Restarted += () => WriteLine(m.Name + " -> Restarted");
            m.Progress += d => {
                Write(m.Name + " -> ");
                for (int i = 0; i < d; i++) {
                    Write("-");
                }
                WriteLine(">");
            };
        }

        private static void TrackHandMovement(ICamera cam) {
            int i = 0;
            cam.LeftHand.Moved += (s,a) => {
                var d = a.NewPosition;
                i++;
                if (i%3 == 0) {
                    WriteLine("Left: IX: {0} IY: {1} WX: {2}, WY:{3} WZ: {4} ",
                        d.Image.X, d.Image.Y, d.World.X, d.World.Y, d.World.Z);
                }
            };
            cam.RightHand.Moved += (s, a) => {
                var d = a.NewPosition;
                WriteLine("Right: X: {0} Y: {1} Z: {2}", d.Image.X, d.Image.Y, d.World.Z);
            };
        }
    }
}