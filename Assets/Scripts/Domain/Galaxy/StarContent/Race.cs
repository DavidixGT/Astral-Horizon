using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Domain;
using Constructor.Ships;
using Economy;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameDatabase.Enums;
using GameServices.Economy;
using GameServices.Random;
using GameStateMachine.States;
using GameDatabase.DataModel;
using GameDatabase.Query;
using GameDatabase.Model;
using Model.Factories;
using Session;
using UnityEngine;
using Zenject;

namespace Galaxy.StarContent
{
    
    public class Race
    {
        [Inject] private readonly IRandom _random;
    }
}
