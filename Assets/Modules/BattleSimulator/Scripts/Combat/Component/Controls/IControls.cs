﻿using System.Collections;

namespace Combat.Component.Controls
{
    public interface IControls
    {
        bool DataChanged { get; set; }

        float Throttle { get; set; }
        float? Course { get; set; }

        SystemsState Systems { get; }
    }
}
