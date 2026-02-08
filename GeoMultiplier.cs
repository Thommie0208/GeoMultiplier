using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using Satchel.BetterMenus;

namespace GeoMultiplier
{

    public class GlobalSettings
    {
        public int geoMultiplier;
    }

    public class GeoMultiplier : Mod, ICustomMenuMod, IGlobalSettings<GlobalSettings>
    {
        public GeoMultiplier() : base("GeoMultiplier") { }
        public override string GetVersion() => "0.1";
        public static GlobalSettings GS = new GlobalSettings();
        private int geoCount = 0;
        private Menu MenuRef;
        public override void Initialize()
        {
            ModHooks.GetPlayerIntHook += PlayerIntGet;
            ModHooks.SetPlayerIntHook += PlayerIntSet;
        }

        public int PlayerIntGet(string target, int val)
        {
            if (target == "geo")
            {
                geoCount = val;
                Log($"PlayerIntGet called for geo with current value: {val}");
            }
            return val;
        }
        public int PlayerIntSet(string target, int val)
        {
            if (target == "geo")
            {
                Log($"PlayerIntSet called with target: {target} and value: {val}");
                int added = val - geoCount;
                val = geoCount + added * GS.geoMultiplier;
                Log($"Vanilla added: {added}, multiplier: {GS.geoMultiplier}, new value: {val}");
            }
            return val;
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates)
        {
            MenuRef ??= new Menu(
                        name: "GeoMultiplier",
                        elements: new Element[]
                        {
                        new CustomSlider(
                            name: "Geo Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.geoMultiplier = (int) val;
                            },
                            loadValue: () => GS.geoMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 10,
                            wholeNumbers: true
                            )
                        }
            );


            return MenuRef.GetMenuScreen(modListMenu);
        }

        public bool ToggleButtonInsideMenu { get; }

        public GlobalSettings OnSaveGlobal() => GS;

        public void OnLoadGlobal(GlobalSettings s)
        {
            GS = s ?? new GlobalSettings();
        }
    }
}
