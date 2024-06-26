﻿using System.Diagnostics;
using System.Linq;
using Galaxy;
using Services.Localization;
using Services.Messenger;
using Services.Resources;
//using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;
using Star = Galaxy.Star;

namespace Gui.StarMap
{
    public class StarSystemObjectItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;

        [SerializeField] private Sprite SpaceStationIcon;
        [SerializeField] private Sprite StarBaseIcon;
        [SerializeField] private Sprite BeaconIcon;
        [SerializeField] private Sprite PandemicIcon;
        [SerializeField] private Color PandemicIconColor;

		public StarObjectType Type { get; private set; }

		public void Initialize(Galaxy.Star star, StarObjectType objectType, ILocalization localization, IResourceLocator resourceLocator)
        {
            Type = objectType;
            _name.text = localization.GetString("$Object" + objectType);
            _icon.color = Color.white;

            switch (objectType)
            {
                case StarObjectType.Race:
                    UnityEngine.Debug.LogError("Setting up race star!!!!!!!!");
                    _icon.sprite = StarBaseIcon;
                    _icon.color = Color.red;
                    break;
                case StarObjectType.Event:
                    _icon.sprite = BeaconIcon;
                    break;
                case StarObjectType.StarBase:
                    _icon.sprite = StarBaseIcon;
                    _icon.color = star.Region.Faction.Color;
                    break;
                case StarObjectType.Hive:
                    _icon.sprite = PandemicIcon;
                    _icon.color = PandemicIconColor;
                    break;
                case StarObjectType.Boss:
                    var ship = star.Boss.CreateFleet().Ships.First();
                    _icon.sprite = resourceLocator.GetSprite(ship.Model.IconImage) ?? resourceLocator.GetSprite(ship.Model.ModelImage);
                    break;

                default:
                    _icon.sprite = SpaceStationIcon;
                    break;
            }
        }
    }
}
