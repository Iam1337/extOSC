/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using System.Reflection;

using extOSC.UI;
using extOSC.Editor.Windows;
using extOSC.Core.Reflection;
using extOSC.Components.Informers;

namespace extOSC.Editor
{
    public static class OSCMenuOptions
    {
        #region Extensions

        private delegate GameObject CreateCallback(OSCControls.Resources resources);

        #endregion

        #region Static Private Vars

        private static MethodInfo _placeUIElementMethod;

        #endregion

        #region Static Public Methods

        [MenuItem("GameObject/extOSC/OSC Manager", false, 40)]
        public static void AddManager(MenuCommand menuCommand)
        {
            var gameObject = OSCControls.CreateManager();
            Undo.RegisterCreatedObjectUndo(gameObject, "Create OSC Manager");
        }

        [MenuItem("GameObject/extOSC/Pad", false, 42)]
        public static void AddPad(MenuCommand menuCommand)
        {
            OSCWindowControlCreator.ShowWindow(menuCommand, (data, command) =>
            {
                InitUIElement<OSCPad, OSCTransmitterInformerVector2>(OSCControls.CreatePad, data, command);
            });
        }

        [MenuItem("GameObject/extOSC/Slider", false, 43)]
        public static void AddSlider(MenuCommand menuCommand)
        {
            OSCWindowControlCreator.ShowWindow(menuCommand, (data, command) =>
            {
                InitUIElement<OSCSlider, OSCTransmitterInformerFloat>(OSCControls.CreateSlider, data, command);
            });
        }

        [MenuItem("GameObject/extOSC/Button", false, 44)]
        public static void AddButton(MenuCommand menuCommand)
        {
            OSCWindowControlCreator.ShowWindow(menuCommand, (data, command) =>
            {
                InitUIElement<OSCButton, OSCTransmitterInformerBool>(OSCControls.CreateButton, data, command);
            });
        }

        [MenuItem("GameObject/extOSC/Rotary", false, 45)]
        public static void AddRotary(MenuCommand menuCommand)
        {
            OSCWindowControlCreator.ShowWindow(menuCommand, (data, command) =>
            {
                InitUIElement<OSCRotary, OSCTransmitterInformerFloat>(OSCControls.CreateRotary, data, command);
            });
        }

        [MenuItem("GameObject/extOSC/Multiply Sliders (Vertical)", false, 46)]
        public static void AddMultiplySlidersVertical(MenuCommand menuCommand)
        {
            OSCWindowControlCreator.ShowWindow(menuCommand, (data, command) =>
            {
                InitMultiplySlidersUIElement(OSCControls.CreateMultiplySlidersVertical, data, command);
            });
        }

        [MenuItem("GameObject/extOSC/Multiply Sliders (Horizontal)", false, 47)]
        public static void AddMultiplySlidersHorizontal(MenuCommand menuCommand)
        {
            OSCWindowControlCreator.ShowWindow(menuCommand, (data, command) =>
            {
                InitMultiplySlidersUIElement(OSCControls.CreateMultiplySlidersHorizontal, data, command);
            });
        }

        #endregion

        #region Static Private Methods

        private static void InitUIElement<T, K>(CreateCallback createAction,
                                                 OSCWindowControlCreator.ControlData data,
                                                 MenuCommand menuCommand) where K : OSCTransmitterInformer where T : Component
        {
            if (createAction == null)
                return;

            var resources = OSCEditorUtils.GetStandardResources();
            resources.Color = data.ControlColor;

            var element = createAction(resources);

            PlaceUIElement(element, menuCommand);

            if (data.UseInformer)
            {
                AddInformer<K>(element.GetComponent<T>(),
                                                         data.InformerTransmitter,
                                                         data.InformAddress,
                                                         data.InformOnChanged,
                                                         data.InformInterval);
            }
        }

        private static void InitMultiplySlidersUIElement(CreateCallback createAction, OSCWindowControlCreator.ControlData data, MenuCommand menuCommand)
        {
            if (createAction == null)
                return;

            var resources = OSCEditorUtils.GetStandardResources();
            resources.Color = data.ControlColor;

            var element = createAction(resources);

            if (data.UseInformer)
            {
                var multiplySliders = element.GetComponent<OSCMultiplySliders>();
                multiplySliders.Address = data.InformAddress;
                multiplySliders.Transmitter = data.InformerTransmitter;
            }

            PlaceUIElement(element, menuCommand);
        }

        private static void AddInformer<T>(Component component,
                                           OSCTransmitter transmitter,
                                           string address,
                                           bool onChanged,
                                           float interval) where T : OSCTransmitterInformer
        {
            var informer = component.gameObject.AddComponent<T>();
            informer.TransmitterAddress = address;
            informer.Transmitter = transmitter;
            informer.InformOnChanged = onChanged;
            informer.InformInterval = interval;

            var reflection = new OSCReflectionMember();
            reflection.Target = component;
            reflection.MemberName = "Value";

            if (!reflection.IsValid())
                reflection.MemberName = "value";

            informer.ReflectionTarget = reflection;
        }

        private static void PlaceUIElement(GameObject gameObject, MenuCommand menuCommand)
        {
            if (_placeUIElementMethod == null)
            {
                var assembly = Assembly.GetAssembly(typeof(SelectableEditor));
                var menuOptionsType = assembly.GetType("UnityEditor.UI.MenuOptions");

                _placeUIElementMethod = menuOptionsType.GetMethod("PlaceUIElementRoot", BindingFlags.Static | BindingFlags.NonPublic);
            }

            _placeUIElementMethod.Invoke(null, new object[] { gameObject, menuCommand });
        }

        #endregion
    }
}