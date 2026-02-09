using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using Satchel.BetterMenus;
using UnityEngineInternal;

namespace GeoMultiplier
{

    public class GlobalSettings
    {
        public float generalMultiplier = 1f;
        public float smallGeoMultiplier = 1f;
        public float mediumGeoMultiplier = 1f;
        public float largeGeoMultiplier = 1f;
        public bool roundingMode;
        public bool keepShadeGeo = false;
    }

    public class EasierGeoEconomy : Mod, ICustomMenuMod, IGlobalSettings<GlobalSettings>
    {
        public EasierGeoEconomy() : base("EasierGeoEconomy") { }
        public override string GetVersion() => "0.1";
        public static GlobalSettings GS = new GlobalSettings();
        private Menu MenuRef;
        private int shadeGeo;

        public override void Initialize()
        {
            On.HeroController.AddGeo += AddGeo;
            ModHooks.AfterPlayerDeadHook += AfterPlayerDead;
        }

        public void AddGeo(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
        {

            Log($"Vanilla added amount: {amount}");
            if (amount == 1)
            {
                Log($"multiplied is {amount * GS.smallGeoMultiplier * GS.generalMultiplier}, small multiplier: {GS.smallGeoMultiplier}, overal multiplier: {GS.generalMultiplier}");
                orig(self, Rounding(amount * GS.smallGeoMultiplier * GS.generalMultiplier));
            }
            else if (amount == 5)
            {
                orig(self, Rounding(amount * GS.mediumGeoMultiplier * GS.generalMultiplier));
            }
            else if (amount == 25)
            {
                orig(self, Rounding(amount * GS.largeGeoMultiplier * GS.generalMultiplier));
            }
            else
            {
                orig(self, amount);
            }
        }

        public void AfterPlayerDead()
        {
            if (!GS.keepShadeGeo) return;
            PlayerData pd = PlayerData.instance;
            shadeGeo = pd.GetVariable<int>("geoPool");
            pd.SetVariable<int>("geo", shadeGeo);
            pd.SetVariable<int>("geoPool", 0);
        }
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates)
        {
            MenuRef ??= new Menu(
                        name: "GeoMultiplier",
                        elements: new Element[]
                        {
                        new HorizontalOption(
                            name: "Keep Geo",
                            description: "Keep geo upon death. The shade spawns regardless",
                            values: new [] { "Yes", "No" },
                            applySetting: index =>
                            {
                                GS.keepShadeGeo = index == 0;
                            },
                            loadSetting: () => GS.keepShadeGeo ? 0 : 1),
                        new CustomSlider(
                            name: "General Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.generalMultiplier = (float) val;
                            },
                            loadValue: () => GS.generalMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 5,
                            wholeNumbers: false
                            ),

                        new CustomSlider(
                            name: "Small Geo Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.smallGeoMultiplier = (float) val;
                            },
                            loadValue: () => GS.smallGeoMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 5,
                            wholeNumbers: false
                            ),

                        new CustomSlider(
                            name: "Medium Geo Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.mediumGeoMultiplier = (float) val;
                            },
                            loadValue: () => GS.mediumGeoMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 5,
                            wholeNumbers: false
                            ),

                        new CustomSlider(
                            name: "Large Geo Multiplier",
                            storeValue: val => // to store the value when the slider is changed by user
                            {
                                GS.largeGeoMultiplier = (float) val;
                            },
                            loadValue: () => GS.largeGeoMultiplier, //to load the value on menu creation
                            minValue: 0,
                            maxValue: 5,
                            wholeNumbers: false
                            ),
                        new HorizontalOption(
                            name: "Round up or down",
                            description: "Rounding up means 1.1 gets rounded to 2",
                            values: new [] { "Round up", "Round down" },
                            applySetting: index =>
                            {
                                GS.roundingMode = index == 0;
                            },
                            loadSetting: () => GS.roundingMode ? 0 : 1)
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

        private int Rounding(float value)
        {
            Log($"Value is : {value}, rounding mode is : {GS.roundingMode}");
            if (!GS.roundingMode)
            {
                return Mathf.FloorToInt(value);
            }
            else
            {
                return Mathf.CeilToInt(value);
            }
        }
    }
}
